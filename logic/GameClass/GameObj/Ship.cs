using GameClass.GameObj.Bullets;
using GameClass.GameObj.Modules;
using GameClass.GameObj.Occupations;
using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj;

public class Ship : Movable, IShip
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public AtomicLong PlayerID { get; } = new(long.MaxValue);
    public override bool IsRigid => true;
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
    private ShipStateType shipState = ShipStateType.Null;
    public ShipStateType ShipState => shipState;
    public IOccupation Occupation { get; }
    public MoneyPool MoneyPool { get; }
    /// <summary>
    /// 子弹数上限, THUAI7为无穷
    /// </summary>
    public IntNumUpdateEachCD BulletNum => new(int.MaxValue, 1);
    /// <summary>
    /// 模块相关
    /// </summary>
    #region Modules

    #region Producer
    private ProducerType producerType = ProducerType.Null;
    public ProducerType ProducerModuleType => producerType;
    private IProducer producer;
    public IProducer ProducerModule => producer;
    #endregion

    #region Constructor
    private ConstructorType constructorType = ConstructorType.Null;
    public ConstructorType ConstructorModuleType => constructorType;
    private IConstructor constructor;
    public IConstructor ConstructorModule => constructor;
    #endregion

    #region Armor
    private ArmorType armorType = ArmorType.Null;
    public ArmorType ArmorModuleType => armorType;
    private IArmor armor;
    public IArmor ArmorModule => armor;
    #endregion

    #region Shield
    private ShieldType shieldType = ShieldType.Null;
    public ShieldType ShieldModuleType => shieldType;
    private IShield shield;
    public IShield ShieldModule => shield;
    #endregion

    #region Weapon
    private WeaponType weaponType = WeaponType.Null;
    public WeaponType WeaponModuleType => weaponType;
    private IWeapon weapon;
    public IWeapon WeaponModule => weapon;
    public Bullet? Attack(double angle)
    {
        lock (actionLock)
        {
            if (weaponType == WeaponType.Null) return null;
            if (BulletNum.TrySub(1) == 1)
            {
                XY res = Position + new XY(angle, Radius + GameData.BulletRadius);
                Bullet? bullet = BulletFactory.GetBullet(this, res, weaponType);
                if (bullet == null) return null;
                FacingDirection = new XY(angle, bullet.AttackDistance);
                return bullet;
            }
            return null;
        }
    }
    #endregion

    public int ProduceSpeed => producer.ProduceSpeed;
    public int ConstructSpeed => constructor.ConstructSpeed;
    public bool InstallModule(ModuleType moduleType)
    {
        lock (actionLock)
        {
            if (moduleType == ModuleType.Null) return false;
            if (!Occupation.IsModuleValid(moduleType)) return false;
            if (MoneyPool.Money < ModuleFactory.FindModuleCost(ShipType, moduleType)) return false;
            switch (moduleType)
            {
                case ModuleType.Producer1:
                    if (producerType != ProducerType.Producer1)
                    {
                        producerType = ProducerType.Producer1;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        SubMoney(producer.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Producer2:
                    if (producerType != ProducerType.Producer2)
                    {
                        producerType = ProducerType.Producer2;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        SubMoney(producer.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Producer3:
                    if (producerType != ProducerType.Producer3)
                    {
                        producerType = ProducerType.Producer3;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        SubMoney(producer.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Constructor1:
                    if (constructorType != ConstructorType.Constructor1)
                    {
                        constructorType = ConstructorType.Constructor1;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        SubMoney(constructor.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Constructor2:
                    if (constructorType != ConstructorType.Constructor2)
                    {
                        constructorType = ConstructorType.Constructor2;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        SubMoney(constructor.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Constructor3:
                    if (constructorType != ConstructorType.Constructor3)
                    {
                        constructorType = ConstructorType.Constructor3;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        SubMoney(constructor.Cost);
                        return true;
                    }
                    break;
                case ModuleType.Armor1:
                    armorType = ArmorType.Armor1;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    Armor.SetRNow(armor.ArmorHP);
                    SubMoney(armor.Cost);
                    return true;
                case ModuleType.Armor2:
                    armorType = ArmorType.Armor2;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    Armor.SetRNow(armor.ArmorHP);
                    SubMoney(armor.Cost);
                    return true;
                case ModuleType.Armor3:
                    armorType = ArmorType.Armor3;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    Armor.SetRNow(armor.ArmorHP);
                    SubMoney(armor.Cost);
                    return true;
                case ModuleType.Shield1:
                    shieldType = ShieldType.Shield1;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    Shield.SetRNow(shield.ShieldHP);
                    SubMoney(shield.Cost);
                    return true;
                case ModuleType.Shield2:
                    shieldType = ShieldType.Shield2;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    Shield.SetRNow(shield.ShieldHP);
                    SubMoney(shield.Cost);
                    return true;
                case ModuleType.Shield3:
                    shieldType = ShieldType.Shield3;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    Shield.SetRNow(shield.ShieldHP);
                    SubMoney(shield.Cost);
                    return true;
                case ModuleType.LaserGun:
                    if (weaponType != WeaponType.LaserGun)
                    {
                        weaponType = WeaponType.LaserGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        SubMoney(weapon.Cost);
                        return true;
                    }
                    break;
                case ModuleType.PlasmaGun:
                    if (weaponType != WeaponType.PlasmaGun)
                    {
                        weaponType = WeaponType.PlasmaGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        SubMoney(weapon.Cost);
                        return true;
                    }
                    break;
                case ModuleType.ShellGun:
                    if (weaponType != WeaponType.ShellGun)
                    {
                        weaponType = WeaponType.ShellGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        SubMoney(weapon.Cost);
                        return true;
                    }
                    break;
                case ModuleType.MissileGun:
                    if (weaponType != WeaponType.MissileGun)
                    {
                        weaponType = WeaponType.MissileGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        SubMoney(weapon.Cost);
                        return true;
                    }
                    break;
                case ModuleType.ArcGun:
                    if (weaponType != WeaponType.ArcGun)
                    {
                        weaponType = WeaponType.ArcGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        SubMoney(weapon.Cost);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
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
        cost += producer.Cost;
        cost += constructor.Cost;
        cost += armor.Cost;
        cost += shield.Cost;
        cost += weapon.Cost;
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
            ShipStateType nowShipState = ShipState;
            if (nowShipState == value) return -1;
            GameObj? lastObj = whatInteractingWith;
            switch (nowShipState)
            {
                case ShipStateType.Attacking:
                    if (value == ShipStateType.Null || value == ShipStateType.Stunned || value == ShipStateType.Swinging)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                case ShipStateType.Stunned:
                    if (value == ShipStateType.Null)
                        return ChangeShipState(running, value, gameObj);
                    else return -1;
                case ShipStateType.Swinging:
                    if (value == ShipStateType.Null || value == ShipStateType.Stunned)
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
            if (state != stateNum) return false;
            runningState = running;
            whatInteractingWith = (GameObj?)obj;
            shipState = value;
            ++stateNum;
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
                this.runningState = runningState;
                return true;
            }
        }
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
        MoveSpeed.SetROri(orgMoveSpeed = Occupation.MoveSpeed);
        MoneyPool = moneyPool;
        (producerType, constructorType, armorType, shieldType, weaponType) = ShipType switch
        {
            ShipType.CivilShip => (
                ProducerType.Producer1,
                ConstructorType.Constructor1,
                ArmorType.Null,
                ShieldType.Null,
                WeaponType.Null
            ),
            ShipType.WarShip => (
                ProducerType.Null,
                ConstructorType.Null,
                ArmorType.Null,
                ShieldType.Null,
                WeaponType.LaserGun
            ),
            ShipType.FlagShip => (
                ProducerType.Null,
                ConstructorType.Null,
                ArmorType.Null,
                ShieldType.Null,
                WeaponType.LaserGun
            ),
            _ => (ProducerType.Null, ConstructorType.Null, ArmorType.Null, ShieldType.Null, WeaponType.Null)
        };
        (producer, constructor, armor, shield, weapon) = (
            ModuleFactory.FindIProducer(ShipType, producerType),
            ModuleFactory.FindIConstructor(ShipType, constructorType),
            ModuleFactory.FindIArmor(ShipType, armorType),
            ModuleFactory.FindIShield(ShipType, shieldType),
            ModuleFactory.FindIWeapon(ShipType, weaponType)
        );
    }
}
