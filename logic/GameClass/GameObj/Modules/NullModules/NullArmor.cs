using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class NullArmor : IArmor
{
    public static NullArmor Instance { get; } = new();
    public int ArmorHP => 0;
    public int Cost => 0;
    public ArmorType ArmorModuleType => ArmorType.Null;
    private NullArmor() { }
}
