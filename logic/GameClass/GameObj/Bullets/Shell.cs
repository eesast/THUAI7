using Preparation.Utility;

namespace GameClass.GameObj.Bullets;

internal sealed class Shell : Bullet
{
    public Shell(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        AP.SetReturnOri(GameData.ShellDamage);
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.ShellRange;
    public override int Speed => GameData.ShellSpeed;
    public override int CastTime => GameData.ShellCastTime;
    public override int SwingTime => GameData.ShellSwingTime;
    public override double ArmorModifier => GameData.ShellArmorModifier;
    public override double ShieldModifier => GameData.ShellShieldModifier;
    public override BulletType TypeOfBullet => BulletType.Shell;
    public override bool CanAttack(GameObj target) => false;
}