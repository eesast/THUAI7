using GameClass.GameObj;
using GameClass.GameObj.Areas;
using Preparation.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gaming
{
    public partial class Game
    {
        public struct ShipInitInfo(long teamID, long playerID, uint birthPoint, ShipType shipType)
        {
            public long teamID = teamID;
            public long playerID = playerID;
            public uint birthPoint = birthPoint;
            public ShipType shipType = shipType;
        }
        private readonly List<Team> teamList;
        public List<Team> TeamList => teamList;
        private readonly Map gameMap;
        public Map GameMap => gameMap;
        public long AddShip(ShipInitInfo shipInitInfo)
        {
            if (!Team.TeamExists(shipInitInfo.teamID))
            {
                return GameObj.invalidID;
            }
            // 由于BirthPoint实质上是可变且每支队伍不同的，所以暂时把它放到Team里？
            XY pos = teamList[(int)shipInitInfo.teamID].BirthPointList[shipInitInfo.birthPoint];
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
        public bool Repair(long ShipID)
        {
            if (!gameMap.Timer.IsGaming)
                return false;
            Ship? ship = gameMap.FindShipInShipID(ShipID);
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
        public long GetTeamMoney(long teamID)
        {
            if (!Team.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].Money;
        }
        public long GetTeamScore(long teamID)
        {
            if (!Team.TeamExists(teamID))
                return -1;
            return teamList[(int)teamID].Score;
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
                }
                if (teamList.Count == numOfTeam)
                {
                    break;
                }
            }
        }
    }
}
