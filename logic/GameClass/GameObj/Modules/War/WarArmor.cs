using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class WarArmor1 : IArmor
{
    private const int cost = GameData.WarShipArmor1Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor1;
    public int ArmorHP => armorHP;
}
public class WarArmor2 : IArmor
{
    private const int cost = GameData.WarShipArmor2Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor2;
    public int ArmorHP => armorHP;
}
public class WarArmor3 : IArmor
{
    private const int cost = GameData.WarShipArmor3Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor3;
    public int ArmorHP => armorHP;
}