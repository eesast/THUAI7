using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Threading;

namespace GameClass.GameObj.Bullets;

internal sealed class Laser : Bullet
{
    public Laser(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        this.AP.SetReturnOri(GameData.LaserDamage);
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.LaserRange;
    public override int Speed => GameData.LaserSpeed;
    public override int CastTime => GameData.LaserCastTime;
    public override int SwingTime => GameData.LaserSwingTime;
    private const int cd = GameData.LaserSwingTime;
    public override int CD => cd;
    public override int MaxBulletNum => 1;
    public override BulletType TypeOfBullet => BulletType.Laser;
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