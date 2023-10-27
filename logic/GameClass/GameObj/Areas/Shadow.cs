using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Shadow : Immovable
{
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Shadow(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}