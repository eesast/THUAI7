using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Resource : Immovable
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public Resource(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Resource)
    {
    }
}
