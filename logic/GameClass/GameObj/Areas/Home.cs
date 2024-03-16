using Preparation.Interface;
using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Home(XY initPos, long id)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Home), IHome
{
    public long TeamID { get; } = id;
    public LongInTheVariableRange HP => new(GameData.HomeHP);
    public override bool IsRigid => false;
    public override ShapeType Shape => ShapeType.Square;

    public void BeAttacked(Bullet bullet)
    {
        if (bullet!.Parent!.TeamID == this.TeamID)
        {
            return;
        }
        this.HP.SubPositiveV(bullet.AP);
    }
}
