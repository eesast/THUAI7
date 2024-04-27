using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj.Bullets;

internal sealed class NullBullet(Ship ship, XY Position, int radius = GameData.BulletRadius)
    : Bullet(ship, radius, Position)
{
    public override double BulletBombRange => 0;
    public override double AttackDistance => 0;
    public override int Speed => 0;
    public override int CastTime => 0;
    public override int SwingTime => 0;
    public override double ArmorModifier => 0;
    public override double ShieldModifier => 0;
    public override BulletType TypeOfBullet => BulletType.Null;
    public override bool CanAttack(GameObj target) => false;
    public override bool CanBeBombed(GameObjType gameObjType) => false;
}
