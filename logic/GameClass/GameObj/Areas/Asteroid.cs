using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Asteroid : Immovable
{
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Asteroid(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}
