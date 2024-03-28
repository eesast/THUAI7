using Preparation.Interface;
using Preparation.Utility;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole(XY initPos, List<XY> grids)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole), IWormhole
{
    public LongInTheVariableRange HP = new(GameData.WormholeHP);
    private readonly List<XY> grids = grids;
    public List<XY> Grids => grids;
    public override bool IsRigid => HP > GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public AtomicInt RepairNum { get; } = new AtomicInt(0);
    public bool Repair(int constructSpeed, Ship ship)
    {
        var addHP = HP.GetMaxV() - HP > constructSpeed ? constructSpeed : HP.GetMaxV() - HP;
        if (ship.MoneyPool.Money < addHP / 10)
        {
            return false;
        }
        return ship.MoneyPool.SubMoney(HP.AddV(addHP) / 10) > 0;
    }
    public void BeAttacked(Bullet bullet)
    {
        long subHP = bullet.AP;
        HP.SubPositiveV(subHP);
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