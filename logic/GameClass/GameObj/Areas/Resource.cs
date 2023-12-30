using Preparation.Utility;
using System;
using System.Threading;

namespace GameClass.GameObj.Areas;

public class Resource : Immovable
{
    public LongInTheVariableRange HP { get; } = new LongInTheVariableRange(GameData.ResourceHP);
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public AtomicInt ProduceNum { get; } = new AtomicInt(0);
    public bool Produce(int produceSpeed, Ship ship)
    {
        if (HP.SubV(produceSpeed) > 0)
        {
            ship.MoneyPool.AddMoney(produceSpeed);
            return true;
        }
        return false;
    }
    public void AddProduceNum(int add = 1)
    {
        ProduceNum.Add(add);
    }
    public void SubProduceNum(int sub = 1)
    {
        ProduceNum.Sub(sub);
    }
    public Resource(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Resource)
    {
    }
}
