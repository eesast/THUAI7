using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue.Atomic;
using System.Threading;

namespace GameClass.GameObj
{
    public abstract class Movable(XY initPos, int initRadius, GameObjType initType)
        : GameObj(initPos, initRadius, initType), IMovable
    {
        protected readonly object actionLock = new();
        /// <summary>
        /// Player.ActionLock > 其他.ActionLock / 其他Lock，应当避免两个Player的Actionlock互锁
        /// </summary>
        public object ActionLock => actionLock;
        private readonly ReaderWriterLockSlim moveReaderWriterLock = new();
        /// <summary>
        /// 规定ActionLock > MoveReaderWriterLock  
        /// </summary>
        public ReaderWriterLockSlim MoveReaderWriterLock => moveReaderWriterLock;
        public Semaphore ThreadNum { get; } = new(1, 1);
        protected long stateNum = 0;
        public long StateNum
        {
            get
            {
                lock (actionLock)
                    return stateNum;
            }
            set
            {
                lock (actionLock) stateNum = value;
            }
        }
        protected RunningStateType runningState = RunningStateType.Null;
        public RunningStateType RunningState
        {
            get
            {
                lock (actionLock) return runningState;
            }
            set
            {
                lock (actionLock)
                    runningState = value;
            }
        }
        public override XY Position
        {
            get
            {
                lock (actionLock)
                    return position;
            }
        }
        protected XY facingDirection = new(1, 0);
        public XY FacingDirection
        {
            get
            {
                lock (actionLock)
                    return facingDirection;
            }
            set
            {
                lock (actionLock)
                    facingDirection = value;
            }
        }
        public AtomicBool IsMoving { get; } = new(false);
        /// <summary>
        /// 移动，改变坐标
        /// </summary>
        public long MovingSetPos(XY moveVec, long stateNo)
        {

            if (moveVec.x != 0 || moveVec.y != 0)
            {
                lock (actionLock)
                {
                    if (!CanMove || IsRemoved) return -1;
                    if (stateNo != stateNum) return -1;
                    facingDirection = moveVec;
                    this.position += moveVec;
                }
            }
            return moveVec * moveVec;
        }

        public void ReSetPos(XY position)
        {
            lock (actionLock)
            {
                this.position = position;
            }
        }
        private readonly AtomicBool canMove = new(false);
        public AtomicBool CanMove { get => canMove; }
        public bool IsAvailableForMove => !IsMoving && CanMove && !IsRemoved; // 是否能接收移动指令
        private readonly AtomicInt moveSpeed = new(0);
        /// <summary>
        /// 移动速度
        /// </summary>
        public AtomicInt MoveSpeed { get => moveSpeed; }
        protected int orgMoveSpeed;
        /// <summary>
        /// 原初移动速度
        /// </summary>
        public int OrgMoveSpeed => orgMoveSpeed;

        /*/// <summary>
        /// 复活时数据重置
        /// </summary>
        public virtual void Reset(PlaceType place)
        {
            lock (gameObjLock)
            {
                this.FacingDirection = new XY(1, 0);
                isMoving = false;
                CanMove = false;
                IsRemoved = true;
                this.Position = birthPos;
                this.Place = place;
            }
        }*/
    }
}
