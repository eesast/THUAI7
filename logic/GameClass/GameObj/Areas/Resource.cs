using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public class Resource(XY initPos)
    : Immovable(initPos, GameData.NumOfPosGridPerCell / 2, GameObjType.Resource)
{
    public InVariableRange<long> HP { get; } = new InVariableRange<long>(GameData.ResourceHP);
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Square;
    public AtomicInt ProduceNum { get; } = new AtomicInt(0);
    public bool Produce(int produceSpeed, Ship ship)
    {
        return ship.MoneyPool.AddMoney(-HP.SubRChange(produceSpeed)) > 0;
    }
    public void AddProduceNum(int add = 1)
    {
        ProduceNum.Add(add);
    }
    public void SubProduceNum(int sub = 1)
    {
        ProduceNum.Sub(sub);
    }
}
