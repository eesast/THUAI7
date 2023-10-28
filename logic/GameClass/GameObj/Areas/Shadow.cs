using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Shadow : Immovable
{
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Square;
    public Shadow(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Shadow)
    {
    }
}