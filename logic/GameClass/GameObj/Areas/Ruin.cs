using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Ruin : Immovable
{
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Ruin(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}
