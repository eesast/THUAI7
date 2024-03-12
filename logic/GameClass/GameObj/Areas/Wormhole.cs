using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Collections.Generic;

namespace GameClass.GameObj.Areas;

public class Wormhole : Immovable, IWormhole
{
    public LongInTheVariableRange HP = new LongInTheVariableRange(GameData.WormholeHP);
    private readonly List<XY> grids = new();
    public List<XY> Grids => grids;
    public override bool IsRigid => HP > GameData.WormholeHP / 2;
    public override ShapeType Shape => ShapeType.Square;
    public AtomicInt RepairNum { get; } = new AtomicInt(0);
    public bool Repair(int constructSpeed, Ship ship)
    {
        return HP.AddV(constructSpeed) > 0;
    }
    public void BeAttacked(Bullet bullet)
    {
        long subHP = bullet.AP;
        this.HP.SubPositiveV(subHP);
    }
    public void AddRepairNum(int add = 1)
    {
        RepairNum.Add(add);
    }
    public void SubRepairNum(int sub = 1)
    {
        RepairNum.Sub(sub);
    }
    public Wormhole(XY initPos, List<XY> grids)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Wormhole)
    {
        this.grids = grids;
    }
}