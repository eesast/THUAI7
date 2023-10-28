using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class WarShield1 : IShield
{
    private const int cost = GameData.WarShipShield1Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield1;
    public int ShieldHP => shieldHP;
}
public class WarShield2 : IShield
{
    private const int cost = GameData.WarShipShield2Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield2;
    public int ShieldHP => shieldHP;
}
public class WarShield3 : IShield
{
    private const int cost = GameData.WarShipShield3Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield3;
    public int ShieldHP => shieldHP;
}