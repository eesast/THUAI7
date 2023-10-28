using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole : Immovable, IWormhole
{
    public LongWithVariableRange HP => throw new NotImplementedException();
    public List<XY> Entrance => throw new NotImplementedException();
    public List<XY> Content => throw new NotImplementedException();
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public override bool IgnoreCollideExecutor(IGameObj targetObj)
    {
        if (HP > GameData.WormholeHP / 2)
        {
            return true;
        }
        return false;
    }
    public Wormhole(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
    {
    }
}