using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagProducer1 : IProducer
{
    private const int cost = GameData.FlagShipProducer1Cost;
    public int Cost => cost;
    private const int produceSpeed = GameData.ScoreProducer1PerSecond;
    public int ProduceSpeed => produceSpeed;
}