using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilLaserGun : IWeapon
{
    private const int cost = GameData.CivilShipLaserGunCost;
    public int Cost => cost;
    private const BulletType bulletType = BulletType.Laser;
    public BulletType BulletType => bulletType;
}