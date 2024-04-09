using GameClass.GameObj.Areas;
using Preparation.Interface;
using Preparation.Utility;
using System.Collections.Generic;

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
        public AtomicInt FactoryNum { get; } = new(0);
        public int MoneyAddPerSecond => GameData.ScoreHomePerSecond + FactoryNum * GameData.ScoreFactoryPerSecond;
        public Team(Home home)
        {
            TeamID = new(home.TeamID);
            Home = home;
            ShipPool = new(
                classfier: (ship) => ship.ShipType,
                idleChecker: (ship) => ship.IsRemoved,
                activator: (ship) =>
                {
                    ship.CanMove.SetROri(true);
                    ship.IsRemoved.SetROri(false);
                },
                inactivator: (ship) =>
                {
                    ship.CanMove.SetROri(false);
                    ship.IsRemoved.SetROri(true);
                });
            ShipPool.Initiate(ShipType.CivilShip, 0,
                              () => new(GameData.ShipRadius, ShipType.CivilShip, MoneyPool));
            ShipPool.Initiate(ShipType.WarShip, 0,
                              () => new(GameData.ShipRadius, ShipType.WarShip, MoneyPool));
            ShipPool.Initiate(ShipType.FlagShip, 0,
                              () => new(GameData.ShipRadius, ShipType.FlagShip, MoneyPool));
        }
        public void AddMoney(long add)
        {
            MoneyPool.Money.Add(add);
            MoneyPool.Score.Add(add);
        }
        public void SubMoney(long sub)
        {
            MoneyPool.Money.SubRNow(sub);
        }
    }
}
