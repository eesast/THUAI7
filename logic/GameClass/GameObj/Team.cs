using GameClass.GameObj.Areas;
using Preparation.Utility;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;

namespace GameClass.GameObj
{
    public class Team(Home home)
    {
        public long TeamID { get; } = home.TeamID;
        public const long invalidTeamID = long.MaxValue;
        public const long noneTeamID = long.MinValue;
        private readonly List<Ship> shipList = new(GameData.MaxShipNum);
        private readonly List<XY> birthPointList = new();
        public List<XY> BirthPointList => birthPointList;
        private Home home = home;
        public MoneyPool MoneyPool { get; } = new MoneyPool();
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
        public void AddMoney(long add)
        {
            MoneyPool.Money.Add(add);
            MoneyPool.Score.Add(add);
        }
        public void SubMoney(long sub)
        {
            MoneyPool.Money.Sub(sub);
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
    }
}
