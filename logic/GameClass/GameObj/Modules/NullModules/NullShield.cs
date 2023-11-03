using Preparation.Interface;

namespace GameClass.GameObj.Modules;

public class NullShield : IShield
{
    public int ShieldHP => 0;
    public int Cost => 0;
}
