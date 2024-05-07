using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue;
using System.Collections.Generic;

namespace Preparation.Interface
{
    public interface IMap
    {
        IMyTimer Timer { get; }

        // the two dicts must have same keys
        Dictionary<GameObjType, LockedClassList<IGameObj>> GameObjDict { get; }

        public PlaceType[,] ProtoGameMap { get; }
        public PlaceType GetPlaceType(IGameObj obj);
        public bool IsOutOfBound(IGameObj obj);
        public IOutOfBound GetOutOfBound(XY pos);  // 返回新建的一个OutOfBound对象
    }
}
