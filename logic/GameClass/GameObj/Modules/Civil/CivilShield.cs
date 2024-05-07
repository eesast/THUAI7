using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilShield1 : IShield
{
    public int Cost => GameData.CivilShipShield1Cost;
    public int ShieldHP => GameData.Shield1;
    public ShieldType ShieldModuleType => ShieldType.Shield1;
}