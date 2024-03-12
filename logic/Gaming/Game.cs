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
        public struct ShipInitInfo(long teamID, long playerID, XY birthPoint, ShipType shipType)
        {
            public long teamID = teamID;
            public long playerID = playerID;
            public XY birthPoint = birthPoint;
            public ShipType shipType = shipType;
        }
        private readonly List<Team> teamList;
        public List<Team> TeamList => teamList;
        private readonly Map gameMap;
        public Map GameMap => gameMap;
        public long AddShip(ShipInitInfo shipInitInfo)
        {
            if (!gameMap.TeamExists(shipInitInfo.teamID))
            {
                return GameObj.invalidID;
            }
            XY pos = shipInitInfo.birthPoint;
            bool validBirthPoint = false;
            foreach (XY birthPoint in teamList[(int)shipInitInfo.teamID].BirthPointList)
            {
                if (GameData.ApproachToInteract(pos, birthPoint) && pos != birthPoint)
                {
                    validBirthPoint = true;
                    break;
                }
            }
            if (gameMap.GetPlaceType(pos) != PlaceType.Null &&
                gameMap.GetPlaceType(pos) != PlaceType.Shadow)
            {
                validBirthPoint = false;
            }
            if (!validBirthPoint)
            {
                // 如果出生点不合法，就找一个合法的出生点
                XY defaultBirthPoint = teamList[(int)shipInitInfo.teamID].BirthPointList[0];
                Random random = new();
                pos = defaultBirthPoint + new XY((random.Next() & 2) - 1, (random.Next() & 2) - 1);
            }
            Ship? newShip = shipManager.AddShip(pos, shipInitInfo.teamID, shipInitInfo.playerID,
                shipInitInfo.shipType, teamList[(int)shipInitInfo.teamID].MoneyPool);
            if (newShip == null)
            {
                return GameObj.invalidID;
            }
            teamList[(int)shipInitInfo.teamID].AddShip(newShip);
            long subMoney = 0;
            switch (newShip.ShipType)
            {
                case ShipType.CivilShip:
                    subMoney = GameData.CivilShipCost;
                    break;
                case ShipType.WarShip:
                    subMoney = GameData.WarShipCost;
                    break;
                case ShipType.FlagShip:
                    subMoney = GameData.FlagShipCost;
                    break;
            }
            teamList[(int)shipInitInfo.teamID].SubMoney(subMoney);
            return newShip.ShipID;
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
        public bool MoveShip(long shipID, int moveTimeInMilliseconds, double angle)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
            {
                return actionManager.MoveShip(ship, moveTimeInMilliseconds, angle);
            }
            else
            {
                return false;
            }
        }
        public bool Produce(long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return actionManager.Produce(ship);
            return false;
        }
        public bool Construct(long shipID, ConstructionType constructionType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return actionManager.Construct(ship, constructionType);
            return false;
        }
        public bool InstallModule(long shipID, ModuleType moduleType)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return moduleManager.InstallModule(ship, moduleType);
            return false;
        }
        public bool Recover(long shipID, long recover)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
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
        public bool Recycle(long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return shipManager.Recycle(ship);
            return false;
        }
        public bool Repair(long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return actionManager.Repair(ship);
            return false;
        }
        public bool Stop(long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return ActionManager.Stop(ship);
            return false;
        }
        public bool Attack(long shipID, double angle)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
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
                        ((Ship)ship).CanMove.SetReturnOri(false);
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
                    gameObjList.AddRange(gameMap.GameObjDict[keyValuePair.Key].ToNewList());
                }
            }
            return gameObjList;
        }
        public void UpdateBirthPoint()
        {
            gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>().ForEach(
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
                    gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>().ForEach(
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
            gameMap.GameObjDict[GameObjType.Home].Cast<GameObj>().ForEach(
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
