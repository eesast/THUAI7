using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullArmor : IArmor
{
    public static NullArmor Instance { get; } = new();
    public int ArmorHP => 0;
    public int Cost => 0;
    private NullArmor() { }
}
