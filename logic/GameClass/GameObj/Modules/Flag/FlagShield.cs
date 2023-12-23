using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagShield1 : IShield
{
    private const int cost = GameData.FlagShipShield1Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield1;
    public int ShieldHP => shieldHP;
}
public class FlagShield2 : IShield
{
    private const int cost = GameData.FlagShipShield2Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield2;
    public int ShieldHP => shieldHP;
}
public class FlagShield3 : IShield
{
    private const int cost = GameData.FlagShipShield3Cost;
    public int Cost => cost;
    private const int shieldHP = GameData.Shield3;
    public int ShieldHP => shieldHP;
}