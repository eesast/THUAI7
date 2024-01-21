using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class WarShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.WarShipMoveSpeed;
    public int MaxHp { get; } = GameData.WarShipMaxHP;
    public int ViewRange { get; } = GameData.WarShipViewRange;
    public int Cost { get; } = GameData.WarShipCost;
    public int BaseArmor { get; } = GameData.WarShipBaseArmor;
    public int BaseShield { get; } = GameData.WarShipBaseShield;
    public bool IsModuleValid(ModuleType moduleType) => moduleType switch
    {
        ModuleType.Producer1 => false,
        ModuleType.Producer2 => false,
        ModuleType.Producer3 => false,
        ModuleType.Constructor1 => false,
        ModuleType.Constructor2 => false,
        ModuleType.Constructor3 => false,
        ModuleType.Armor1 => true,
        ModuleType.Armor2 => true,
        ModuleType.Armor3 => true,
        ModuleType.Shield1 => true,
        ModuleType.Shield2 => true,
        ModuleType.Shield3 => true,
        ModuleType.LaserGun => true,
        ModuleType.PlasmaGun => true,
        ModuleType.ShellGun => true,
        ModuleType.MissileGun => true,
        ModuleType.ArcGun => true,
        _ => false
    };
}