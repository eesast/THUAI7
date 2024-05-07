using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class NullConstructor : IConstructor
{
    public static NullConstructor Instance { get; } = new();
    public int ConstructSpeed => 0;
    public int Cost => 0;
    public ConstructorType ConstructorModuleType => ConstructorType.Null;
    private NullConstructor() { }
}
