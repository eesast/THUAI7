using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullConstructor : IConstructor
{
    public int ConstructSpeed => 0;
    public int Cost => 0;
}
