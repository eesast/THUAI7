using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

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