using Preparation.Utility;

namespace Preparation.Interface
{
    public interface IModule
    {
        public int Cost { get; }
    }
    public interface IProducer : IModule
    {
        public int ProduceSpeed { get; }
    }
    public interface IConstructor : IModule
    {
        public int ConstructSpeed { get; }
    }
    public interface IArmor : IModule
    {
        public int ArmorHP { get; }
    }
    public interface IShield : IModule
    {
        public int ShieldHP { get; }
    }
    public interface IWeapon : IModule
    {
        public BulletType BulletType { get; }
    }
    public class CivilProducer1 : IProducer
    {
        private const int cost = GameData.CivilShipProducer1Cost;
        public int Cost => cost;
        private const int produceSpeed = GameData.ScoreProducer1PerSecond;
        public int ProduceSpeed => produceSpeed;
    }
    public class CivilProducer2 : IProducer
    {
        private const int cost = GameData.CivilShipProducer1Cost;
        public int Cost => cost;
        private const int produceSpeed = GameData.ScoreProducer2PerSecond;
        public int ProduceSpeed => produceSpeed;
    }
    public class CivilProducer3 : IProducer
    {
        private const int cost = GameData.CivilShipProducer1Cost;
        public int Cost => cost;
        private const int produceSpeed = GameData.ScoreProducer3PerSecond;
        public int ProduceSpeed => produceSpeed;
    }
    public class CivilConstructor1 : IConstructor
    {
        private const int cost = GameData.CivilShipConstructor1Cost;
        public int Cost => cost;
        private const int constructSpeed = GameData.Constructor1Speed;
        public int ConstructSpeed => constructSpeed;
    }
    public class CivilConstructor2 : IConstructor
    {
        private const int cost = GameData.CivilShipConstructor2Cost;
        public int Cost => cost;
        private const int constructSpeed = GameData.Constructor2Speed;
        public int ConstructSpeed => constructSpeed;
    }
    public class CivilConstructor3 : IConstructor
    {
        private const int cost = GameData.CivilShipConstructor3Cost;
        public int Cost => cost;
        private const int constructSpeed = GameData.Constructor2Speed;
        public int ConstructSpeed => constructSpeed;
    }
    public class CivilArmor1 : IArmor
    {
        private const int cost = GameData.CivilShipArmor1Cost;
        public int Cost => cost;
        private const int armorHP = GameData.Armor1;
        public int ArmorHP => armorHP;
    }
    public class CivilShield1 : IShield
    {
        private const int cost = GameData.CivilShipShield1Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield1;
        public int ShieldHP => shieldHP;
    }
    public class CivilLaserGun : IWeapon
    {
        private const int cost = GameData.CivilShipLaserGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Laser;
        public BulletType BulletType => bulletType;
    }
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
    public class WarShield1 : IShield
    {
        private const int cost = GameData.WarShipShield1Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield1;
        public int ShieldHP => shieldHP;
    }
    public class WarShield2 : IShield
    {
        private const int cost = GameData.WarShipShield2Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield2;
        public int ShieldHP => shieldHP;
    }
    public class WarShield3 : IShield
    {
        private const int cost = GameData.WarShipShield3Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield3;
        public int ShieldHP => shieldHP;
    }
    public class WarLaserGun : IWeapon
    {
        private const int cost = GameData.WarShipLaserGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Laser;
        public BulletType BulletType => bulletType;
    }
    public class WarPlasmaGun : IWeapon
    {
        private const int cost = GameData.WarShipPlasmaGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Plasma;
        public BulletType BulletType => bulletType;
    }
    public class WarShellGun : IWeapon
    {
        private const int cost = GameData.WarShipShellGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Shell;
        public BulletType BulletType => bulletType;
    }
    public class WarMissileGun : IWeapon
    {
        private const int cost = GameData.WarShipMissileGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Missile;
        public BulletType BulletType => bulletType;
    }
    public class WarArcGun : IWeapon
    {
        private const int cost = GameData.WarShipArcGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Arc;
        public BulletType BulletType => bulletType;
    }
    public class FlagProducer1 : IProducer
    {
        private const int cost = GameData.FlagShipProducer1Cost;
        public int Cost => cost;
        private const int produceSpeed = GameData.ScoreProducer1PerSecond;
        public int ProduceSpeed => produceSpeed;
    }
    public class FlagConstructor1 : IConstructor
    {
        private const int cost = GameData.FlagShipConstructor1Cost;
        public int Cost => cost;
        private const int constructSpeed = GameData.Constructor1Speed;
        public int ConstructSpeed => constructSpeed;
    }
    public class FlagArmor1 : IArmor
    {
        private const int cost = GameData.FlagShipArmor1Cost;
        public int Cost => cost;
        private const int armorHP = GameData.Armor1;
        public int ArmorHP => armorHP;
    }
    public class FlagArmor2 : IArmor
    {
        private const int cost = GameData.FlagShipArmor2Cost;
        public int Cost => cost;
        private const int armorHP = GameData.Armor2;
        public int ArmorHP => armorHP;
    }
    public class FlagArmor3 : IArmor
    {
        private const int cost = GameData.FlagShipArmor3Cost;
        public int Cost => cost;
        private const int armorHP = GameData.Armor3;
        public int ArmorHP => armorHP;
    }
    public class FlagShield1 : IShield
    {
        private const int cost = GameData.FlagShipShield1Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield1;
        public int ShieldHP => shieldHP;
    }
    public class FlagShield2 : IShield
    {
        private const int cost = GameData.FlagShipShield2Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield2;
        public int ShieldHP => shieldHP;
    }
    public class FlagShield3 : IShield
    {
        private const int cost = GameData.FlagShipShield3Cost;
        public int Cost => cost;
        private const int shieldHP = GameData.Shield3;
        public int ShieldHP => shieldHP;
    }
    public class FlagLaserGun : IWeapon
    {
        private const int cost = GameData.FlagShipLaserGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Laser;
        public BulletType BulletType => bulletType;
    }
    public class FlagPlasmaGun : IWeapon
    {
        private const int cost = GameData.FlagShipPlasmaGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Plasma;
        public BulletType BulletType => bulletType;
    }
    public class FlagShellGun : IWeapon
    {
        private const int cost = GameData.FlagShipShellGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Shell;
        public BulletType BulletType => bulletType;
    }
    public class FlagMissileGun : IWeapon
    {
        private const int cost = GameData.FlagShipMissileGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Missile;
        public BulletType BulletType => bulletType;
    }
    public class FlagArcGun : IWeapon
    {
        private const int cost = GameData.FlagShipArcGunCost;
        public int Cost => cost;
        private const BulletType bulletType = BulletType.Arc;
        public BulletType BulletType => bulletType;
    }
    public static class ModuleFactory
    {
        public static IProducer FindIProducer(ShipType shipType, ProducerType producerType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    switch (producerType)
                    {
                        case ProducerType.Producer1:
                            return new CivilProducer1();
                        case ProducerType.Producer2:
                            return new CivilProducer2();
                        case ProducerType.Producer3:
                            return new CivilProducer3();
                        default:
                            return new CivilProducer1();
                    }
                case ShipType.FlagShip:
                    switch (producerType)
                    {
                        case ProducerType.Producer1:
                            return new FlagProducer1();
                        default:
                            return new FlagProducer1();
                    }
                default:
                    return new CivilProducer1();
            }
        }
        public static IConstructor FindIConstructor(ShipType shipType, ConstructorType constructorType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    switch (constructorType)
                    {
                        case ConstructorType.Constructor1:
                            return new CivilConstructor1();
                        case ConstructorType.Constructor2:
                            return new CivilConstructor2();
                        case ConstructorType.Constructor3:
                            return new CivilConstructor3();
                        default:
                            return new CivilConstructor1();
                    }
                case ShipType.FlagShip:
                    switch (constructorType)
                    {
                        case ConstructorType.Constructor1:
                            return new FlagConstructor1();
                        default:
                            return new FlagConstructor1();
                    }
                default:
                    return new CivilConstructor1();
            }
        }
        public static IArmor FindIArmor(ShipType shipType, ArmorType armorType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    switch (armorType)
                    {
                        case ArmorType.Armor1:
                            return new CivilArmor1();
                        default:
                            return new CivilArmor1();
                    }
                case ShipType.WarShip:
                    switch (armorType)
                    {
                        case ArmorType.Armor1:
                            return new WarArmor1();
                        case ArmorType.Armor2:
                            return new WarArmor2();
                        case ArmorType.Armor3:
                            return new WarArmor3();
                        default:
                            return new WarArmor1();
                    }
                case ShipType.FlagShip:
                    switch (armorType)
                    {
                        case ArmorType.Armor1:
                            return new FlagArmor1();
                        case ArmorType.Armor2:
                            return new FlagArmor2();
                        case ArmorType.Armor3:
                            return new FlagArmor3();
                        default:
                            return new FlagArmor1();
                    }
                default:
                    return new CivilArmor1();
            }
        }
        public static IShield FindIShield(ShipType shipType, ShieldType shieldType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    switch (shieldType)
                    {
                        case ShieldType.Shield1:
                            return new CivilShield1();
                        default:
                            return new CivilShield1();
                    }
                case ShipType.WarShip:
                    switch (shieldType)
                    {
                        case ShieldType.Shield1:
                            return new WarShield1();
                        case ShieldType.Shield2:
                            return new WarShield2();
                        case ShieldType.Shield3:
                            return new WarShield3();
                        default:
                            return new WarShield1();
                    }
                case ShipType.FlagShip:
                    switch (shieldType)
                    {
                        case ShieldType.Shield1:
                            return new FlagShield1();
                        case ShieldType.Shield2:
                            return new FlagShield2();
                        case ShieldType.Shield3:
                            return new FlagShield3();
                        default:
                            return new FlagShield1();
                    }
                default:
                    return new CivilShield1();
            }
        }
        public static IWeapon FindIWeapon(ShipType shipType, WeaponType weaponType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    switch (weaponType)
                    {
                        case WeaponType.LaserGun:
                            return new CivilLaserGun();
                        default:
                            return new CivilLaserGun();
                    }
                case ShipType.WarShip:
                    switch (weaponType)
                    {
                        case WeaponType.LaserGun:
                            return new WarLaserGun();
                        case WeaponType.PlasmaGun:
                            return new WarPlasmaGun();
                        case WeaponType.ShellGun:
                            return new WarShellGun();
                        case WeaponType.MissileGun:
                            return new WarMissileGun();
                        case WeaponType.ArcGun:
                            return new WarArcGun();
                        default:
                            return new WarLaserGun();
                    }
                case ShipType.FlagShip:
                    switch (weaponType)
                    {
                        case WeaponType.LaserGun:
                            return new FlagLaserGun();
                        case WeaponType.PlasmaGun:
                            return new FlagPlasmaGun();
                        case WeaponType.ShellGun:
                            return new FlagShellGun();
                        case WeaponType.MissileGun:
                            return new FlagMissileGun();
                        case WeaponType.ArcGun:
                            return new FlagArcGun();
                        default:
                            return new FlagLaserGun();
                    }
                default:
                    return new CivilLaserGun();
            }
        }
    }
}
