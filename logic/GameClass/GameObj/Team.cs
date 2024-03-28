using GameClass.GameObj.Areas;
using Preparation.Utility;
using System.Collections.Generic;
using Preparation.Interface;

namespace GameClass.GameObj
{
    public class Team : IPlayer
    {
        public AtomicLong TeamID { get; }
        public AtomicLong PlayerID { get; } = new(0);
        public const long invalidTeamID = long.MaxValue;
        public const long noneTeamID = long.MinValue;
        private readonly List<XY> birthPointList = [];
        public ObjPool<Ship, ShipType> ShipPool { get; }
        public List<XY> BirthPointList => birthPointList;
        public Home Home { get; set; }
        public MoneyPool MoneyPool { get; } = new();

        public Team(Home home)
        {
            TeamID = new(home.TeamID);
            Home = home;
            ShipPool = new(
                classfier: (ship) => ship.ShipType,
                idleChecker: (ship) => ship.IsRemoved,
                activator: (ship) =>
                {
                    ship.CanMove.SetReturnOri(true);
                    ship.IsRemoved.SetReturnOri(false);
                },
                inactivator: (ship) =>
                {
                    ship.CanMove.SetReturnOri(false);
                    ship.IsRemoved.SetReturnOri(true);
                });
            ShipPool.Initiate(ShipType.CivilShip, GameData.MaxCivilShipNum,
                              () => new(GameData.ShipRadius, ShipType.CivilShip, MoneyPool));
            ShipPool.Initiate(ShipType.WarShip, GameData.MaxWarShipNum,
                              () => new(GameData.ShipRadius, ShipType.WarShip, MoneyPool));
            ShipPool.Initiate(ShipType.FlagShip, GameData.MaxFlagShipNum,
                              () => new(GameData.ShipRadius, ShipType.FlagShip, MoneyPool));
        }

        /*public bool AddShip(Ship ship)
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
        }*/

        public void AddMoney(long add)
        {
            MoneyPool.Money.Add(add);
            MoneyPool.Score.Add(add);
        }
        public void SubMoney(long sub)
        {
            MoneyPool.Money.Sub(sub);
        }
    }
}
