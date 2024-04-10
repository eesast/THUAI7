using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public class Home(XY initPos, long id)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Home), IHome
{
    public long TeamID { get; } = id;
    public InVariableRange<long> HP => new(GameData.HomeHP);
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;

    public void BeAttacked(Bullet bullet)
    {
        if (bullet!.Parent!.TeamID != TeamID)
            HP.SubPositiveV(bullet.AP);
    }
}
