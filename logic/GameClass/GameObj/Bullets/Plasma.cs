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
    private const int cd = GameData.PlasmaSwingTime;
    public override int CD => cd;
    public const int maxBulletNum = 1;
    public override int MaxBulletNum => maxBulletNum;
    public override BulletType TypeOfBullet => BulletType.Plasma;
    public override bool CanAttack(GameObj target)
    {
        //if (target.Type == GameObjType.Ship
        //    || target.Type == GameObjType.Construction
        //    || target.Type == GameObjType.Wormhole
        //    || target.Type == GameObjType.Home)
        //    return true;
        return false;
    }
    public override bool CanBeBombed(GameObjType gameObjType)
    {
        return false;
    }
}