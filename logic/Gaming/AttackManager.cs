using System.Collections.Generic;
using System.Threading;
using GameClass.GameObj;
using GameClass.GameObj.Areas;
using GameClass.GameObj.Bullets;
using GameEngine;
using Preparation.Utility;
using Preparation.Interface;
using Timothy.FrameRateTask;

namespace Gaming
{
    public partial class Game
    {
        private readonly AttackManager attackManager;
        private class AttackManager(Map gameMap, ShipManager shipManager)
        {
            private readonly Map gameMap = gameMap;
            private readonly ShipManager shipManager = shipManager;
            public readonly MoveEngine moveEngine = new(
                    gameMap: gameMap,
                    OnCollision: (obj, collisionObj, moveVec) => MoveEngine.AfterCollision.Destroyed,
                    EndMove: obj => obj.CanMove.SetReturnOri(false)
                );

            public void ProduceBulletNaturally(BulletType bulletType, Ship ship, double angle, XY pos)
            {
                // 子弹如果没有和其他物体碰撞，将会一直向前直到超出人物的attackRange
                if (bulletType == BulletType.Null) return;
                Bullet? bullet = BulletFactory.GetBullet(ship, pos, bulletType);
                if (bullet == null) return;
                gameMap.Add(bullet);
                moveEngine.MoveObj(bullet, (int)(bullet.AttackDistance * 1000 / bullet.MoveSpeed), angle, ++bullet.StateNum);  // 这里时间参数除出来的单位要是ms
            }
            private void BombObj(Bullet bullet, GameObj objBeingShot)
            {
                switch (objBeingShot.Type)
                {
                    case GameObjType.Ship:
                        shipManager.BeAttacked((Ship)objBeingShot, bullet);
                        break;
                    case GameObjType.Construction:
                        ((Construction)objBeingShot).BeAttacked(bullet);
                        break;
                    case GameObjType.Wormhole:
                        ((Wormhole)objBeingShot).BeAttacked(bullet);
                        break;
                    case GameObjType.Home:
                        ((Home)objBeingShot).BeAttacked(bullet);
                        break;
                    default:
                        break;
                }
            }
            public bool TryRemoveBullet(Bullet bullet)
            {
                if (gameMap.Remove(bullet))
                {
                    bullet.CanMove.SetReturnOri(false);
                    if (bullet.BulletBombRange > 0)
                    {
                        BombedBullet bombedBullet = new(bullet);
                        gameMap.Add(bombedBullet);
                        new Thread
                        (() =>
                        {
                            Thread.Sleep(GameData.FrameDuration * 5);
                            gameMap.RemoveJustFromMap(bombedBullet);
                        }
                        )
                        { IsBackground = true }.Start();
                    }
                    return true;
                }
                else return false;
            }
            private void BulletBomb(Bullet bullet, GameObj? objBeingShot)
            {
                if (!TryRemoveBullet(bullet))
                {
                    return;
                }
                if (bullet.BulletBombRange == 0)
                {
                    if (objBeingShot == null)
                    {
                        shipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
                        return;
                    }
                    BombObj(bullet, objBeingShot);
                    shipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
                    return;
                }
                else
                {
                    var beAttackedList = new List<IGameObj>();
                    foreach (var kvp in gameMap.GameObjDict)
                    {
                        if (bullet.CanBeBombed(kvp.Key))
                        {
                            gameMap.GameObjLockDict[kvp.Key].EnterReadLock();
                            try
                            {
                                foreach (var item in gameMap.GameObjDict[kvp.Key])
                                {
                                    if (bullet.CanAttack((GameObj)item))
                                    {
                                        beAttackedList.Add(item);
                                    }
                                }
                            }
                            finally
                            {
                                gameMap.GameObjLockDict[kvp.Key].ExitReadLock();
                            }
                        }
                    }
                    foreach (GameObj beAttackedObj in beAttackedList)
                    {
                        BombObj(bullet, beAttackedObj);
                    }
                    beAttackedList.Clear();
                    shipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
                }
            }
            public bool Attack(Ship ship, double angle)
            {
                if (!ship.Commandable())
                {
                    return false;
                }
                Bullet? bullet = ship.Attack(angle);
                if (bullet != null)
                {
                    gameMap.Add(bullet);
                    moveEngine.MoveObj(bullet, (int)(bullet.AttackDistance * 1000 / bullet.MoveSpeed), angle, ++bullet.StateNum);  // 这里时间参数除出来的单位要是ms
                    if (bullet.CastTime > 0)
                    {
                        long stateNum = ship.SetShipState(RunningStateType.Waiting, ShipStateType.Attacking);
                        if (stateNum == -1)
                        {
                            TryRemoveBullet(bullet);
                            return false;
                        }
                        new Thread
                        (() =>
                            {
                                ship.ThreadNum.WaitOne();
                                if (!ship.StartThread(stateNum, RunningStateType.RunningActively))
                                {
                                    TryRemoveBullet(bullet);
                                    ship.ThreadNum.Release();
                                    return;
                                }
                                new FrameRateTaskExecutor<int>(
                                    loopCondition: () => stateNum == ship.StateNum && gameMap.Timer.IsGaming,
                                    loopToDo: () => { },
                                    timeInterval: GameData.CheckInterval,
                                    finallyReturn: () => 0,
                                    maxTotalDuration: bullet.CastTime
                                ).Start();
                                ship.ThreadNum.Release();
                                if (gameMap.Timer.IsGaming)
                                {
                                    if (!ship.ResetShipState(stateNum))
                                    {
                                        TryRemoveBullet(bullet);
                                    }
                                }
                            }
                        )
                        { IsBackground = true }.Start();
                    }
                }
                if (bullet != null)
                    return true;
                else
                    return false;
            }
        }
    }
}
