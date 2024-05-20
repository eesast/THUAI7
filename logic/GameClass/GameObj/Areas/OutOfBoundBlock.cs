using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj.Areas;

/// <summary>
/// 逻辑墙
/// </summary>
public class OutOfBoundBlock(XY initPos)
    : Immovable(initPos, int.MaxValue, GameObjType.OutOfBoundBlock), IOutOfBound
{
    public override bool IsRigid(bool args = false) => true;
    public override ShapeType Shape => ShapeType.Square;
}
