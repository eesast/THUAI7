using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Shadow(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Shadow)
{
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Square;
}