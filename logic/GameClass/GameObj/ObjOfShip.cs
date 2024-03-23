using Preparation.Interface;
using Preparation.Utility;
using System.Threading;

namespace GameClass.GameObj
{
    /// <summary>
    /// 所有物，具有主人（Parent）（特定玩家）属性的对象
    /// </summary>
    public abstract class ObjOfShip(XY initPos, int initRadius, GameObjType initType)
        : Movable(initPos, initRadius, initType), IObjOfShip
    {
        private IShip? parent = null;
        public IShip? Parent
        {
            get
            {
                return Interlocked.CompareExchange(ref parent, null, null);
            }
            set
            {
                Interlocked.Exchange(ref parent, value);
            }
        }
    }
}
