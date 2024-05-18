using Preparation.Utility;
using Preparation.Utility.Value;

namespace GameClass.GameObj
{
    // 为方便界面组做子弹爆炸特效，现引入“爆炸中的子弹”，在每帧发送给界面组
    public sealed class BombedBullet(Bullet bullet)
        : Immovable(bullet.Position, bullet.Radius, GameObjType.BombedBullet)
    {
        public override ShapeType Shape => ShapeType.Circle;
        public override bool IsRigid(bool args = false) => false;
        public long MappingID { get; } = bullet.ID;
        public readonly Bullet bulletHasBombed = bullet;
        public readonly XY facingDirection = bullet.FacingDirection;
    }
}
