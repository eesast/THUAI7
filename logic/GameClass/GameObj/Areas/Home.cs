using Preparation.Interface;
using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Home : Immovable, IHome
{
    private long teamID;
    public long TeamID => teamID;
    public LongInTheVariableRange HP => new LongInTheVariableRange(GameData.HomeHP);
    public AtomicLong Score => new AtomicLong(0);
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Square;
    public void AddScore(long add)
    {
        Score.Add(add);
    }
    public Home(XY initPos, long teamID)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Home)
    {
        this.teamID = teamID;
    }
}
