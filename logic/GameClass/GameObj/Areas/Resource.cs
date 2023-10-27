using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Resource : Immovable
{
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Resource(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}
