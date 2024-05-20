using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj.Areas;

public class WormholeCell(XY initPos, Wormhole wormhole)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
{
    public override bool IsRigid(bool args = false) => args || Wormhole.HP < GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public readonly Wormhole Wormhole = wormhole;
}
