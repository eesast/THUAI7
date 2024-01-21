using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilProducer1 : IProducer
{
    public int Cost => GameData.CivilShipProducer1Cost;
    public int ProduceSpeed => GameData.ScoreProducer1PerSecond;
}
public class CivilProducer2 : IProducer
{
    public int Cost => GameData.CivilShipProducer2Cost;
    public int ProduceSpeed => GameData.ScoreProducer2PerSecond;
}
public class CivilProducer3 : IProducer
{
    public int Cost => GameData.CivilShipProducer3Cost;
    public int ProduceSpeed => GameData.ScoreProducer3PerSecond;
}