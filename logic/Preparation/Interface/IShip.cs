﻿using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IShip : IMovable
    {
        public AtomicLong TeamID { get; }
        public LongInTheVariableRange HP { get; }
        public LongInTheVariableRange Armor { get; }
        public LongInTheVariableRange Shield { get; }
        public ShipType ShipType { get; }
        public ShipStateType ShipState { get; }
        public IntNumUpdateEachCD BulletNum { get; }
        public long SetShipState(RunningStateType running, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
        public bool ResetShipState(long state, RunningStateType running = RunningStateType.Null, ShipStateType value = ShipStateType.Null, IGameObj? obj = null);
    }
}
