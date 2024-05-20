using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Logging;
using Preparation.Utility.Value;
using System;
using System.Threading;
using Timothy.FrameRateTask;

namespace GameEngine
{
    /// <summary>
    /// Constrctor
    /// </summary>
    /// <param name="gameMap">游戏地图</param>
    /// <param name="OnCollision">
    /// <para>发生碰撞时要做的事情</para>
    /// <para>- 第一个参数为移动的物体</para>
    /// <para>- 第二个参数为撞到的物体</para>
    /// <para>- 第三个参数为移动的位移向量</para>
    /// <para>返回值见AfterCollision的定义</para>
    /// </param>
    /// <param name="EndMove">结束碰撞时要做的事情</param>
    public class MoveEngine(
        IMap gameMap,
        Func<IMovable, IGameObj, XY, MoveEngine.AfterCollision> OnCollision,
        Action<IMovable> EndMove,
        bool collideWithWormhole = false
        )
    {
        /// <summary>
        /// 碰撞结束后要做的事情
        /// </summary>
        public enum AfterCollision
        {
            ContinueCheck = 0,  // 碰撞后继续检查其他碰撞,暂时没用
            MoveMax = 1,        // 行走最远距离
            Destroyed = 2       // 物体已经毁坏
        }

        private readonly IMyTimer gameTimer = gameMap.Timer;
        private readonly Action<IMovable> EndMove = EndMove;

        public IGameObj? CheckCollision(IMovable obj, XY Pos)
        {
            return collisionChecker.CheckCollision(obj, Pos);
        }

        private readonly CollisionChecker collisionChecker = new(gameMap);
        private readonly Func<IMovable, IGameObj, XY, AfterCollision> OnCollision = OnCollision;
        private bool collideWithWormhole = collideWithWormhole;

        /// <summary>
        /// 在无碰撞的前提下行走最远的距离
        /// </summary>
        /// <param name="obj">移动物体，默认obj.Rigid为true</param>
        /// <param name="moveVec">移动的位移向量</param>
        private bool MoveMax(IMovable obj, XY moveVec, long stateNum)
        {
            /*由于四周是墙，所以人物永远不可能与越界方块碰撞*/
            double maxLen = collisionChecker.FindMax(obj, moveVec);
            maxLen = Math.Min(maxLen, obj.MoveSpeed / GameData.NumOfStepPerSecond);
            //if (maxLen == 0)
            //{
            //    // 尝试滑动
            //    IGameObj? collisionObj = collisionChecker.CheckCollisionWhenMoving(obj, moveVec);
            //    XY slideVec = new(0, 0);
            //    switch (collisionObj?.Shape)
            //    {
            //        case ShapeType.Circle:
            //            XY connectVec = collisionObj.Position - obj.Position;
            //            slideVec = new XY(
            //                 connectVec.Perpendicular(),
            //                 moveVec.Length() * Math.Cos(connectVec.Angle() - moveVec.Angle())
            //            );
            //            break;
            //        case ShapeType.Square:
            //            if (Math.Abs(collisionObj.Position.x - obj.Position.x) == collisionObj.Radius + obj.Radius)
            //            {
            //                slideVec = new XY(0, moveVec.y * Math.Sign(collisionObj.Position.y - obj.Position.y));
            //            }
            //            else if (Math.Abs(collisionObj.Position.y - obj.Position.y) == collisionObj.Radius + obj.Radius)
            //            {
            //                slideVec = new XY(moveVec.x * Math.Sign(collisionObj.Position.x - obj.Position.x), 0);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //    double slideLen = collisionChecker.FindMax(obj, slideVec);
            //    slideLen = Math.Min(slideLen, slideVec.Length());
            //    slideLen = Math.Min(slideLen, obj.MoveSpeed / GameData.NumOfStepPerSecond);
            //    return (obj.MovingSetPos(new XY(slideVec, slideLen), stateNum)) >= 0;
            //}
            return (obj.MovingSetPos(new XY(moveVec, maxLen), stateNum)) >= 0;
        }

        private bool LoopDo(IMovable obj, double direction, ref double deltaLen, long stateNum)
        {
            double moveVecLength = obj.MoveSpeed / GameData.NumOfStepPerSecond;
            XY res = new(direction, moveVecLength);

            // 越界情况处理：如果越界，则与越界方块碰撞
            bool flag;  // 循环标志
            do
            {
                flag = false;
                IGameObj? collisionObj = collisionChecker.CheckCollisionWhenMoving(obj, res, collideWithWormhole);
                if (collisionObj == null)
                    break;

                switch (OnCollision(obj, collisionObj, res))
                {
                    case AfterCollision.ContinueCheck:
                        flag = true;
                        break;
                    case AfterCollision.Destroyed:
                        GameEngineLogging.logger.ConsoleLogDebug(
                            Logger.ObjInfo(obj)
                            + " collide with "
                            + Logger.ObjInfo(collisionObj)
                            + " and has been removed from the game");
                        return false;
                    case AfterCollision.MoveMax:
                        if (!MoveMax(obj, res, stateNum)) return false;
                        moveVecLength = 0;
                        res = new XY(direction, moveVecLength);
                        break;
                }
            } while (flag);

            long moveL = obj.MovingSetPos(res, stateNum);
            if (moveL == -1) return false;
            deltaLen = deltaLen + moveVecLength - Math.Sqrt(moveL);
            return true;
        }

        public void MoveObj(IMovable obj, int moveTime, double direction, long stateNum)
        {
            GameEngineLogging.logger.ConsoleLogDebug(
                Logger.ObjInfo(obj)
                + $" position {obj.Position}, start moving in direction {direction}");
            if (!gameTimer.IsGaming) return;
            lock (obj.ActionLock)
            {
                if (!obj.IsAvailableForMove) { EndMove(obj); return; }
                obj.IsMoving.SetROri(true);
            }

            new Thread
            (
                () =>
                {
                    double moveVecLength = 0.0;
                    XY res = new(direction, moveVecLength);
                    double deltaLen = 0.0;  // 转向，并用deltaLen存储行走的误差
                    IGameObj? collisionObj = null;
                    bool isEnded = false;

                    bool flag;  // 循环标志
                    do
                    {
                        flag = false;
                        collisionObj = collisionChecker.CheckCollision(obj, obj.Position);
                        if (collisionObj == null)
                            break;

                        switch (OnCollision(obj, collisionObj, res))
                        {
                            case AfterCollision.ContinueCheck:
                                flag = true;
                                break;
                            case AfterCollision.Destroyed:
                                GameEngineLogging.logger.ConsoleLogDebug(
                                    Logger.ObjInfo(obj)
                                    + " collide with "
                                    + Logger.ObjInfo(collisionObj)
                                    + " and has been removed from the game");
                                isEnded = true;
                                break;
                            case AfterCollision.MoveMax:
                                break;
                        }
                    } while (flag);

                    if (isEnded)
                    {
                        obj.IsMoving.SetROri(false);
                        EndMove(obj);
                        return;
                    }
                    else
                    {
                        if (moveTime >= GameData.NumOfPosGridPerCell / GameData.NumOfStepPerSecond)
                        {
                            Thread.Sleep(GameData.NumOfPosGridPerCell / GameData.NumOfStepPerSecond);
                            new FrameRateTaskExecutor<int>(
                                () => gameTimer.IsGaming,
                                () =>
                                {
                                    if (obj.StateNum != stateNum || !obj.CanMove || obj.IsRemoved)
                                        return !(isEnded = true);
                                    return !(isEnded = !LoopDo(obj, direction, ref deltaLen, stateNum));
                                },
                                GameData.NumOfPosGridPerCell / GameData.NumOfStepPerSecond,
                                () =>
                                {
                                    return 0;
                                },
                                maxTotalDuration: moveTime - GameData.NumOfPosGridPerCell / GameData.NumOfStepPerSecond
                            )
                            {
                                AllowTimeExceed = true,
                                MaxTolerantTimeExceedCount = ulong.MaxValue,
                                TimeExceedAction = b =>
                                {
                                    if (b) GameEngineLogging.logger.ConsoleLog(
                                            "Fatal Error: The computer runs so slow that " +
                                            "the object cannot finish moving during this time!!!!!!");
                                    else GameEngineLogging.logger.ConsoleLogDebug(
                                            "Debug info: Object moving time exceed for once");
                                }
                            }.Start();
                            if (!isEnded && obj.StateNum == stateNum && obj.CanMove && !obj.IsRemoved)
                                isEnded = !LoopDo(obj, direction, ref deltaLen, stateNum);
                        }
                        if (isEnded)
                        {
                            obj.IsMoving.SetROri(false);
                            EndMove(obj);
                            return;
                        }
                        if (obj.StateNum == stateNum && obj.CanMove && !obj.IsRemoved)
                        {
                            int leftTime = moveTime % (GameData.NumOfPosGridPerCell / GameData.NumOfStepPerSecond);
                            if (leftTime > 0)
                            {
                                Thread.Sleep(leftTime);  // 多移动的在这里补回来
                            }
                            do
                            {
                                flag = false;
                                moveVecLength = (double)deltaLen + leftTime * obj.MoveSpeed / GameData.NumOfPosGridPerCell;
                                res = new XY(direction, moveVecLength);
                                if ((collisionObj = collisionChecker.CheckCollisionWhenMoving(obj, res)) == null)
                                {
                                    obj.MovingSetPos(res, stateNum);
                                }
                                else
                                {
                                    switch (OnCollision(obj, collisionObj, res))
                                    {
                                        case AfterCollision.ContinueCheck:
                                            flag = true;
                                            break;
                                        case AfterCollision.Destroyed:
                                            GameEngineLogging.logger.ConsoleLogDebug(
                                                Logger.ObjInfo(obj)
                                                + " collide with "
                                                + Logger.ObjInfo(collisionObj)
                                                + " and has been removed from the game");
                                            isEnded = true;
                                            break;
                                        case AfterCollision.MoveMax:
                                            MoveMax(obj, res, stateNum);
                                            moveVecLength = 0;
                                            res = new XY(direction, moveVecLength);
                                            break;
                                    }
                                }
                            } while (flag);
                        }
                        obj.IsMoving.SetROri(false);  // 结束移动
                        EndMove(obj);
                    }
                }
            ).Start();
        }
    }
}
