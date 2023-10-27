using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole : Immovable, IWormhole
{
    public List<XY> Entrance => throw new NotImplementedException();
    public List<XY> Content => throw new NotImplementedException();
    public override bool IsRigid => throw new NotImplementedException();
    public override ShapeType Shape => ShapeType.Square;
    public Wormhole(XY initPos, GameObjType initType)
        : base(initPos, int.MaxValue, initType)
    {
    }
}