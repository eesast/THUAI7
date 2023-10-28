using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IShip : IMovable
    {
        public AtomicLong TeamID { get; }
        public LongWithVariableRange HP { get; }
        public LongWithVariableRange Armor { get; }
        public LongWithVariableRange Shield { get; }
        public ShipType ShipType { get; }
        public ShipStateType ShipState { get; }
        public IntNumUpdateByCD BulletNum { get; }
        public long SetShipState(RunningStateType running, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
        public bool ResetShipState(long state, RunningStateType running = RunningStateType.Null, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
    }
}
