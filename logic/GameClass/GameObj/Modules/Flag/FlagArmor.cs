using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagArmor1 : IArmor
{
    private const int cost = GameData.FlagShipArmor1Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor1;
    public int ArmorHP => armorHP;
}
public class FlagArmor2 : IArmor
{
    private const int cost = GameData.FlagShipArmor2Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor2;
    public int ArmorHP => armorHP;
}
public class FlagArmor3 : IArmor
{
    private const int cost = GameData.FlagShipArmor3Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor3;
    public int ArmorHP => armorHP;
}