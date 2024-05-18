using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue.Atomic;

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
    public override bool IsRigid(bool args = false) => true;                 // 默认为true
    public override ShapeType Shape => ShapeType.Circle;  // 默认为圆形
    public abstract BulletType TypeOfBullet { get; }
    public abstract bool CanAttack(GameObj target);
    public virtual bool CanBeBombed(GameObjType gameObjType) => gameObjType switch
    {
        GameObjType.Ship => true,
        GameObjType.Construction => true,
        GameObjType.Wormhole => true,
        GameObjType.Home => true,
        _ => false
    };
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
        CanMove.SetROri(true);
        MoveSpeed.SetROri(Speed);
        Parent = ship;
    }
}
