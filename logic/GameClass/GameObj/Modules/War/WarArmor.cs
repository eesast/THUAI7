using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class WarArmor1 : IArmor
{
    public int Cost => GameData.WarShipArmor1Cost;
    public int ArmorHP => GameData.Armor1;
    public ArmorType ArmorModuleType => ArmorType.Armor1;
}
public class WarArmor2 : IArmor
{
    public int Cost => GameData.WarShipArmor2Cost;
    public int ArmorHP => GameData.Armor2;
    public ArmorType ArmorModuleType => ArmorType.Armor2;
}
public class WarArmor3 : IArmor
{
    public int Cost => GameData.WarShipArmor3Cost;
    public int ArmorHP => GameData.Armor3;
    public ArmorType ArmorModuleType => ArmorType.Armor3;
}