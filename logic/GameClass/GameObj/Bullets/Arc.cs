using Preparation.Utility;
using System;

namespace GameClass.GameObj.Bullets;

internal sealed class Arc : Bullet
{
    public Arc(Ship ship, XY pos, int radius = GameData.BulletRadius) :
        base(ship, radius, pos)
    {
        Random random = new();
        this.AP.SetReturnOri(random.Next(GameData.ArcDamageMin, GameData.ArcDamageMax));
    }
    public override double BulletBombRange => 0;
    public override double AttackDistance => GameData.ArcRange;
    public override int Speed => GameData.ArcSpeed;
    public override int CastTime => GameData.ArcCastTime;
    public override int SwingTime => GameData.ArcSwingTime;
    private const int cd = GameData.ArcSwingTime;
    public override int CD => cd;
    public const int maxBulletNum = 1;
    public override int MaxBulletNum => maxBulletNum;
    public override BulletType TypeOfBullet => BulletType.Arc;
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