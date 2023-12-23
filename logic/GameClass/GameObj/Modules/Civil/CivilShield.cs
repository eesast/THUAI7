using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilShield1 : IShield
{
    private const int cost = GameData.CivilShipShield1Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield1;
    public int ShieldHP => shieldHP;
}