using Preparation.Utility;
using System;
using System.Threading;

namespace GameClass.GameObj.Areas;

public class Resource : Immovable
{
    public LongInTheVariableRange HP => throw new NotImplementedException();
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    private int producingNum = 0;
    public int ProducingNum
    {
        get => Interlocked.CompareExchange(ref producingNum, 0, 0);
    }
    public void AddProducingNum()
    {
        Interlocked.Increment(ref producingNum);
    }
    public void SubProducingNum()
    {
        Interlocked.Decrement(ref producingNum);
    }
    public bool Produce(int produceSpeed, Ship ship)
    {
        long orgHP, value;
        lock (gameObjLock)
        {
            if (HP == 0)
            {
                return false;
            }
            orgHP = HP.GetValue();
            HP.SubV(produceSpeed);
            if (HP > HP.GetMaxV())
            {
                HP.SetV(HP.GetMaxV());
            }
            else if (HP < 0)
            {
                HP.SetV(0);
            }
            value = HP.GetValue();
        }
        if (value < orgHP)
        {
            if (value == 0) return true;
        }
        return false;
    }
    public Resource(XY initPos)
        : base(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Resource)
    {
    }
}
