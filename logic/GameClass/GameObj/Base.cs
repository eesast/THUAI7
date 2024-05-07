using GameClass.GameObj.Areas;
using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue;
using Preparation.Utility.Value.SafeValue.Atomic;
using System.Collections.Generic;

namespace GameClass.GameObj
{
    public class Base : IPlayer
    {
        public AtomicLong TeamID { get; }
        public AtomicLong PlayerID { get; } = new(0);
        public const long invalidTeamID = long.MaxValue;
        public const long noneTeamID = long.MinValue;
        public ObjPool<Ship, ShipType> ShipPool { get; }
        private readonly List<XY> birthPointList = [];
        public List<XY> BirthPointList => birthPointList;
        public Home Home { get; set; }
        public MoneyPool MoneyPool { get; } = new();
        public AtomicInt FactoryNum { get; } = new(0);
        public int MoneyAddPerSecond => GameData.ScoreHomePerSecond + FactoryNum * GameData.ScoreFactoryPerSecond;
        public Base(Home home)
        {
            TeamID = new(home.TeamID);
            Home = home;
            ShipPool = new(
                classfier: (ship) => ship.ShipType,
                idleChecker: (ship) => ship.IsRemoved,
                tryActivator: (ship) =>
                {
                    if (ship.IsRemoved.TrySet(false))
                    {
                        ship.CanMove.SetROri(true);
                        return true;
                    }
                    return false;
                },
                inactivator: (ship) =>
                {
                    ship.CanMove.SetROri(false);
                    ship.IsRemoved.SetROri(true);
                });
            // 池初始化，但是由于服务器采用逐个添加船只的方式，因此这里不进行任何行为
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
        }
        public void SubMoney(long sub)
        {
            MoneyPool.Money.SubRNow(sub);
        }
    }
}
