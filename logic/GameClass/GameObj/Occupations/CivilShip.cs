using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class CivilShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.CivilShipMoveSpeed;
    public int MaxHp { get; } = GameData.CivilShipMaxHP;
    public int ViewRange { get; } = GameData.CivilShipViewRange;
    public int Cost { get; } = GameData.CivilShipCost;
    public int BaseArmor { get; } = GameData.CivilShipBaseArmor;
    public int BaseShield { get; } = GameData.CivilShipBaseShield;
    public bool IsModuleValid(ModuleType moduleType) => moduleType switch
    {
        ModuleType.Producer1 => true,
        ModuleType.Producer2 => true,
        ModuleType.Producer3 => true,
        ModuleType.Constructor1 => true,
        ModuleType.Constructor2 => true,
        ModuleType.Constructor3 => true,
        ModuleType.Armor1 => true,
        ModuleType.Armor2 => false,
        ModuleType.Armor3 => false,
        ModuleType.Shield1 => true,
        ModuleType.Shield2 => false,
        ModuleType.Shield3 => false,
        ModuleType.LaserGun => true,
        ModuleType.PlasmaGun => false,
        ModuleType.ShellGun => false,
        ModuleType.MissileGun => false,
        ModuleType.ArcGun => false,
        _ => false
    };
}