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
            _ => new NullProducer()
        },
        ShipType.FlagShip => producerType switch
        {
            ProducerType.Producer1 => new FlagProducer1(),
            _ => new NullProducer()
        },
        _ => new NullProducer()
    };
    public static IConstructor FindIConstructor(ShipType shipType, ConstructorType constructorType) => shipType switch
    {
        ShipType.CivilShip => constructorType switch
        {
            ConstructorType.Constructor1 => new CivilConstructor1(),
            ConstructorType.Constructor2 => new CivilConstructor2(),
            ConstructorType.Constructor3 => new CivilConstructor3(),
            _ => new NullConstructor()
        },
        ShipType.FlagShip => constructorType switch
        {
            ConstructorType.Constructor1 => new FlagConstructor1(),
            _ => new NullConstructor()
        },
        _ => new NullConstructor()
    };
    public static IArmor FindIArmor(ShipType shipType, ArmorType armorType) => shipType switch
    {
        ShipType.CivilShip => armorType switch
        {
            ArmorType.Armor1 => new CivilArmor1(),
            _ => new NullArmor()
        },
        ShipType.WarShip => armorType switch
        {
            ArmorType.Armor1 => new WarArmor1(),
            ArmorType.Armor2 => new WarArmor2(),
            ArmorType.Armor3 => new WarArmor3(),
            _ => new NullArmor()
        },
        ShipType.FlagShip => armorType switch
        {
            ArmorType.Armor1 => new FlagArmor1(),
            ArmorType.Armor2 => new FlagArmor2(),
            ArmorType.Armor3 => new FlagArmor3(),
            _ => new NullArmor()
        },
        _ => new NullArmor()
    };
    public static IShield FindIShield(ShipType shipType, ShieldType shieldType) => shipType switch
    {
        ShipType.CivilShip => shieldType switch
        {
            ShieldType.Shield1 => new CivilShield1(),
            _ => new NullShield()
        },
        ShipType.WarShip => shieldType switch
        {
            ShieldType.Shield1 => new WarShield1(),
            ShieldType.Shield2 => new WarShield2(),
            ShieldType.Shield3 => new WarShield3(),
            _ => new NullShield()
        },
        ShipType.FlagShip => shieldType switch
        {
            ShieldType.Shield1 => new FlagShield1(),
            ShieldType.Shield2 => new FlagShield2(),
            ShieldType.Shield3 => new FlagShield3(),
            _ => new NullShield()
        },
        _ => new NullShield()
    };
    public static IWeapon FindIWeapon(ShipType shipType, WeaponType weaponType) => shipType switch
    {
        ShipType.CivilShip => weaponType switch
        {
            WeaponType.LaserGun => new CivilLaserGun(),
            _ => new NullWeapon()
        },
        ShipType.WarShip => weaponType switch
        {
            WeaponType.LaserGun => new WarLaserGun(),
            WeaponType.PlasmaGun => new WarPlasmaGun(),
            WeaponType.ShellGun => new WarShellGun(),
            WeaponType.MissileGun => new WarMissileGun(),
            WeaponType.ArcGun => new WarArcGun(),
            _ => new NullWeapon()
        },
        ShipType.FlagShip => weaponType switch
        {
            WeaponType.LaserGun => new FlagLaserGun(),
            WeaponType.PlasmaGun => new FlagPlasmaGun(),
            WeaponType.ShellGun => new FlagShellGun(),
            WeaponType.MissileGun => new FlagMissileGun(),
            WeaponType.ArcGun => new FlagArcGun(),
            _ => new NullWeapon()
        },
        _ => new NullWeapon()
    };
}