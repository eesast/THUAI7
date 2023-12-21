using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public class NullArea : Immovable
{
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Null;
    public NullArea(XY initPos)
        : base(initPos, int.MaxValue, GameObjType.Null)
    {
    }
}
