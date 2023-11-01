using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullProducer : IProducer
{
    public int ProduceSpeed => 0;
    public int Cost => 0;
}
