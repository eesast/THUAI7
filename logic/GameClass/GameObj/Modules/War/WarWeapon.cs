using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class WarLaserGun : IWeapon
{
    public int Cost => GameData.WarShipLaserGunCost;
    public BulletType BulletType => BulletType.Laser;
    public WeaponType WeaponModuleType => WeaponType.LaserGun;
}
public class WarPlasmaGun : IWeapon
{
    public int Cost => GameData.WarShipPlasmaGunCost;
    public BulletType BulletType => BulletType.Plasma;
    public WeaponType WeaponModuleType => WeaponType.PlasmaGun;
}
public class WarShellGun : IWeapon
{
    public int Cost => GameData.WarShipShellGunCost;
    public BulletType BulletType => BulletType.Shell;
    public WeaponType WeaponModuleType => WeaponType.ShellGun;
}
public class WarMissileGun : IWeapon
{
    public int Cost => GameData.WarShipMissileGunCost;
    public BulletType BulletType => BulletType.Missile;
    public WeaponType WeaponModuleType => WeaponType.MissileGun;
}
public class WarArcGun : IWeapon
{
    public int Cost => GameData.WarShipArcGunCost;
    public BulletType BulletType => BulletType.Arc;
    public WeaponType WeaponModuleType => WeaponType.ArcGun;
}