using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagLaserGun : IWeapon
{
    public int Cost => GameData.FlagShipLaserGunCost;
    public BulletType BulletType => BulletType.Laser;
}
public class FlagPlasmaGun : IWeapon
{
    public int Cost => GameData.FlagShipPlasmaGunCost;
    public BulletType BulletType => BulletType.Plasma;
}
public class FlagShellGun : IWeapon
{
    public int Cost => GameData.FlagShipShellGunCost;
    public BulletType BulletType => BulletType.Shell;
}
public class FlagMissileGun : IWeapon
{
    public int Cost => GameData.FlagShipMissileGunCost;
    public BulletType BulletType => BulletType.Missile;
}
public class FlagArcGun : IWeapon
{
    public int Cost => GameData.FlagShipArcGunCost;
    public BulletType BulletType => BulletType.Arc;
}