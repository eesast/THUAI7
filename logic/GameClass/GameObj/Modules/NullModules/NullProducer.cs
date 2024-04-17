using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class NullProducer : IProducer
{
    public static NullProducer Instance { get; } = new();
    public int ProduceSpeed => 0;
    public int Cost => 0;
    public ProducerType ProducerModuleType => ProducerType.Null;
    private NullProducer() { }
}