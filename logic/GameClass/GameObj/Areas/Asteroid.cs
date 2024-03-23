using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Asteroid(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Asteroid)
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
}
