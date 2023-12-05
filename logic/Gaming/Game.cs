using System;
using System.Threading;
using System.Collections.Generic;
using Preparation.Utility;
using Preparation.Interface;
using GameClass.GameObj;
using GameClass.GameObj.Areas;

namespace Gaming
{
    public partial class Game
    {
        public struct ShipInitInfo
        {
            public long teamID;
            public long playerID;
            public uint birthPoint;
            public ShipType shipType;
            public ShipInitInfo(long teamID, long playerID, uint birthPoint, ShipType shipType)
            {
                this.teamID = teamID;
                this.playerID = playerID;
                this.birthPoint = birthPoint;
                this.shipType = shipType;
            }
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
        public Game(uint[,] mapResource, int numOfTeam)
        {
            gameMap = new Map(mapResource);
            teamList = new List<Team>();
            foreach (GameObj gameObj in gameMap.GameObjDict[GameObjType.Home])
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
