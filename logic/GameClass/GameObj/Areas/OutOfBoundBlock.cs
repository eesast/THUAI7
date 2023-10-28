using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Areas;

/// <summary>
/// 逻辑墙
/// </summary>
public class OutOfBoundBlock : Immovable, IOutOfBound
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public OutOfBoundBlock(XY initPos)
    : base(initPos, int.MaxValue, GameObjType.OutOfBoundBlock)
    {
    }
}
