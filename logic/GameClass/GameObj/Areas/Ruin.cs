using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public class Ruin(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Ruin)
{
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
}
