using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilLaserGun : IWeapon
{
    public int Cost => GameData.CivilShipLaserGunCost;
    public BulletType BulletType => BulletType.Laser;
    public WeaponType WeaponModuleType => WeaponType.LaserGun;
}