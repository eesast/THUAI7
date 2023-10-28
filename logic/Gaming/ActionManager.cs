using System;
using System.Threading;
using GameClass.GameObj;
using GameEngine;
using Preparation.Utility;
using Protobuf;
using Timothy.FrameRateTask;

namespace Gaming
{
    public partial class Game
    {
        private readonly ActionManager actionManager;
        private class ActionManager
        {
            public readonly MoveEngine moveEngine;
            public bool MoveShip(Ship shipToMove, int moveTimeInMilliseconds, double moveDirection)
            {
                if (moveTimeInMilliseconds < 5)
                {
                    return false;
                }
                long stateNum = shipToMove.SetShipState(RunningStateType.Waiting, ShipStateType.Moving);
                if (stateNum == -1)
                {
                    return false;
                }
                new Thread
                (
                () =>
                {
                    shipToMove.ThreadNum.WaitOne();
                    if (!shipToMove.StartThread(stateNum, RunningStateType.RunningActively))
                    {
                        shipToMove.ThreadNum.Release();
                        return;
                    }
                    moveEngine.MoveObj(shipToMove, moveTimeInMilliseconds, moveDirection, shipToMove.StateNum);
                    Thread.Sleep(moveTimeInMilliseconds);
                    shipToMove.ResetShipState(stateNum);
                }
                )
                { IsBackground = true }.Start();
                return true;
            }
            public static bool Stop(Ship ship)
            {
                lock (ship.ActionLock)
                {
                    if (ship.Commandable())
                    {
                        ship.SetShipState(RunningStateType.Null);
                        return true;
                    }
                }
                return false;
            }
            public bool Produce(Ship ship)
            {
                return false;
            }
            public bool Construct(Ship ship)
            {
                return false;
            }

        }
    }
}
