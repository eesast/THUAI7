using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullArmor : IArmor
{
    public int ArmorHP => 0;
    public int Cost => 0;
}
