using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilArmor1 : IArmor
{
    private const int cost = GameData.CivilShipArmor1Cost;
    public int Cost => cost;
    private const int armorHP = GameData.Armor1;
    public int ArmorHP => armorHP;
}