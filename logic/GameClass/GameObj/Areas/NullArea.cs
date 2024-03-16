using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public class NullArea(XY initPos)
    : Immovable(initPos, int.MaxValue, GameObjType.Null)
{
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Null;
}
