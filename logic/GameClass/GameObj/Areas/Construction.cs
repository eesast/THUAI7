using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Construction : Immovable
{
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Construction(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}
