using Preparation.Utility;

namespace GameClass.GameObj.Bullets;

internal sealed class Laser : Bullet
{
    public Laser(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        AP.SetROri(GameData.LaserDamage);
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.LaserRange;
    public override int Speed => GameData.LaserSpeed;
    public override int CastTime => GameData.LaserCastTime;
    public override int SwingTime => GameData.LaserSwingTime;
    public override double ArmorModifier => GameData.LaserArmorModifier;
    public override double ShieldModifier => GameData.LaserShieldModifier;
    public override BulletType TypeOfBullet => BulletType.Laser;
    public override bool CanAttack(GameObj target) => false;
}