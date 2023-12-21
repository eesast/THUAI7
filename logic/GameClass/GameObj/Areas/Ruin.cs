using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Ruin : Immovable
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public Ruin(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Ruin)
    {
    }
}
