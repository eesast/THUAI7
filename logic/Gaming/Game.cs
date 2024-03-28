using GameClass.GameObj;
using GameClass.GameObj.Areas;
using GameClass.MapGenerator;
using Preparation.Interface;
using Preparation.Utility;
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
        private readonly List<Team> teamList;
        public List<Team> TeamList => teamList;
        private readonly Map gameMap;
        public Map GameMap => gameMap;
        public long AddPlayer(PlayerInitInfo playerInitInfo)
        {
            if (!gameMap.TeamExists(playerInitInfo.teamID))
            {
                return GameObj.invalidID;
            }
            if (playerInitInfo.shipType != ShipType.Null)
            {
                // Add a ship
                Ship? newShip = shipManager.AddShip(playerInitInfo.teamID, playerInitInfo.playerID,
                    playerInitInfo.shipType, teamList[(int)playerInitInfo.teamID].MoneyPool);
                if (newShip == null)
                {
                    return GameObj.invalidID;
                }
                //teamList[(int)playerInitInfo.teamID].AddShip(newShip);
                return newShip.PlayerID;
            }
            else
            {
                // Add a home
                return playerInitInfo.playerID;
            }
        }
        public bool ActivateShip(long teamID, long playerID, ShipType shipType, int birthPointIndex = 0)
        {
            Ship? ship = teamList[(int)teamID].ShipPool.GetObj(shipType);
            if (ship == null)
                return false;
            else if (ship.IsRemoved == false)
                return false;
            if (birthPointIndex < 0)
                birthPointIndex = 0;
            if (birthPointIndex >= teamList[(int)teamID].BirthPointList.Count)
                birthPointIndex = teamList[(int)teamID].BirthPointList.Count - 1;
            XY pos = teamList[(int)teamID].BirthPointList[birthPointIndex];
            Random random = new();
            pos += new XY(((random.Next() & 2) - 1) * 1000, ((random.Next() & 2) - 1) * 1000);
            return shipManager.ActivateShip(ship, pos);
        }
        public bool StartGame(int milliSeconds)
        {
            if (gameMap.Timer.IsGaming)
                return false;
            // 开始游戏
            new Thread
            (
                () =>
                {
                    if (!gameMap.Timer.StartGame(milliSeconds))
                        return;
                    EndGame();  // 游戏结束时要做的事
                }
            )
            { IsBackground = true }.Start();
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
            if (ship != null)
            {
                return actionManager.MoveShip(ship, moveTimeInMilliseconds, angle);
            }
            else
            {
                return false;
            }
        }
        public bool Produce(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return actionManager.Produce(ship);
            return false;
        }
        public bool Construct(long teamID, long shipID, ConstructionType constructionType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
            {
                var flag = actionManager.Construct(ship, constructionType);
                if (constructionType == ConstructionType.Community && flag)
                {
                    UpdateBirthPoint();
                }
                return flag;
            }
            return false;
        }
        public bool InstallModule(long teamID, long shipID, ModuleType moduleType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return moduleManager.InstallModule(ship, moduleType);
            return false;
        }
        public bool Recover(long teamID, long shipID, long recover)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
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
                    return shipManager.Recover(ship, recover);
                }
            }
            return false;
        }
        public bool Recycle(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return shipManager.Recycle(ship);
            return false;
        }
        public bool Repair(long teamID, long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInPlayerID(teamID, shipID);
            if (ship != null)
                return actionManager.Repair(ship);
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
            if (ship != null)
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
        public void UpdateBirthPoint()
        {
            gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>()?.ForEach(
                delegate (Construction construction)
                {
                    if (construction.ConstructionType == ConstructionType.Community)
                    {
                        bool exist = false;
                        foreach (XY birthPoint in teamList[(int)construction.TeamID].BirthPointList)
                        {
                            if (construction.Position == birthPoint)
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (!exist)
                        {
                            teamList[(int)construction.TeamID].BirthPointList.Add(construction.Position);
                        }
                    }
                }
            );
            foreach (Team team in teamList)
            {
                foreach (XY birthPoint in team.BirthPointList)
                {
                    gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>()?.ForEach(
                        delegate (Construction construction)
                    {
                        if (construction.Position == birthPoint)
                        {
                            if (construction.ConstructionType != ConstructionType.Community || construction.TeamID != team.TeamID)
                            {
                                team.BirthPointList.Remove(birthPoint);
                            }
                        }
                    }
                    );
                }
            }
        }
        public Game(MapStruct mapResource, int numOfTeam)
        {
            gameMap = new(mapResource);
            shipManager = new(gameMap);
            moduleManager = new();
            actionManager = new(gameMap, shipManager);
            attackManager = new(gameMap, shipManager);
            teamList = [];
            gameMap.GameObjDict[GameObjType.Home].Cast<GameObj>()?.ForEach(
                delegate (GameObj gameObj)
                {
                    if (gameObj.Type == GameObjType.Home)
                    {
                        teamList.Add(new Team((Home)gameObj));
                        teamList.Last().BirthPointList.Add(gameObj.Position);
                        teamList.Last().AddMoney(GameData.InitialMoney);
                    }
                    /*         if (teamList.Count == numOfTeam)
                             {
                                 break;
                             }*/
                }
                );

        }
    }
}
