using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagProducer1 : IProducer
{
    public int Cost => GameData.FlagShipProducer1Cost;
    public int ProduceSpeed => GameData.ScoreProducer1PerSecond;
    public ProducerType ProducerModuleType => ProducerType.Producer1;
}