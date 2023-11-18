using System;
using System.Threading;
using GameClass.GameObj;
using GameClass.GameObj.Areas;
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
            private readonly Map gameMap;
            private readonly ShipManager shipManager;
            public readonly MoveEngine moveEngine;
            public ActionManager(Map gameMap, ShipManager shipManager)
            {
                this.gameMap = gameMap;
                this.shipManager = shipManager;
                this.moveEngine = new MoveEngine(
                    gameMap: gameMap,
                    OnCollision: (obj, collisionObj, moveVec) =>
                    {
                        Ship ship = (Ship)obj;
                        switch (collisionObj.Type)
                        {
                            case GameObjType.Bullet:
                                if (((Bullet)collisionObj).Parent != ship)
                                {
                                    // TODO
                                    gameMap.Remove((GameObj)collisionObj);
                                }
                                break;
                            default:
                                break;
                        }
                        return MoveEngine.AfterCollision.MoveMax;
                    },
                    EndMove: obj =>
                    {
                        obj.ThreadNum.Release();
                    }
                );
                this.shipManager = shipManager;
            }
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
                Resource? resource = (Resource?)gameMap.OneForInteract(ship.Position, GameObjType.Resource);
                if (resource == null)
                {
                    return false;
                }
                if (resource.HP == 0)
                {
                    return false;
                }
                long stateNum = ship.SetShipState(RunningStateType.Waiting, ShipStateType.Producing);
                if (stateNum == -1)
                {
                    return false;
                }
                new Thread
                (
                    () =>
                    {
                        ship.ThreadNum.WaitOne();
                        if (!ship.StartThread(stateNum, RunningStateType.RunningActively))
                        {
                            ship.ThreadNum.Release();
                            return;
                        }
                        resource.AddProduceNum();
                        Thread.Sleep(GameData.CheckInterval);
                        new FrameRateTaskExecutor<int>
                        (
                            loopCondition: () => stateNum == ship.StateNum && gameMap.Timer.IsGaming,
                            loopToDo: () =>
                            {
                                if (resource.HP == 0)
                                {
                                    ship.ResetShipState(stateNum);
                                    return false;
                                }
                                return true;
                            },
                            timeInterval: GameData.CheckInterval,
                            finallyReturn: () => 0
                        ).Start();
                        ship.ThreadNum.Release();
                        resource.SubProduceNum();
                    }
                )
                { IsBackground = true }.Start();
                return false;
            }
            public bool Construct(Ship ship)
            {
                return false;
            }

        }
    }
}
