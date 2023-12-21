using Preparation.Utility;
using System;

namespace GameClass.GameObj.Areas;

public class Construction : Immovable
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public LongInTheVariableRange HP { get; } = new LongInTheVariableRange(0, GameData.CommunityHP);
    public override bool IsRigid => constructionType == ConstructionType.Community;
    public override ShapeType Shape => ShapeType.Square;
    private ConstructionType constructionType = ConstructionType.Null;
    public ConstructionType ConstructionType => constructionType;
    public AtomicInt ConstructNum { get; } = new AtomicInt(0);
    public bool Construct(int constructSpeed, ConstructionType constructionType, Ship ship)
    {
        if (constructionType == ConstructionType.Null)
        {
            return false;
        }
        if (this.constructionType != ConstructionType.Null && this.constructionType != constructionType && this.HP > 0)
        {
            return false;
        }
        if (this.constructionType == ConstructionType.Null || this.HP == 0)
        {
            this.TeamID.SetReturnOri(ship.TeamID);
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
        return HP.AddV(constructSpeed) > 0;
    }
    public void BeAttacked(Bullet bullet)
    {
        if (bullet!.Parent!.TeamID == this.TeamID)
        {
            return;
        }
        long subHP = bullet.AP;
        this.HP.SubPositiveV(subHP);
    }
    public void AddConstructNum(int add = 1)
    {
        ConstructNum.Add(add);
    }
    public void SubConstructNum(int sub = 1)
    {
        ConstructNum.Sub(sub);
    }
    public Construction(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Construction)
    {
    }
}
