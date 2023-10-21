using Preparation.Interface;
using Preparation.Utility;
using System;
using System.Threading;

namespace GameClass.GameObj
{
    public abstract class Bullet : ObjOfShip
    {
        public abstract double BulletBombRange { get; }
        public abstract double AttackDistance { get; }
        private AtomicInt ap = new(0);
        public AtomicInt AP { get => ap; }
        public abstract int Speed { get; }
        public abstract int CastTime { get; }
        public abstract int SwingTime { get; }
        public abstract int CD { get; }
        public abstract int MaxBulletNum { get; }
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
    public static class BulletFactory
    {
        public static Bullet? GetBullet(Ship ship, XY pos, BulletType bulletType)
        {
            switch (bulletType)
            {
                case BulletType.Laser:
                    return new Laser(ship, pos);
                case BulletType.Plasma:
                    return new Plasma(ship, pos);
                case BulletType.Shell:
                    return new Shell(ship, pos);
                case BulletType.Missile:
                    return new Missile(ship, pos);
                case BulletType.Arc:
                    return new Arc(ship, pos);
                default:
                    return null;
            }
        }
    }
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
    internal sealed class Shell : Bullet
    {
        public Shell(Ship ship, XY pos, int radius = GameData.BulletRadius) :
            base(ship, radius, pos)
        {
            this.AP.SetReturnOri(GameData.ShellDamage);
        }
        public override double BulletBombRange => 0;
        public override double AttackDistance => GameData.ShellRange;
        public override int Speed => GameData.ShellSpeed;
        public override int CastTime => GameData.ShellCastTime;
        public override int SwingTime => GameData.ShellSwingTime;
        private const int cd = GameData.ShellSwingTime;
        public override int CD => cd;
        public const int maxBulletNum = 1;
        public override int MaxBulletNum => maxBulletNum;
        public override BulletType TypeOfBullet => BulletType.Shell;
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
    internal sealed class Arc : Bullet
    {
        public Arc(Ship ship, XY pos, int radius = GameData.BulletRadius) :
            base(ship, radius, pos)
        {
            Random random = new Random();
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
}
