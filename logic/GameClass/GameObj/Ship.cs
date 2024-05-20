using GameClass.GameObj.Bullets;
using GameClass.GameObj.Modules;
using GameClass.GameObj.Occupations;
using Preparation.Interface;
using Preparation.Utility;
using Preparation.Utility.Value;
using Preparation.Utility.Value.SafeValue.Atomic;
using Preparation.Utility.Value.SafeValue.LockedValue;
using Preparation.Utility.Value.SafeValue.TimeBased;

namespace GameClass.GameObj;

public class Ship : Movable, IShip
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public AtomicLong PlayerID { get; } = new(long.MaxValue);
    public override bool IsRigid(bool args = false) => true;
    public override ShapeType Shape => ShapeType.Circle;
    public int ViewRange { get; }
    public override bool IgnoreCollideExecutor(IGameObj targetObj)
    {
        if (IsRemoved)
            return true;
        if (targetObj.Type == GameObjType.Ship
         && XY.DistanceCeil3(targetObj.Position, Position)
            < Radius + targetObj.Radius - GameData.AdjustLength)
            return true;
        return false;
    }
    // 属性值
    public InVariableRange<long> HP { get; }
    public InVariableRange<long> Armor { get; }
    public InVariableRange<long> Shield { get; }
    public ShipType ShipType { get; }
    private ShipStateType shipState = ShipStateType.Deceased;
    public ShipStateType ShipState
    {
        get
        {
            lock (actionLock)
                return shipState;
        }
    }
    public IOccupation Occupation { get; }
    public MoneyPool MoneyPool { get; }
    /// <summary>
    /// 子弹数上限, THUAI7为无穷
    /// </summary>
    public IntNumUpdateEachCD BulletNum { get; } = new(int.MaxValue, 1);
    /// <summary>
    /// 模块相关
    /// </summary>
    #region Modules

    public AtomicTNotNull<IProducer> ProducerModule { get; } = new(NullProducer.Instance);
    public ProducerType ProducerModuleType => ProducerModule.Get().ProducerModuleType;

    public AtomicTNotNull<IConstructor> ConstructorModule { get; } = new(NullConstructor.Instance);
    public ConstructorType ConstructorModuleType => ConstructorModule.Get().ConstructorModuleType;

    public AtomicTNotNull<IArmor> ArmorModule { get; } = new(NullArmor.Instance);
    public ArmorType ArmorModuleType => ArmorModule.Get().ArmorModuleType;

    public AtomicTNotNull<IShield> ShieldModule { get; } = new(NullShield.Instance);
    public ShieldType ShieldModuleType => ShieldModule.Get().ShieldModuleType;

    public AtomicTNotNull<IWeapon> WeaponModule { get; } = new(NullWeapon.Instance);
    public WeaponType WeaponModuleType => WeaponModule.Get().WeaponModuleType;

    public Bullet? Attack(double angle)
    {
        lock (actionLock)
        {
            if (WeaponModuleType == WeaponType.Null) return null;
            if (BulletNum.TrySub(1) == 1)
            {
                XY res = Position + new XY(angle, Radius + GameData.BulletRadius);
                Bullet? bullet = BulletFactory.GetBullet(this, res, WeaponModuleType);
                if (bullet == null) return null;
                FacingDirection = new XY(angle, bullet.AttackDistance);
                return bullet;
            }
            return null;
        }
    }

    public int ProduceSpeed => ProducerModule.Get().ProduceSpeed;
    public int ConstructSpeed => ConstructorModule.Get().ConstructSpeed;
    public bool InstallModule(ModuleType moduleType)
    {
        if (moduleType == ModuleType.Null) return false;
        if (!Occupation.IsModuleValid(moduleType)) return false;
        if (MoneyPool.Money < ModuleFactory.FindModuleCost(ShipType, moduleType)) return false;

        switch (moduleType)
        {
            case ModuleType.Producer1:
                if (ProducerModuleType != ProducerType.Producer1)
                {
                    ProducerModule.SetROri(ModuleFactory.FindIProducer(ShipType, ProducerType.Producer1));
                    SubMoney(ProducerModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Producer2:
                if (ProducerModuleType != ProducerType.Producer2)
                {
                    ProducerModule.SetROri(ModuleFactory.FindIProducer(ShipType, ProducerType.Producer2));
                    SubMoney(ProducerModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Producer3:
                if (ProducerModuleType != ProducerType.Producer3)
                {
                    ProducerModule.SetROri(ModuleFactory.FindIProducer(ShipType, ProducerType.Producer3));
                    SubMoney(ProducerModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Constructor1:
                if (ConstructorModuleType != ConstructorType.Constructor1)
                {
                    ConstructorModule.SetROri(ModuleFactory.FindIConstructor(ShipType, ConstructorType.Constructor1));
                    SubMoney(ConstructorModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Constructor2:
                if (ConstructorModuleType != ConstructorType.Constructor2)
                {
                    ConstructorModule.SetROri(ModuleFactory.FindIConstructor(ShipType, ConstructorType.Constructor2));
                    SubMoney(ConstructorModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Constructor3:
                if (ConstructorModuleType != ConstructorType.Constructor3)
                {
                    ConstructorModule.SetROri(ModuleFactory.FindIConstructor(ShipType, ConstructorType.Constructor3));
                    SubMoney(ConstructorModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.Armor1:
                IArmor armor;
                lock (Armor.VLock)
                {
                    armor = ArmorModule.SetROri(ModuleFactory.FindIArmor(ShipType, ArmorType.Armor1));
                    Armor.SetRNow(armor.ArmorHP);
                }
                SubMoney(armor.Cost);
                return true;
            case ModuleType.Armor2:
                IArmor armor2;
                lock (Armor.VLock)
                {
                    armor2 = ArmorModule.SetROri(ModuleFactory.FindIArmor(ShipType, ArmorType.Armor2));
                    Armor.SetRNow(armor2.ArmorHP);
                }
                SubMoney(armor2.Cost);
                return true;

            case ModuleType.Armor3:
                IArmor armor3;
                lock (Armor.VLock)
                {
                    armor3 = ArmorModule.SetROri(ModuleFactory.FindIArmor(ShipType, ArmorType.Armor3));
                    Armor.SetRNow(armor3.ArmorHP);
                }
                SubMoney(armor3.Cost);
                return true;

            case ModuleType.Shield1:
                IShield shield1;
                lock (Shield.VLock)
                {
                    shield1 = ShieldModule.SetROri(ModuleFactory.FindIShield(ShipType, ShieldType.Shield1));
                    Shield.SetRNow(shield1.ShieldHP);
                }
                SubMoney(shield1.Cost);
                return true;

            case ModuleType.Shield2:
                IShield shield2;
                lock (Shield.VLock)
                {
                    shield2 = ShieldModule.SetROri(ModuleFactory.FindIShield(ShipType, ShieldType.Shield2));
                    Shield.SetRNow(shield2.ShieldHP);
                }
                SubMoney(shield2.Cost);
                return true;

            case ModuleType.Shield3:
                IShield shield3;
                lock (Shield.VLock)
                {
                    shield3 = ShieldModule.SetROri(ModuleFactory.FindIShield(ShipType, ShieldType.Shield3));
                    Shield.SetRNow(shield3.ShieldHP);
                }
                SubMoney(shield3.Cost);
                return true;

            case ModuleType.LaserGun:
                if (WeaponModuleType != WeaponType.LaserGun)
                {
                    WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, WeaponType.LaserGun));
                    SubMoney(WeaponModule.Get().Cost);
                    return true;
                }
                break;
            case ModuleType.PlasmaGun:
                if (WeaponModuleType != WeaponType.PlasmaGun)
                {
                    WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, WeaponType.PlasmaGun));
                    SubMoney(WeaponModule.Get().Cost);
                    return true;
                }
                break;

            case ModuleType.ShellGun:
                if (WeaponModuleType != WeaponType.ShellGun)
                {
                    WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, WeaponType.ShellGun));
                    SubMoney(WeaponModule.Get().Cost);
                    return true;
                }
                break;

            case ModuleType.MissileGun:
                if (WeaponModuleType != WeaponType.MissileGun)
                {
                    WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, WeaponType.MissileGun));
                    SubMoney(WeaponModule.Get().Cost);
                    return true;
                }
                break;

            case ModuleType.ArcGun:
                if (WeaponModuleType != WeaponType.ArcGun)
                {
                    WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, WeaponType.ArcGun));
                    SubMoney(WeaponModule.Get().Cost);
                    return true;
                }
                break;

            default:
                break;
        }
        return false;
    }

    #endregion

    private GameObj? whatInteractingWith = null;
    public GameObj? WhatInteractingWith
    {
        get
        {
            lock (actionLock)
            {
                return whatInteractingWith;
            }
        }
    }
    public long AddMoney(long add)
    {
        return MoneyPool.AddMoney(add);
    }
    public long SubMoney(long sub)
    {
        return MoneyPool.SubMoney(sub);
    }
    public long GetCost()
    {
        var cost = 0;
        switch (ShipType)
        {
            case ShipType.CivilShip:
                cost += GameData.CivilShipCost;
                break;
            case ShipType.WarShip:
                cost += GameData.WarShipCost;
                break;
            case ShipType.FlagShip:
                cost += GameData.FlagShipCost;
                break;
            default:
                return 0;
        }
        cost += ProducerModule.Get().Cost;
        cost += ConstructorModule.Get().Cost;
        cost += ArmorModule.Get().Cost;
        cost += ShieldModule.Get().Cost;
        cost += WeaponModule.Get().Cost;
        return cost;
    }
    private long ChangeShipState(RunningStateType running, ShipStateType value = ShipStateType.Null, GameObj? gameObj = null)
    {
        //只能被SetShipState引用
        if (runningState == RunningStateType.RunningSleepily)
        {
            ThreadNum.Release();
        }
        runningState = running;
        whatInteractingWith = gameObj;
        shipState = value;
        return ++stateNum;
    }
    private long ChangeShipStateInOneThread(RunningStateType running, ShipStateType value = ShipStateType.Null, GameObj? gameObj = null)
    {
        if (runningState == RunningStateType.RunningSleepily)
        {
            ThreadNum.Release();
        }
        runningState = running;
        //只能被SetPlayerState引用
        whatInteractingWith = gameObj;
        shipState = value;
        return stateNum;
    }
    public long SetShipState(RunningStateType running, ShipStateType value = ShipStateType.Null, IGameObj? obj = null)
    {
        GameObj? gameObj = (GameObj?)obj;
        lock (actionLock)
        {
            ShipStateType nowShipState = shipState;
            ShipLogging.logger.ConsoleLogDebug(
                LoggingFunctional.ShipLogInfo(this)
                + $" SetShipState from {nowShipState} to {value}");
            if (nowShipState == value) return -1;
            GameObj? lastObj = whatInteractingWith;
            switch (nowShipState)
            {
                case ShipStateType.Attacking:
                    if (value == ShipStateType.Null || value == ShipStateType.Deceased || value == ShipStateType.Stunned || value == ShipStateType.Swinging)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                case ShipStateType.Stunned:
                    if (value == ShipStateType.Null || value == ShipStateType.Deceased)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                case ShipStateType.Swinging:
                    if (value == ShipStateType.Null || value == ShipStateType.Deceased || value == ShipStateType.Stunned)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                case ShipStateType.Deceased:
                    if (value == ShipStateType.Null)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                default:
                    return ChangeShipState(running, value, gameObj);
            }
        }
    }
    public long SetShipStateNaturally()
    {
        lock (actionLock)
        {
            runningState = RunningStateType.Null;
            whatInteractingWith = null;
            shipState = ShipStateType.Null;
            return ++stateNum;
        }
    }
    public bool ResetShipState(long state, RunningStateType running = RunningStateType.Null, ShipStateType value = ShipStateType.Null, IGameObj? obj = null)
    {
        lock (actionLock)
        {
            if (state != stateNum)
            {
                ShipLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(this)
                    + " ResetShipState failed");
                return false;
            }
            runningState = running;
            whatInteractingWith = (GameObj?)obj;
            shipState = value;
            ++stateNum;
            ShipLogging.logger.ConsoleLogDebug(
                LoggingFunctional.ShipLogInfo(this)
                + $" ResetShipState succeeded {stateNum}");
            return true;
        }
    }
    public bool ResetShipStateInOneThread(long state, RunningStateType running = RunningStateType.Null, ShipStateType value = ShipStateType.Null, IGameObj? obj = null)
    {
        lock (actionLock)
        {
            if (state != stateNum) return false;
            runningState = running;
            whatInteractingWith = (GameObj?)obj;
            shipState = value;
            return true;
        }
    }
    public bool StartThread(long stateNum, RunningStateType runningState)
    {
        lock (actionLock)
        {
            if (StateNum == stateNum)
            {
                ShipLogging.logger.ConsoleLogDebug(
                    LoggingFunctional.ShipLogInfo(this)
                    + " StartThread succeeded");
                this.runningState = runningState;
                return true;
            }
        }
        ShipLogging.logger.ConsoleLogDebug(
            LoggingFunctional.ShipLogInfo(this)
            + " StartThread failed");
        return false;
    }
    public bool TryToRemoveFromGame(ShipStateType shipStateType)
    {
        lock (actionLock)
        {
            if (SetShipState(RunningStateType.RunningForcibly, shipStateType) == -1) return false;
            TryToRemove();
            CanMove.SetROri(false);
            position = GameData.PosNotInGame;
        }
        return true;
    }
    public bool Commandable()
    {
        lock (ActionLock)
        {
            return (shipState != ShipStateType.Stunned
                && shipState != ShipStateType.Swinging
                && shipState != ShipStateType.Attacking);
        }
    }
    public void Init()
    {
        HP.SetMaxV(Occupation.MaxHp);
        HP.SetVToMaxV();
        Armor.SetMaxV(Occupation.BaseArmor);
        Armor.SetVToMaxV();
        Shield.SetMaxV(Occupation.BaseShield);
        Shield.SetVToMaxV();
        MoveSpeed.SetROri(orgMoveSpeed = Occupation.MoveSpeed);
        var (producerType, constructorType, weaponType) = ShipType switch
        {
            ShipType.CivilShip => (
                ProducerType.Producer1,
                ConstructorType.Constructor1,
                WeaponType.Null
            ),
            ShipType.WarShip => (
                ProducerType.Null,
                ConstructorType.Null,
                WeaponType.LaserGun
            ),
            ShipType.FlagShip => (
                ProducerType.Null,
                ConstructorType.Null,
                WeaponType.LaserGun
            ),
            _ => (ProducerType.Null, ConstructorType.Null, WeaponType.Null)
        };
        ProducerModule.SetROri(ModuleFactory.FindIProducer(ShipType, producerType));
        ConstructorModule.SetROri(ModuleFactory.FindIConstructor(ShipType, constructorType));
        ArmorModule.SetROri(ModuleFactory.FindIArmor(ShipType, ArmorType.Null));
        ShieldModule.SetROri(ModuleFactory.FindIShield(ShipType, ShieldType.Null));
        WeaponModule.SetROri(ModuleFactory.FindIWeapon(ShipType, weaponType));
    }
    public Ship(int initRadius, ShipType shipType, MoneyPool moneyPool) :
        base(GameData.PosNotInGame, initRadius, GameObjType.Ship)
    {
        CanMove.SetROri(false);
        IsRemoved.SetROri(true);
        Occupation = OccupationFactory.FindIOccupation(ShipType = shipType);
        ViewRange = Occupation.ViewRange;
        HP = new(Occupation.MaxHp);
        Armor = new(Occupation.BaseArmor);
        Shield = new(Occupation.BaseShield);
        MoneyPool = moneyPool;
        Init();
    }
}
