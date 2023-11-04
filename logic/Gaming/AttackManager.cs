using System;
using System.Threading;
using System.Collections.Generic;
using GameClass.GameObj;
using GameClass.GameObj.Bullets;
using Preparation.Utility;
using GameEngine;
using Preparation.Interface;
using Timothy.FrameRateTask;

namespace Gaming
{
    public partial class Game
    {
        private readonly AttackManager attackManager;
        private class AttackManager
        {
            readonly Map gameMap;
            public readonly MoveEngine moveEngine;
            readonly ShipManager shipManager;
            public AttackManager(Map gameMap, ShipManager shipManager)
            {
                this.gameMap = gameMap;
                moveEngine = new MoveEngine(
                    gameMap: gameMap,
                    OnCollision: (obj, collisionObj, moveVec) =>
                    {
                        return MoveEngine.AfterCollision.Destroyed;
                    },
                    EndMove: obj =>
                    {
                        obj.CanMove.SetReturnOri(false);
                    }
                );
                this.shipManager = shipManager;
            }
            public void ProduceBulletNaturally(BulletType bulletType, Ship ship, double angle, XY pos)
            {
                // 子弹如果没有和其他物体碰撞，将会一直向前直到超出人物的attackRange
                if (bulletType == BulletType.Null) return;
                Bullet? bullet = BulletFactory.GetBullet(ship, pos, bulletType);
                if (bullet == null) return;
                gameMap.Add(bullet);
                moveEngine.MoveObj(bullet, (int)(bullet.AttackDistance * 1000 / bullet.MoveSpeed), angle, ++bullet.StateNum);  // 这里时间参数除出来的单位要是ms
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
        }
    }
}
