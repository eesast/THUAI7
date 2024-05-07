using Preparation.Utility;
using Preparation.Utility.Value.SafeValue.LockedValue;
using Preparation.Utility.Value.SafeValue.TimeBased;

namespace Preparation.Interface
{
    public interface IShip : IMovable, IPlayer
    {
        public InVariableRange<long> HP { get; }
        public InVariableRange<long> Armor { get; }
        public InVariableRange<long> Shield { get; }
        public ShipType ShipType { get; }
        public ShipStateType ShipState { get; }
        public IntNumUpdateEachCD BulletNum { get; }
        public long AddMoney(long add);
        public long SubMoney(long sub);
        public long SetShipState(RunningStateType running, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
        public bool ResetShipState(long state, RunningStateType running = RunningStateType.Null, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
    }
}
