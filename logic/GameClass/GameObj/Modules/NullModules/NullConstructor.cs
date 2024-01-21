using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullConstructor : IConstructor
{
    public static NullConstructor Instance { get; } = new();
    public int ConstructSpeed => 0;
    public int Cost => 0;
    private NullConstructor() { }
}
