using Preparation.Interface;
using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Home : Immovable, IHome
{
    public AtomicLong TeamID { get; }
    public LongInTheVariableRange HP => new LongInTheVariableRange(GameData.HomeHP);

    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Square;
    public Home(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Home)
    {
    }
}
