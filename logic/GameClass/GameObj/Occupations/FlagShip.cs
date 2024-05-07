using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class FlagShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.FlagShipMoveSpeed;
    public int MaxHp { get; } = GameData.FlagShipMaxHP;
    public int ViewRange { get; } = GameData.FlagShipViewRange;
    public int Cost { get; } = GameData.FlagShipCost;
    public int BaseArmor { get; } = GameData.FlagShipBaseArmor;
    public int BaseShield { get; } = GameData.FlagShipBaseShield;
    public bool IsModuleValid(ModuleType moduleType) => moduleType switch
    {
        ModuleType.Producer1 => true,
        ModuleType.Producer2 => false,
        ModuleType.Producer3 => false,
        ModuleType.Constructor1 => true,
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