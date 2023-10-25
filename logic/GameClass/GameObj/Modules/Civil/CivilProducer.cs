using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilProducer1 : IProducer
{
    private const int cost = GameData.CivilShipProducer1Cost;
    public int Cost => cost;
    private const int produceSpeed = GameData.ScoreProducer1PerSecond;
    public int ProduceSpeed => produceSpeed;
}
public class CivilProducer2 : IProducer
{
    private const int cost = GameData.CivilShipProducer1Cost;
    public int Cost => cost;
    private const int produceSpeed = GameData.ScoreProducer2PerSecond;
    public int ProduceSpeed => produceSpeed;
}
public class CivilProducer3 : IProducer
{
    private const int cost = GameData.CivilShipProducer1Cost;
    public int Cost => cost;
    private const int produceSpeed = GameData.ScoreProducer3PerSecond;
    public int ProduceSpeed => produceSpeed;
}