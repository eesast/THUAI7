using GameClass.GameObj;
using GameClass.GameObj.Map;
using GameClass.GameObj.Areas;
using GameClass.GameObj.Bullets;
using GameEngine;
using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Timothy.FrameRateTask;
using Preparation.Utility.Logging;

namespace Gaming
{
    public partial class Game
    {
        private readonly AttackManager attackManager;
        private class AttackManager
        {
            private readonly Game game;
            private readonly Map gameMap;
            private readonly ShipManager shipManager;
            private readonly MoveEngine moveEngine;
            public AttackManager(Game game, Map gameMap, ShipManager shipManager)
            {
                this.game = game;
                this.gameMap = gameMap;
                this.shipManager = shipManager;
                moveEngine = new(
                    gameMap: gameMap,
                    OnCollision: (obj, collisionObj, moveVec) =>
                    {
                        BulletBomb((Bullet)obj, (GameObj)collisionObj);
                        return MoveEngine.AfterCollision.Destroyed;
                    },
                    EndMove: obj =>
                    {
                        AttackManagerLogging.logger.ConsoleLogDebug(
                            LoggingFunctional.AutoLogInfo(obj)
                            + $" end move at {obj.Position} Time: {Environment.TickCount64}");
                        if (obj.CanMove)
                        {
                            BulletBomb((Bullet)obj, null);
                        }
                        obj.CanMove.SetROri(false);
                    },
                    collideWithWormhole: true
                );
                this.game = game;
            }
            public void ProduceBulletNaturally(BulletType bulletType, Ship ship, double angle, XY pos)
            {
                // 子弹如果没有和其他物体碰撞，将会一直向前直到超出人物的attackRange
                if (bulletType == BulletType.Null) return;
                Bullet? bullet = BulletFactory.GetBullet(ship, pos, bulletType);
                if (bullet == null) return;
                AttackManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.BulletLogInfo(bullet)
                    + $" attack in {pos}");
                gameMap.Add(bullet);
                moveEngine.MoveObj(
                    bullet,
                    (int)(bullet.AttackDistance * 1000 / bullet.MoveSpeed),
                    angle,
                    ++bullet.StateNum);  // 这里时间参数除出来的单位要是ms
            }
            private void BombObj(Bullet bullet, GameObj objBeingShot)
            {
                AttackManagerLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.BulletLogInfo(bullet)
                    + " bombed "
                    + LoggingFunctional.AutoLogInfo(objBeingShot));
                switch (objBeingShot.Type)
                {
                    case GameObjType.Ship:
                        if (((Ship)objBeingShot).TeamID.Get() == bullet.Parent!.TeamID.Get())
                        {
                            AttackManagerLogging.logger.ConsoleLogDebug(
                                LoggingFunctional.BulletLogInfo(bullet)
                                + " bombed "
                                + LoggingFunctional.ShipLogInfo((Ship)objBeingShot)
                                + " in the same team!");
                            return;
                        }
                        shipManager.BeAttacked((Ship)objBeingShot, bullet);
                        break;
                    case GameObjType.Construction:
                        var constructionType = ((Construction)objBeingShot).ConstructionType;
                        var flag = ((Construction)objBeingShot).BeAttacked(bullet);
                        if (flag)
                        {
                            ((Construction)objBeingShot).IsActivated.SetROri(false);
                            if (constructionType == ConstructionType.Community)
                            {
                                game.RemoveBirthPoint(
                                    ((Construction)objBeingShot).TeamID,
                                    ((Construction)objBeingShot).Position);
                            }
                            else if (constructionType == ConstructionType.Factory)
                            {
                                game.RemoveFactory(((Construction)objBeingShot).TeamID);
                            }
                        }
                        break;
                    case GameObjType.Wormhole:
                        var previousHP = ((WormholeCell)objBeingShot).Wormhole.HP.GetValue();
                        ((WormholeCell)objBeingShot).Wormhole.BeAttacked(bullet);
                        if (previousHP >= GameData.WormholeHP / 2 && ((WormholeCell)objBeingShot).Wormhole.HP < GameData.WormholeHP / 2)
                        {
                            var shipList = gameMap.ShipInTheList(((WormholeCell)objBeingShot).Wormhole.Cells);
                            if (shipList != null)
                            {
                                foreach (Ship ship in shipList)
                                {
                                    AttackManagerLogging.logger.ConsoleLogDebug(
                                        LoggingFunctional.ShipLogInfo(ship)
                                        + " is destroyed!");
                                    var money = ship.GetCost();
                                    bullet.Parent!.AddMoney(money);
                                    AttackManagerLogging.logger.ConsoleLogDebug(
                                        LoggingFunctional.ShipLogInfo((Ship)bullet.Parent)
                                        + $" get {money} money because of destroying "
                                        + LoggingFunctional.ShipLogInfo(ship));
                                    shipManager.Remove(ship);
                                }
                            }
                        }
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
                    bullet.CanMove.SetROri(false);
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
                if (objBeingShot != null)
                    AttackManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.BulletLogInfo(bullet)
                        + " bombed with "
                        + LoggingFunctional.AutoLogInfo(objBeingShot));
                else
                    AttackManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.BulletLogInfo(bullet)
                        + " bombed without objBeingShot");

                if (!TryRemoveBullet(bullet))
                {
                    return;
                }
                if (bullet.BulletBombRange == 0)
                {
                    if (objBeingShot == null)
                    {
                        ShipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
                        return;
                    }
                    BombObj(bullet, objBeingShot);
                    ShipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
                    return;
                }
                else
                {
                    var beAttackedList = new List<IGameObj>();
                    foreach (var kvp in gameMap.GameObjDict)
                    {
                        if (bullet.CanBeBombed(kvp.Key))
                        {
                            var thisList = gameMap.GameObjDict[kvp.Key].FindAll(gameObj => bullet.CanAttack((GameObj)gameObj));
                            if (thisList != null) beAttackedList.AddRange(thisList);
                        }
                    }
                    foreach (GameObj beAttackedObj in beAttackedList.Cast<GameObj>())
                    {
                        BombObj(bullet, beAttackedObj);
                    }
                    beAttackedList.Clear();
                    ShipManager.BackSwing((Ship)bullet.Parent!, bullet.SwingTime);
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
                    AttackManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.BulletLogInfo(bullet)
                        + $" attack in {bullet.Position}");
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
                {
                    AttackManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + " successfully attacked!");
                    return true;
                }
                else
                {
                    AttackManagerLogging.logger.ConsoleLogDebug(
                        LoggingFunctional.ShipLogInfo(ship)
                        + " failed to attack!");
                    return false;
                }
            }
        }
    }
}
