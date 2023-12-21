using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullShield : IShield
{
    public static NullShield Instance { get; } = new();
    public int ShieldHP => 0;
    public int Cost => 0;
    private NullShield() { }
}
