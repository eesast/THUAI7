using Preparation.Interface;
using Preparation.Utility;
using System.Threading;

namespace GameClass.GameObj
{
    /// <summary>
    /// 所有物，具有主人（Parent）（特定玩家）属性的对象
    /// </summary>
    public abstract class ObjOfShip : Movable, IObjOfShip
    {
        private ReaderWriterLockSlim objOfShipReaderWriterLock = new();
        public ReaderWriterLockSlim ObjOfShipReaderWriterLock => objOfShipReaderWriterLock;
        private IShip? parent = null;
        public IShip? Parent
        {
            get
            {
                lock (objOfShipReaderWriterLock)
                    return parent;
            }
            set
            {
                lock (objOfShipReaderWriterLock)
                    parent = value;
            }
        }
        public ObjOfShip(XY initPos, int initRadius, GameObjType initType) :
            base(initPos, initRadius, initType)
        {
        }
    }
}
