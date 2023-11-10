using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullProducer : IProducer
{
    public static NullProducer Instance { get; } = new();
    public int ProduceSpeed => 0;
    public int Cost => 0;
    private NullProducer() { }
}
