using GameClass.GameObj.Areas;
using Preparation.Utility;
using System.Collections.Generic;
using System.Threading;

namespace GameClass.GameObj
{
    public class Team
    {
        private static long currentMaxTeamID = 0;
        public static long CurrentMaxTeamID => currentMaxTeamID;
        private readonly long teamID;
        public long TeamID => teamID;
        public const long invalidTeamID = long.MaxValue;
        public const long noneTeamID = long.MinValue;
        private readonly List<Ship> shipList;
        private readonly Dictionary<uint, XY> birthPointList;
        public Dictionary<uint, XY> BirthPointList => birthPointList;
        private Home home;
        public Ship? GetShip(long shipID)
        {
            foreach (Ship ship in shipList)
            {
                if (ship.ShipID == shipID)
                    return ship;
            }
            return null;
        }
        public bool AddShip(Ship ship)
        {
            switch (ship.ShipType)
            {
                case ShipType.CivilShip:
                    if (GetCivilShipNum() >= GameData.MaxCivilShipNum)
                        return false;
                    break;
                case ShipType.WarShip:
                    if (GetWarShipNum() >= GameData.MaxWarShipNum)
                        return false;
                    break;
                case ShipType.FlagShip:
                    if (GetFlagShipNum() >= GameData.MaxFlagShipNum)
                        return false;
                    break;
                default:
                    return false;
            }
            //shipList.Add(ship);
            return true;
        }
        public void AddMoney(long shipID, long add)
        {
            foreach (Ship ship in shipList)
            {
                if (ship.ShipID == shipID)
                {
                    ship.Money.Add(add);
                    ship.Score.Add(add);
                }
            }
        }
        public void SubMoney(long shipID, long sub)
        {
            foreach (Ship ship in shipList)
            {
                if (ship.ShipID == shipID)
                {
                    ship.Money.Sub(sub);
                }
            }
        }
        public void SetHome(Home home)
        {
            this.home = home;
        }
        public int GetShipNum()
        {
            return shipList.Count;
        }
        public int GetCivilShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.CivilShip)
                    num++;
            }
            return num;
        }
        public int GetWarShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.WarShip)
                    num++;
            }
            return num;
        }
        public int GetFlagShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.FlagShip)
                    num++;
            }
            return num;
        }
        public void RemoveShip(Ship ship)
        {
            int i;
            for (i = 0; i < shipList.Count; i++)
            {
                if (shipList[i] == ship)
                    break;
            }
            if (i < shipList.Count)
                shipList.RemoveAt(i);
        }
        public long[] GetShipIDs()
        {
            long[] shipIDs = new long[shipList.Count];
            int i = 0;
            foreach (Ship ship in shipList)
            {
                shipIDs[i++] = ship.ShipID;
            }
            return shipIDs;
        }
        public static bool TeamExists(long teamID)
        {
            return teamID < currentMaxTeamID;
        }
        public void UpdateBirthPoint()
        { }
        public Team(Home home)
        {
            this.teamID = currentMaxTeamID++;
            this.shipList = new List<Ship>(GameData.MaxShipNum);
            this.home = home;
            this.home.TeamID.SetReturnOri(teamID);
        }
    }
}
