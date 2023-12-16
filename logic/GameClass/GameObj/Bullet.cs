using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj;

public abstract class Bullet : ObjOfShip
{
    public abstract double BulletBombRange { get; }
    public abstract double AttackDistance { get; }
    public AtomicInt AP { get; } = new(0);
    public abstract int Speed { get; }
    public abstract int CastTime { get; }
    public abstract int SwingTime { get; }
    public abstract double ArmorModifier { get; }
    public abstract double ShieldModifier { get; }
    public override bool IsRigid => true;                 // 默认为true
    public override ShapeType Shape => ShapeType.Circle;  // 默认为圆形
    public abstract BulletType TypeOfBullet { get; }
    public abstract bool CanAttack(GameObj target);
    public abstract bool CanBeBombed(GameObjType gameObjType);
    public override bool IgnoreCollideExecutor(IGameObj targetObj)
    {
        if (targetObj == Parent) return true;
        if (targetObj.Type == GameObjType.Bullet)
            return true;
        return false;
    }
    public Bullet(Ship ship, int radius, XY Position) :
        base(Position, radius, GameObjType.Bullet)
    {
        this.CanMove.SetReturnOri(true);
        this.MoveSpeed.SetReturnOri(this.Speed);
        this.Parent = ship;
    }
}
