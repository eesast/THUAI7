using Preparation.Utility;
using Preparation.Utility.Value;
using System;

namespace GameClass.GameObj.Bullets;

internal sealed class Arc : Bullet
{
    public Arc(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        Random random = new();
        AP.SetROri(random.Next(GameData.ArcDamageMin, GameData.ArcDamageMax));
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.ArcRange;
    public override int Speed => GameData.ArcSpeed;
    public override int CastTime => GameData.ArcCastTime;
    public override int SwingTime => GameData.ArcSwingTime;
    public override double ArmorModifier => GameData.ArcArmorModifier;
    public override double ShieldModifier => GameData.ArcShieldModifier;
    public override BulletType TypeOfBullet => BulletType.Arc;
    public override bool CanAttack(GameObj target) => false;
}