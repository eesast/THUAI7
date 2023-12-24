using GameClass.GameObj;
using GameClass.GameObj.Areas;
using Preparation.Interface;
using Preparation.Utility;
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
            // 由于BirthPoint实质上是可变且每支队伍不同的，所以暂时把它放到Team里？
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
            if (gameMap.ProtoGameMap[pos.x, pos.y] != (uint)PlaceType.Null &&
                gameMap.ProtoGameMap[pos.x, pos.y] != (uint)PlaceType.Shadow)
            {
                validBirthPoint = false;
            }
            if (!validBirthPoint)
            {
                return GameObj.invalidID;
            }
            Ship? newShip = shipManager.AddShip(pos, shipInitInfo.teamID, shipInitInfo.playerID, shipInitInfo.shipType);
            if (newShip == null)
            {
                return GameObj.invalidID;
            }
            teamList[(int)shipInitInfo.teamID].AddShip(newShip);
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
        public bool Recycle(long shipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(shipID);
            if (ship != null)
                return actionManager.Recycle(ship);
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
                    gameMap.GameObjLockDict[keyValuePair.Key].EnterWriteLock();
                    try
                    {
                        if (keyValuePair.Key == GameObjType.Ship)
                        {
                            foreach (Ship ship in gameMap.GameObjDict[GameObjType.Ship].Cast<Ship>())
                            {
                                ship.CanMove.SetReturnOri(false);
                            }
                        }
                        gameMap.GameObjDict[keyValuePair.Key].Clear();
                    }
                    finally
                    {
                        gameMap.GameObjLockDict[keyValuePair.Key].ExitWriteLock();
                    }
                }
            }
        }
        public long GetTeamMoney(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].Money;
        }
        public long GetTeamScore(long teamID)
        {
            if (!gameMap.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].Score;
        }
        public List<IGameObj> GetGameObj()
        {
            var gameObjList = new List<IGameObj>();
            foreach (var keyValuePair in gameMap.GameObjDict)
            {
                if (GameData.NeedCopy(keyValuePair.Key))
                {
                    gameMap.GameObjLockDict[keyValuePair.Key].EnterReadLock();
                    try
                    {
                        gameObjList.AddRange(gameMap.GameObjDict[keyValuePair.Key]);
                    }
                    finally
                    {
                        gameMap.GameObjLockDict[keyValuePair.Key].ExitReadLock();
                    }
                }
            }
            return gameObjList;
        }
        public void UpdateBirthPoint()
        {
            foreach (Construction construction in gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>())
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
            foreach (Team team in teamList)
            {
                foreach (XY birthPoint in team.BirthPointList)
                {
                    foreach (Construction construction in gameMap.GameObjDict[GameObjType.Construction].Cast<Construction>())
                    {
                        if (construction.Position == birthPoint)
                        {
                            if (construction.ConstructionType != ConstructionType.Community || construction.TeamID != team.TeamID)
                            {
                                team.BirthPointList.Remove(birthPoint);
                            }
                        }
                    }
                }
            }
        }
        public Game(uint[,] mapResource, int numOfTeam)
        {
            gameMap = new(mapResource);
            shipManager = new(gameMap);
            moduleManager = new();
            actionManager = new(gameMap, shipManager);
            attackManager = new(gameMap, shipManager);
            teamList = [];
            foreach (GameObj gameObj in gameMap.GameObjDict[GameObjType.Home].Cast<GameObj>())
            {
                if (gameObj.Type == GameObjType.Home)
                {
                    teamList.Add(new Team((Home)gameObj));
                    teamList.Last().BirthPointList.Add(gameObj.Position);
                }
                if (teamList.Count == numOfTeam)
                {
                    break;
                }
            }
        }
    }
}
