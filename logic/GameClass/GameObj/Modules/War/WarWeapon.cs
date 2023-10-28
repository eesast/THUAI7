using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

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