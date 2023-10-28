using System;
using System.Threading;
using System.Collections.Generic;
using Preparation.Utility;
using Preparation.Interface;
using GameClass.GameObj;

namespace Gaming
{
    public partial class Game
    {
        public struct ShipInitInfo
        {
            public long teamID;
            public long playerID;
            public ShipInitInfo(long teamID, long playerID)
            {
                this.teamID = teamID;
                this.playerID = playerID;
            }
        }
        private readonly List<Team> teamList;
        public List<Team> TeamList => teamList;
        private readonly Map gameMap;
        public Map GameMap => gameMap;
        public Game(uint[,] mapResource, int numOfTeam)
        {
            gameMap = new Map(mapResource);
            teamList = new List<Team>();
            for (int i = 0; i < numOfTeam; i++)
            {
                teamList.Add(new Team());
            }
        }
    }
}
