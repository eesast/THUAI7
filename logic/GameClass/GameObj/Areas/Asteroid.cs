using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj.Areas;

public class Asteroid(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Asteroid)
{
    public override bool IsRigid(bool args = false) => true;
    public override ShapeType Shape => ShapeType.Square;
}
