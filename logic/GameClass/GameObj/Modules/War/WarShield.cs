using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class WarShield1 : IShield
{
    public int Cost => GameData.WarShipShield1Cost;
    public int ShieldHP => GameData.Shield1;
    public ShieldType ShieldModuleType => ShieldType.Shield1;
}
public class WarShield2 : IShield
{
    public int Cost => GameData.WarShipShield2Cost;
    public int ShieldHP => GameData.Shield2;
    public ShieldType ShieldModuleType => ShieldType.Shield2;
}
public class WarShield3 : IShield
{
    public int Cost => GameData.WarShipShield3Cost;
    public int ShieldHP => GameData.Shield3;
    public ShieldType ShieldModuleType => ShieldType.Shield3;
}