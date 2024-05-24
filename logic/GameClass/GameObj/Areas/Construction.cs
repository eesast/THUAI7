using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.LockedValue;

namespace GameClass.GameObj.Areas;

public class Construction(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Construction)
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public InVariableRange<long> HP { get; } = new(0, GameData.CommunityHP);
    public override bool IsRigid(bool args = false) => true;
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
    public AtomicInt ConstructNum { get; } = new(0);
    public AtomicBool IsActivated { get; } = new(false);

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

        return HP.AddVUseOtherRChange<long>(constructSpeed, ship.MoneyPool.Money, 1) > 0;

    }
    public bool BeAttacked(Bullet bullet)
    {
        var previousActivated = IsActivated.Get();
        if (bullet!.Parent!.TeamID != TeamID)
        {
            long subHP = bullet.AP;
            HP.SubPositiveV(subHP);
        }
        if (HP.IsBelowMaxTimes(0.5))
        {
            IsActivated.Set(false);
        }
        return HP.IsBelowMaxTimes(0.5) && previousActivated;
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
