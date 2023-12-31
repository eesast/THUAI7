using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagShield1 : IShield
{
    public int Cost => GameData.FlagShipShield1Cost;
    public int ShieldHP => GameData.Shield1;
}
public class FlagShield2 : IShield
{
    public int Cost => GameData.FlagShipShield2Cost;
    public int ShieldHP => GameData.Shield2;
}
public class FlagShield3 : IShield
{
    public int Cost => GameData.FlagShipShield3Cost;
    public int ShieldHP => GameData.Shield3;
}