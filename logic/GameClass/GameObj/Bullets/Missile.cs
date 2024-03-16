using System;
using Preparation.Utility;

namespace GameClass.GameObj.Bullets;

internal sealed class Missile : Bullet
{
    public Missile(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        AP.SetReturnOri(GameData.MissileDamage);
    }
    public override double BulletBombRange => GameData.MissileBombRange;
    public override double AttackDistance => GameData.MissileRange;
    public override int Speed => GameData.MissileSpeed;
    public override int CastTime => GameData.MissileCastTime;
    public override int SwingTime => GameData.ShellSwingTime;
    public override double ArmorModifier => GameData.MissileArmorModifier;
    public override double ShieldModifier => Double.MaxValue;
    public override BulletType TypeOfBullet => BulletType.Missile;
    public override bool CanAttack(GameObj target) =>
        XY.DistanceFloor3(target.Position, this.Position) <= GameData.MissileBombRange;
}