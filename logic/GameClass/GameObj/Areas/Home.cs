using Preparation.Interface;
using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Home : Immovable, IHome
{
    public AtomicLong TeamID => throw new NotImplementedException();
    public LongWithVariableRange HP => throw new NotImplementedException();
    public long Score => throw new NotImplementedException();
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public void AddScore(long add)
    {
        throw new NotImplementedException();
    }
    public Home(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}
