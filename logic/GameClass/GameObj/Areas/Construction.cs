﻿using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Construction(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Construction)
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public LongInTheVariableRange HP { get; } = new(0, GameData.CommunityHP);
    public override bool IsRigid => constructionType == ConstructionType.Community;
    public override ShapeType Shape => ShapeType.Square;

    private readonly object lockOfConstructionType = new();
    private ConstructionType constructionType = ConstructionType.Null;
    public ConstructionType ConstructionType
    {
        get
        {
            lock (lockOfConstructionType)
                return constructionType;
        }
    }
    public AtomicInt ConstructNum { get; } = new AtomicInt(0);

    public bool Construct(int constructSpeed, ConstructionType constructionType, Ship ship)
    {
        if (constructionType == ConstructionType.Null)
            return false;
        lock (lockOfConstructionType)
        {
            if (this.constructionType == ConstructionType.Null || HP == 0)
            {
                TeamID.SetROri(ship.TeamID);
                this.constructionType = constructionType;
                switch (constructionType)
                {
                    case ConstructionType.Community:
                        HP.SetMaxV(GameData.CommunityHP);
                        break;
                    case ConstructionType.Factory:
                        HP.SetMaxV(GameData.FactoryHP);
                        break;
                    case ConstructionType.Fort:
                        HP.SetMaxV(GameData.FortHP);
                        break;
                    default:
                        break;
                }
            }
            else
            if (this.constructionType != constructionType)
            {
                return false;
            }
        }
        var addHP = HP.GetMaxV() - HP > constructSpeed ? constructSpeed : HP.GetMaxV() - HP;
        if (ship.MoneyPool.Money < addHP / 10)
        {
            return false;
        }
        return ship.MoneyPool.SubMoney(HP.AddV(addHP) / 10) > 0;
    }
    public void BeAttacked(Bullet bullet)
    {
        if (bullet!.Parent!.TeamID != TeamID)
        {
            long subHP = bullet.AP;
            HP.SubPositiveV(subHP);
        }
        if (HP == 0)
        {
            constructionType = ConstructionType.Null;
        }
    }
    public void AddConstructNum(int add = 1)
    {
        ConstructNum.Add(add);
    }
    public void SubConstructNum(int sub = 1)
    {
        ConstructNum.Sub(sub);
    }
}
