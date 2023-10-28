using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole : Immovable, IWormhole
{
    public LongInTheVariableRange HP => throw new NotImplementedException();
    public List<XY> Entrance => throw new NotImplementedException();
    public List<XY> Content => throw new NotImplementedException();
    public override bool IsRigid => HP > GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public Wormhole(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
    {
    }
}