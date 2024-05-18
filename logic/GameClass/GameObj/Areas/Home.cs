using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.LockedValue;

namespace GameClass.GameObj.Areas;

public class Home(XY initPos, long id)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Home), IHome
{
    public long TeamID { get; } = id;
    public InVariableRange<long> HP { get; } = new(GameData.HomeHP);
    public override bool IsRigid(bool args = false) => true;
    public override ShapeType Shape => ShapeType.Square;
    public AtomicInt RepairNum { get; } = new AtomicInt(0);
    public bool Repair(int constructSpeed, Ship ship)
    {
        return HP.AddVUseOtherRChange(constructSpeed, ship.MoneyPool.Money, 1) > 0;
    }
    public void BeAttacked(Bullet bullet)
    {
        if (bullet!.Parent!.TeamID != TeamID)
            HP.SubPositiveV(bullet.AP);
    }
    public void AddRepairNum(int add = 1)
    {
        RepairNum.Add(add);
    }
    public void SubRepairNum(int sub = 1)
    {
        RepairNum.Sub(sub);
    }
}
