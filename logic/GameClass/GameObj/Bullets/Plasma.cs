using Preparation.Utility;

namespace GameClass.GameObj.Bullets;

internal sealed class Plasma : Bullet
{
    public Plasma(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        this.AP.SetReturnOri(GameData.PlasmaDamage);
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.PlasmaRange;
    public override int Speed => GameData.PlasmaSpeed;
    public override int CastTime => GameData.PlasmaCastTime;
    public override int SwingTime => GameData.PlasmaSwingTime;
    public override double ArmorModifier => GameData.PlasmaArmorModifier;
    public override double ShieldModifier => GameData.PlasmaShieldModifier;
    public override BulletType TypeOfBullet => BulletType.Plasma;
    public override bool CanAttack(GameObj target) => false;
}