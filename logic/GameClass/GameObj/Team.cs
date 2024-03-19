using GameClass.GameObj.Areas;
using Preparation.Utility;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;
using Preparation.Interface;

namespace GameClass.GameObj
{
    public class Team(Home home) : IPlayer
    {
        public AtomicLong TeamID { get; } = new(home.TeamID);
        public AtomicLong PlayerID { get; } = new(0);
        public const long invalidTeamID = long.MaxValue;
        public const long noneTeamID = long.MinValue;
        private readonly List<Ship> shipList = new(GameData.MaxShipNum);
        private readonly List<XY> birthPointList = [];
        public List<XY> BirthPointList => birthPointList;
        private Home home = home;
        public MoneyPool MoneyPool { get; } = new MoneyPool();
        public Ship? GetShip(long shipID)
        {
            foreach (Ship ship in shipList)
            {
                if (ship.PlayerID == shipID)
                    return ship;
            }
            return null;
        }
        public Ship? GetNewShip(ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    var civilIndex = GetNewCivilShipIndex();
                    return civilIndex == -1 ? null : shipList[civilIndex];
                case ShipType.WarShip:
                    var warIndex = GetNewWarShipIndex();
                    return warIndex == -1 ? null : shipList[warIndex];
                case ShipType.FlagShip:
                    var flagIndex = GetNewFlagShipIndex();
                    return flagIndex == -1 ? null : shipList[flagIndex];
                default:
                    return null;
            }
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
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (!ship.IsRemoved)
                    num++;
            }
            return num;
        }
        public int GetCivilShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.CivilShip && !ship.IsRemoved)
                    num++;
            }
            return num;
        }
        public int GetNewCivilShipIndex()
        {
            var from = 1;
            var to = 1 + GameData.MaxCivilShipNum;
            for (int index = from; index < to; index++)
            {
                if (shipList[index].IsRemoved)
                    return index;
            }
            return -1;
        }
        public int GetWarShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.WarShip && !ship.IsRemoved)
                    num++;
            }
            return num;
        }
        public int GetNewWarShipIndex()
        {
            var from = 1 + GameData.MaxCivilShipNum;
            var to = 1 + GameData.MaxCivilShipNum + GameData.MaxWarShipNum;
            for (int index = from; index < to; index++)
            {
                if (shipList[index].IsRemoved)
                    return index;
            }
            return -1;
        }
        public int GetFlagShipNum()
        {
            int num = 0;
            foreach (Ship ship in shipList)
            {
                if (ship.ShipType == ShipType.FlagShip && !ship.IsRemoved)
                    num++;
            }
            return num;
        }
        public int GetNewFlagShipIndex()
        {
            var from = 1 + GameData.MaxCivilShipNum + GameData.MaxWarShipNum;
            var to = 1 + GameData.MaxCivilShipNum + GameData.MaxWarShipNum + GameData.MaxFlagShipNum;
            for (int index = from; index < to; index++)
            {
                if (shipList[index].IsRemoved)
                    return index;
            }
            return -1;
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
                shipIDs[i++] = ship.PlayerID;
            }
            return shipIDs;
        }
    }
}
