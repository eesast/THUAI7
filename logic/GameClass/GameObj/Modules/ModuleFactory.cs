using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public static class ModuleFactory
{
    public static IProducer FindIProducer(ShipType shipType, ProducerType producerType) => shipType switch
    {
        ShipType.CivilShip => producerType switch
        {
            ProducerType.Producer1 => new CivilProducer1(),
            ProducerType.Producer2 => new CivilProducer2(),
            ProducerType.Producer3 => new CivilProducer3(),
            _ => NullProducer.Instance
        },
        ShipType.FlagShip => producerType switch
        {
            ProducerType.Producer1 => new FlagProducer1(),
            _ => NullProducer.Instance
        },
        _ => NullProducer.Instance
    };
    public static IConstructor FindIConstructor(ShipType shipType, ConstructorType constructorType) => shipType switch
    {
        ShipType.CivilShip => constructorType switch
        {
            ConstructorType.Constructor1 => new CivilConstructor1(),
            ConstructorType.Constructor2 => new CivilConstructor2(),
            ConstructorType.Constructor3 => new CivilConstructor3(),
            _ => NullConstructor.Instance
        },
        ShipType.FlagShip => constructorType switch
        {
            ConstructorType.Constructor1 => new FlagConstructor1(),
            _ => NullConstructor.Instance
        },
        _ => NullConstructor.Instance
    };
    public static IArmor FindIArmor(ShipType shipType, ArmorType armorType) => shipType switch
    {
        ShipType.CivilShip => armorType switch
        {
            ArmorType.Armor1 => new CivilArmor1(),
            _ => NullArmor.Instance
        },
        ShipType.WarShip => armorType switch
        {
            ArmorType.Armor1 => new WarArmor1(),
            ArmorType.Armor2 => new WarArmor2(),
            ArmorType.Armor3 => new WarArmor3(),
            _ => NullArmor.Instance
        },
        ShipType.FlagShip => armorType switch
        {
            ArmorType.Armor1 => new FlagArmor1(),
            ArmorType.Armor2 => new FlagArmor2(),
            ArmorType.Armor3 => new FlagArmor3(),
            _ => NullArmor.Instance
        },
        _ => NullArmor.Instance
    };
    public static IShield FindIShield(ShipType shipType, ShieldType shieldType) => shipType switch
    {
        ShipType.CivilShip => shieldType switch
        {
            ShieldType.Shield1 => new CivilShield1(),
            _ => NullShield.Instance
        },
        ShipType.WarShip => shieldType switch
        {
            ShieldType.Shield1 => new WarShield1(),
            ShieldType.Shield2 => new WarShield2(),
            ShieldType.Shield3 => new WarShield3(),
            _ => NullShield.Instance
        },
        ShipType.FlagShip => shieldType switch
        {
            ShieldType.Shield1 => new FlagShield1(),
            ShieldType.Shield2 => new FlagShield2(),
            ShieldType.Shield3 => new FlagShield3(),
            _ => NullShield.Instance
        },
        _ => NullShield.Instance
    };
    public static IWeapon FindIWeapon(ShipType shipType, WeaponType weaponType) => shipType switch
    {
        ShipType.CivilShip => weaponType switch
        {
            WeaponType.LaserGun => new CivilLaserGun(),
            _ => NullWeapon.Instance
        },
        ShipType.WarShip => weaponType switch
        {
            WeaponType.LaserGun => new WarLaserGun(),
            WeaponType.PlasmaGun => new WarPlasmaGun(),
            WeaponType.ShellGun => new WarShellGun(),
            WeaponType.MissileGun => new WarMissileGun(),
            WeaponType.ArcGun => new WarArcGun(),
            _ => NullWeapon.Instance
        },
        ShipType.FlagShip => weaponType switch
        {
            WeaponType.LaserGun => new FlagLaserGun(),
            WeaponType.PlasmaGun => new FlagPlasmaGun(),
            WeaponType.ShellGun => new FlagShellGun(),
            WeaponType.MissileGun => new FlagMissileGun(),
            WeaponType.ArcGun => new FlagArcGun(),
            _ => NullWeapon.Instance
        },
        _ => NullWeapon.Instance
    };
    public static int FindModuleCost(ShipType shipType, ModuleType moduleType) => shipType switch
    {
        ShipType.CivilShip => moduleType switch
        {
            ModuleType.Producer1 => GameData.CivilShipProducer1Cost,
            ModuleType.Producer2 => GameData.CivilShipProducer2Cost,
            ModuleType.Producer3 => GameData.CivilShipProducer3Cost,
            ModuleType.Constructor1 => GameData.CivilShipConstructor1Cost,
            ModuleType.Constructor2 => GameData.CivilShipConstructor2Cost,
            ModuleType.Constructor3 => GameData.CivilShipConstructor3Cost,
            ModuleType.Armor1 => GameData.CivilShipArmor1Cost,
            ModuleType.Shield1 => GameData.CivilShipShield1Cost,
            ModuleType.LaserGun => GameData.CivilShipLaserGunCost,
            _ => int.MaxValue
        },
        ShipType.WarShip => moduleType switch
        {
            ModuleType.Armor1 => GameData.WarShipArmor1Cost,
            ModuleType.Armor2 => GameData.WarShipArmor2Cost,
            ModuleType.Armor3 => GameData.WarShipArmor3Cost,
            ModuleType.Shield1 => GameData.WarShipShield1Cost,
            ModuleType.Shield2 => GameData.WarShipShield2Cost,
            ModuleType.Shield3 => GameData.WarShipShield3Cost,
            ModuleType.LaserGun => GameData.WarShipLaserGunCost,
            ModuleType.PlasmaGun => GameData.WarShipPlasmaGunCost,
            ModuleType.ShellGun => GameData.WarShipShellGunCost,
            ModuleType.MissileGun => GameData.WarShipMissileGunCost,
            ModuleType.ArcGun => GameData.WarShipArcGunCost,
            _ => int.MaxValue
        },
        ShipType.FlagShip => moduleType switch
        {
            ModuleType.Producer1 => GameData.FlagShipProducer1Cost,
            ModuleType.Constructor1 => GameData.FlagShipConstructor1Cost,
            ModuleType.Armor1 => GameData.FlagShipArmor1Cost,
            ModuleType.Armor2 => GameData.FlagShipArmor2Cost,
            ModuleType.Armor3 => GameData.FlagShipArmor3Cost,
            ModuleType.Shield1 => GameData.FlagShipShield1Cost,
            ModuleType.Shield2 => GameData.FlagShipShield2Cost,
            ModuleType.Shield3 => GameData.FlagShipShield3Cost,
            ModuleType.LaserGun => GameData.FlagShipLaserGunCost,
            ModuleType.PlasmaGun => GameData.FlagShipPlasmaGunCost,
            ModuleType.ShellGun => GameData.FlagShipShellGunCost,
            ModuleType.MissileGun => GameData.FlagShipMissileGunCost,
            ModuleType.ArcGun => GameData.FlagShipArcGunCost,
            _ => int.MaxValue
        }
    }
}