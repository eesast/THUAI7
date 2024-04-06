using GameClass.GameObj;
using GameClass.GameObj.Areas;
using GameEngine;
using Preparation.Utility;
using System;
using System.Threading;
using Timothy.FrameRateTask;

namespace Gaming
{
    public partial class Game
    {
        private readonly ActionManager actionManager;
        private class ActionManager(Game game, Map gameMap, ShipManager shipManager)
        {
            private readonly Game game = game;
            private readonly Map gameMap = gameMap;
            private readonly ShipManager shipManager = shipManager;
            public readonly MoveEngine moveEngine = new(
                    gameMap: gameMap,
                    OnCollision: (obj, collisionObj, moveVec) =>
                    {
                        Ship ship = (Ship)obj;
                        switch (collisionObj.Type)
                        {
                            case GameObjType.Bullet:
                                if (((Bullet)collisionObj).Parent != ship)
                                {
                                    ShipManager.BeStunned(ship, 1000);
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
                                if (!resource.Produce(ship.ProduceSpeed * GameData.CheckInterval, ship))
                                {
                                    ship.ThreadNum.Release();
                                    ship.ResetShipState(stateNum);
                                    return false;
                                }
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
            public bool Construct(Ship ship, ConstructionType constructionType)
            {
                Construction? construction = (Construction?)gameMap.OneForInteract(ship.Position, GameObjType.Construction);
                if (construction == null)
                {
                    return false;
                }
                if (construction.HP == construction.HP.GetMaxV())
                {
                    return false;
                }
                long stateNum = ship.SetShipState(RunningStateType.Waiting, ShipStateType.Constructing);
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
                        construction.AddConstructNum();
                        Thread.Sleep(GameData.CheckInterval);
                        new FrameRateTaskExecutor<int>
                        (
                            loopCondition: () => stateNum == ship.StateNum && gameMap.Timer.IsGaming,
                            loopToDo: () =>
                            {
                                if (!construction.Construct(ship.ConstructSpeed * GameData.CheckInterval, constructionType, ship))
                                {
                                    ship.ThreadNum.Release();
                                    ship.ResetShipState(stateNum);
                                    return false;
                                }
                                if (construction.HP == construction.HP.GetMaxV())
                                {
                                    ship.ResetShipState(stateNum);
                                    if (!construction.IsActivated)
                                    {
                                        switch (construction.ConstructionType)
                                        {
                                            case ConstructionType.Factory:
                                                game.AddFactory(construction.TeamID);
                                                break;
                                            case ConstructionType.Community:
                                                game.AddBirthPoint(construction.TeamID, construction.Position);
                                                break;
                                            case ConstructionType.Fort:
                                                new Thread
                                                (
                                                    () =>
                                                    {
                                                        Thread.Sleep(GameData.CheckInterval);
                                                        new FrameRateTaskExecutor<int>
                                                        (
                                                            loopCondition: () =>
                                                                gameMap.Timer.IsGaming && construction.HP >
                                                                construction.HP.GetMaxV() * 0.5,
                                                            loopToDo: () =>
                                                            {
                                                                var ships = gameMap.ShipInTheRange(
                                                                    construction.Position, GameData.FortRange);
                                                                var random = new Random();
                                                                if (ships.Count > 0)
                                                                {
                                                                    var ship = ships[random.Next(ships.Count)];
                                                                    shipManager.BeAttacked(ship, GameData.FortDamage,
                                                                        construction.TeamID);
                                                                }
                                                                return true;
                                                            },
                                                            timeInterval: GameData.CheckInterval,
                                                            finallyReturn: () => 0
                                                        ).Start();
                                                    }
                                                ) { IsBackground = true }.Start();
                                                break;
                                        }
                                        construction.IsActivated.Set(true);
                                    }
                                    return false;
                                }
                                return true;
                            },
                            timeInterval: GameData.CheckInterval,
                            finallyReturn: () => 0
                        ).Start();
                        ship.ThreadNum.Release();
                        construction.SubConstructNum();
                    }
                )
                { IsBackground = true }.Start();
                return false;
            }
            public bool Repair(Ship ship)
            {
                Wormhole? wormhole = (Wormhole?)gameMap.OneForInteract(ship.Position, GameObjType.Wormhole);
                if (wormhole == null)
                {
                    return false;
                }
                if (wormhole.HP == wormhole.HP.GetMaxV())
                {
                    return false;
                }
                long stateNum = ship.SetShipState(RunningStateType.Waiting, ShipStateType.Constructing);
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
                        wormhole.AddRepairNum();
                        Thread.Sleep(GameData.CheckInterval);
                        new FrameRateTaskExecutor<int>
                        (
                            loopCondition: () => stateNum == ship.StateNum && gameMap.Timer.IsGaming,
                            loopToDo: () =>
                            {
                                if (!wormhole.Repair(ship.ConstructSpeed * GameData.CheckInterval, ship))
                                {
                                    ship.ThreadNum.Release();
                                    ship.ResetShipState(stateNum);
                                    return false;
                                }
                                if (wormhole.HP == wormhole.HP.GetMaxV())
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
                        wormhole.SubRepairNum();
                    }
                )
                { IsBackground = true }.Start();
                return false;
            }
            public bool AddMoneyNaturally(Team team)
            {
                new Thread
                (
                    () =>
                    {
                        Thread.Sleep(GameData.CheckInterval);
                        new FrameRateTaskExecutor<int>
                        (
                            loopCondition: () => gameMap.Timer.IsGaming,
                            loopToDo: () =>
                            {
                                team.AddMoney(team.MoneyAddPerSecond);
                                return true;
                            },
                            timeInterval: GameData.CheckInterval,
                            finallyReturn: () => 0
                        ).Start();
                    }
                )
                { IsBackground = true }.Start();
                return false;
            }
        }
    }
}
