using Preparation.Interface;
using Preparation.Utility;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class WormholeCell(XY initPos, Wormhole wormhole)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
{
    public override bool IsRigid => Wormhole.HP < GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public readonly Wormhole Wormhole = wormhole;
}
