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
    public override double ArmorModifier => GameData.LaserArmorModifier;
    public override double ShieldModifier => GameData.LaserShieldModifier;
    public override BulletType TypeOfBullet => BulletType.Laser;
    public override bool CanAttack(GameObj target) => false;
    public override bool CanBeBombed(GameObjType gameObjType) => gameObjType switch
    {
        GameObjType.Ship => true,
        GameObjType.Construction => true,
        GameObjType.Wormhole => true,
        GameObjType.Home => true,
        _ => false
    };
}