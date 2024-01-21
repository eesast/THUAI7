using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilArmor1 : IArmor
{
    public int Cost => GameData.CivilShipArmor1Cost;
    public int ArmorHP => GameData.Armor1;
}