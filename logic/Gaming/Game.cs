using GameClass.GameObj;
using GameClass.GameObj.Map;
using GameClass.GameObj.Areas;
using GameClass.MapGenerator;
using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gaming
{
    public partial class Game
    {
        public struct PlayerInitInfo(long teamID, long playerID, ShipType shipType)
        {
            public long teamID = teamID;
            public long playerID = playerID;
            public ShipType shipType = shipType;
        }
        private readonly List<Base> teamList;
        public List<Base> TeamList => teamList;
        private readonly Map gameMap;
        public Map GameMap => gameMap;
        private readonly Random random = new();
        public long AddPlayer(PlayerInitInfo playerInitInfo)
        {
            if (!gameMap.TeamExists(playerInitInfo.teamID))
            {
                return GameObj.invalidID;
            }
            if (playerInitInfo.playerID != 0)
            {
                // Add a ship
                var shipType = playerInitInfo.shipType;
                switch (shipType)
                {
                    case ShipType.Null:
                        return GameObj.invalidID;
                    case ShipType.CivilShip:
                        if (teamList[(int)playerInitInfo.teamID].ShipPool.GetNum(ShipType.CivilShip)
                            >= GameData.MaxCivilShipNum)
                        {
                            return GameObj.invalidID;
                        }
                        break;
                    case ShipType.WarShip:
                        if (teamList[(int)playerInitInfo.teamID].ShipPool.GetNum(ShipType.WarShip)
                            >= GameData.MaxWarShipNum)
                        {
                            return GameObj.invalidID;
                        }
                        break;
                    case ShipType.FlagShip:
                        if (teamList[(int)playerInitInfo.teamID].ShipPool.GetNum(ShipType.FlagShip)
                            >= GameData.MaxFlagShipNum)
                        {
                            return GameObj.invalidID;
                        }
                        break;
                    default:
                        return GameObj.invalidID;
                }
                Ship? newShip = ShipManager.AddShip(playerInitInfo.teamID,
                                                    playerInitInfo.playerID,
                                                    playerInitInfo.shipType,
                                                    teamList[(int)playerInitInfo.teamID].MoneyPool);
                if (newShip == null)
                {
                    return GameObj.invalidID;
                }
                teamList[(int)playerInitInfo.teamID].ShipPool.Append(newShip);
                return newShip.PlayerID;
            }
            else
            {
                // Add a home
                return playerInitInfo.playerID;
            }
        }
        public long ActivateShip(long teamID, ShipType shipType, int birthPointIndex = 0)
        {
            GameLogging.logger.ConsoleLogDebug($"Try to activate {teamID} {shipType} at birthpoint {birthPointIndex}");
            Ship? ship = teamList[(int)teamID].ShipPool.GetObj(shipType);
            if (ship == null)
            {
                GameLogging.logger.ConsoleLogDebug($"Fail to activate {teamID} {shipType}, no ship available");
                return GameObj.invalidID;
            }
            if (birthPointIndex < 0)
                birthPointIndex = 0;
            if (birthPointIndex >= teamList[(int)teamID].BirthPointList.Count)
                birthPointIndex = teamList[(int)teamID].BirthPointList.Count - 1;
            XY pos = teamList[(int)teamID].BirthPointList[birthPointIndex];
            pos += new XY(((random.Next() & 2) - 1) * 1000, ((random.Next() & 2) - 1) * 1000);
            if (shipManager.ActivateShip(ship, pos))
            {
                GameLogging.logger.ConsoleLogDebug($"Successfully activated {teamID} {shipType} at {pos}");
                return ship.PlayerID;
            }
            else
            {
                teamList[(int)teamID].ShipPool.ReturnObj(ship);
                GameLogging.logger.ConsoleLogDebug($"Fail to activate {teamID} {shipType} at {pos}, rule not permitted");
                return GameObj.invalidID;
            }
        }
        public bool StartGame(int milliSeconds)
        {
            if (gameMap.Timer.IsGaming)
                return false;
            // 开始游戏
            foreach (var team in TeamList)
            {
                actionManager.TeamTask(team);
                ActivateShip(team.TeamID, ShipType.CivilShip);
            }
            gameMap.Timer.Start(() => { }, () => EndGame(), milliSeconds);
            return true;
        }
        public void EndGame()
        {
        }
        public bool MoveShip(long teamID, long shipID, int moveTimeInMilliseconds, double angle)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
            {
                GameLogging.logger.ConsoleLogDebug(
                    "Try to move "
                    + LoggingFunctional.ShipLogInfo(ship)
                    + $" {moveTimeInMilliseconds} {angle}");
                return actionManager.MoveShip(ship, moveTimeInMilliseconds, angle);
            }
            else
            {
                GameLogging.logger.ConsoleLogDebug(
                    "Fail to move "
                    + LoggingFunctional.ShipLogInfo(teamID, shipID)
                    + ", not found");
                return false;
            }
        }
        public bool Produce(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
                return actionManager.Produce(ship);
            return false;
        }
        public bool Construct(long teamID, long shipID, ConstructionType constructionType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
            {
                return actionManager.Construct(ship, constructionType);
            }
            return false;
        }
        public bool InstallModule(long teamID, long shipID, ModuleType moduleType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
                return moduleManager.InstallModule(ship, moduleType);
            return false;
        }
        public bool Recover(long teamID, long shipID, long recover)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
            {
                bool validRecoverPoint = false;
                foreach (XY recoverPoint in teamList[(int)ship.TeamID].BirthPointList)
                {
                    if (GameData.ApproachToInteract(ship.Position, recoverPoint) && ship.Position != recoverPoint)
                    {
                        validRecoverPoint = true;
                        break;
                    }
                }
                if (validRecoverPoint)
                {
                    return ShipManager.Recover(ship, recover);
                }
            }
            return false;
        }
        public bool Recycle(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
            {
                bool validRecyclePoint = false;
                foreach (XY recyclePoint in teamList[(int)ship.TeamID].BirthPointList)
                {
                    if (GameData.ApproachToInteract(ship.Position, recyclePoint) && ship.Position != recyclePoint)
                    {
                        validRecyclePoint = true;
                        break;
                    }
                }
                if (validRecyclePoint)
                {
                    return shipManager.Recycle(ship);
                }
            }
            return false;
        }
        public bool RepairHome(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return actionManager.RepairHome(ship);
            return false;
        }
        public bool RepairWormhole(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return actionManager.RepairWormhole(ship);
            return false;
        }
        public bool Stop(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return ActionManager.Stop(ship);
            return false;
        }
        public bool Attack(long teamID, long shipID, double angle)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null && ship.IsRemoved == false)
                return attackManager.Attack(ship, angle);
            return false;
        }
        public void ClearAllLists()
        {
            foreach (var keyValuePair in gameMap.GameObjDict)
            {
                if (!GameData.NeedCopy(keyValuePair.Key))
                {
                    gameMap.GameObjDict[GameObjType.Ship].ForEach(delegate (IGameObj ship)
                    {
                        ((Ship)ship).CanMove.SetROri(false);
                    });
                    gameMap.GameObjDict[keyValuePair.Key].Clear();
                }
            }
        }
        public long GetTeamMoney(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].MoneyPool.Money;
        }
        public long GetTeamScore(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].MoneyPool.Score;
        }
        public List<IGameObj> GetGameObj()
        {
            var gameObjList = new List<IGameObj>();
            foreach (var keyValuePair in gameMap.GameObjDict)
            {
                if (GameData.NeedCopy(keyValuePair.Key))
                {
                    var thisList = gameMap.GameObjDict[keyValuePair.Key].ToNewList();
                    if (thisList != null) gameObjList.AddRange(thisList);
                }
            }
            return gameObjList;
        }
        public void AddBirthPoint(long teamID, XY pos)
        {
            if (!gameMap.TeamExists(teamID))
                return;
            if (teamList[(int)teamID].BirthPointList.Contains(pos))
                return;
            teamList[(int)teamID].BirthPointList.Add(pos);
        }
        public void RemoveBirthPoint(long teamID, XY pos)
        {
            if (!gameMap.TeamExists(teamID))
                return;
            if (!teamList[(int)teamID].BirthPointList.Contains(pos))
                return;
            teamList[(int)teamID].BirthPointList.Remove(pos);
        }
        public void AddFactory(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return;
            teamList[(int)teamID].FactoryNum.Add(1);
        }
        public void RemoveFactory(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return;
            teamList[(int)teamID].FactoryNum.Sub(1);
        }
        public Game(MapStruct mapResource, int numOfTeam)
        {
            gameMap = new(mapResource);
            shipManager = new(this, gameMap);
            moduleManager = new();
            actionManager = new(this, gameMap, shipManager);
            attackManager = new(this, gameMap, shipManager);
            teamList = [];
            gameMap.GameObjDict[GameObjType.Home].Cast<GameObj>()?.ForEach(
                delegate (GameObj gameObj)
                {
                    if (gameObj.Type == GameObjType.Home)
                    {
                        teamList.Add(new Base((Home)gameObj));
                        teamList.Last().BirthPointList.Add(gameObj.Position);
                        teamList.Last().AddMoney(GameData.InitialMoney);
                    }
                }
            );
        }
    }
}
