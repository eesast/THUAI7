using Preparation.Utility;

namespace GameClass.GameObj.Bullets;

internal sealed class Missile : Bullet
{
    public Missile(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        this.AP.SetReturnOri(GameData.MissileDamage);
    }
    public override double BulletBombRange => GameData.MissileBombRange;
    public override double AttackDistance => GameData.MissileRange;
    public override int Speed => GameData.MissileSpeed;
    public override int CastTime => GameData.MissileCastTime;
    public override int SwingTime => GameData.ShellSwingTime;
    private const int cd = GameData.ShellSwingTime;
    public override int CD => cd;
    public const int maxBulletNum = 1;
    public override int MaxBulletNum => maxBulletNum;
    public override BulletType TypeOfBullet => BulletType.Missile;
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
        //return true;
        return false;
    }
}