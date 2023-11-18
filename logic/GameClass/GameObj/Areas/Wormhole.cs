using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole : Immovable, IWormhole
{
    public LongInTheVariableRange HP = new LongInTheVariableRange(GameData.WormholeHP);
    private List<XY> grids = new();
    public List<XY> Grids => grids;
    public override bool IsRigid => HP > GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public Wormhole(XY initPos, List<XY> grids)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
    {
        this.grids = grids;
    }
}