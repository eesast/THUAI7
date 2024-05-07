using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagArmor1 : IArmor
{
    public int Cost => GameData.FlagShipArmor1Cost;
    public int ArmorHP => GameData.Armor1;
    public ArmorType ArmorModuleType => ArmorType.Armor1;
}
public class FlagArmor2 : IArmor
{
    public int Cost => GameData.FlagShipArmor2Cost;
    public int ArmorHP => GameData.Armor2;
    public ArmorType ArmorModuleType => ArmorType.Armor2;
}
public class FlagArmor3 : IArmor
{
    public int Cost => GameData.FlagShipArmor3Cost;
    public int ArmorHP => GameData.Armor3;
    public ArmorType ArmorModuleType => ArmorType.Armor3;
}