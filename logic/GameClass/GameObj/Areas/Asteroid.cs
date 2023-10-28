using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Asteroid : Immovable
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public Asteroid(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Asteroid)
    {
    }
}
