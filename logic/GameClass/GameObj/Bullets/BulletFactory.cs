using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj.Bullets;

public static class BulletFactory
{
    public static Bullet? GetBullet(Ship ship, XY pos, BulletType bulletType) => bulletType switch
    {
        BulletType.Laser => new Laser(ship, pos),
        BulletType.Plasma => new Plasma(ship, pos),
        BulletType.Shell => new Shell(ship, pos),
        BulletType.Missile => new Missile(ship, pos),
        BulletType.Arc => new Arc(ship, pos),
        _ => new NullBullet(ship, pos)
    };
    public static Bullet? GetBullet(Ship ship, XY pos, WeaponType weaponType) => weaponType switch
    {
        WeaponType.LaserGun => new Laser(ship, pos),
        WeaponType.PlasmaGun => new Plasma(ship, pos),
        WeaponType.ShellGun => new Shell(ship, pos),
        WeaponType.MissileGun => new Missile(ship, pos),
        WeaponType.ArcGun => new Arc(ship, pos),
        _ => new NullBullet(ship, pos)
    };
}