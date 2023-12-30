using System;
using GameClass.GameObj.Bullets;
using Preparation.Interface;
using Preparation.Utility;
using GameClass.GameObj.Modules;
using GameClass.GameObj.Occupations;

namespace GameClass.GameObj;

public class Ship : Movable, IShip
{
    public AtomicLong TeamID { get; } = new(long.MaxValue);
    public AtomicLong ShipID { get; } = new(long.MaxValue);
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Circle;
    public int ViewRange { get; }
    public override bool IgnoreCollideExecutor(IGameObj targetObj)
    {
        if (IsRemoved)
            return true;
        if (targetObj.Type == GameObjType.Ship && XY.DistanceCeil3(targetObj.Position, this.Position) < this.Radius + targetObj.Radius - GameData.AdjustLength)
            return true;
        return false;
    }
    // 属性值
    public LongInTheVariableRange HP { get; }
    public LongInTheVariableRange Armor { get; }
    public LongInTheVariableRange Shield { get; }
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
            if (!this.Occupation.IsModuleValid(moduleType)) return false;
            switch (moduleType)
            {
                case ModuleType.Producer1:
                    if (producerType != ProducerType.Producer1)
                    {
                        producerType = ProducerType.Producer1;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        return true;
                    }
                    break;
                case ModuleType.Producer2:
                    if (producerType != ProducerType.Producer2)
                    {
                        producerType = ProducerType.Producer2;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        return true;
                    }
                    break;
                case ModuleType.Producer3:
                    if (producerType != ProducerType.Producer3)
                    {
                        producerType = ProducerType.Producer3;
                        producer = ModuleFactory.FindIProducer(ShipType, producerType);
                        return true;
                    }
                    break;
                case ModuleType.Constructor1:
                    if (constructorType != ConstructorType.Constructor1)
                    {
                        constructorType = ConstructorType.Constructor1;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        return true;
                    }
                    break;
                case ModuleType.Constructor2:
                    if (constructorType != ConstructorType.Constructor2)
                    {
                        constructorType = ConstructorType.Constructor2;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        return true;
                    }
                    break;
                case ModuleType.Constructor3:
                    if (constructorType != ConstructorType.Constructor3)
                    {
                        constructorType = ConstructorType.Constructor3;
                        constructor = ModuleFactory.FindIConstructor(ShipType, constructorType);
                        return true;
                    }
                    break;
                case ModuleType.Armor1:
                    armorType = ArmorType.Armor1;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    this.Armor.SetV(armor.ArmorHP);
                    return true;
                case ModuleType.Armor2:
                    armorType = ArmorType.Armor2;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    this.Armor.SetV(armor.ArmorHP);
                    return true;
                case ModuleType.Armor3:
                    armorType = ArmorType.Armor3;
                    armor = ModuleFactory.FindIArmor(ShipType, armorType);
                    this.Armor.SetV(armor.ArmorHP);
                    return true;
                case ModuleType.Shield1:
                    shieldType = ShieldType.Shield1;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    this.Shield.SetV(shield.ShieldHP);
                    return true;
                case ModuleType.Shield2:
                    shieldType = ShieldType.Shield2;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    this.Shield.SetV(shield.ShieldHP);
                    return true;
                case ModuleType.Shield3:
                    shieldType = ShieldType.Shield3;
                    shield = ModuleFactory.FindIShield(ShipType, shieldType);
                    this.Shield.SetV(shield.ShieldHP);
                    return true;
                case ModuleType.LaserGun:
                    if (weaponType != WeaponType.LaserGun)
                    {
                        weaponType = WeaponType.LaserGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        return true;
                    }
                    break;
                case ModuleType.PlasmaGun:
                    if (weaponType != WeaponType.PlasmaGun)
                    {
                        weaponType = WeaponType.PlasmaGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        return true;
                    }
                    break;
                case ModuleType.ShellGun:
                    if (weaponType != WeaponType.ShellGun)
                    {
                        weaponType = WeaponType.ShellGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        return true;
                    }
                    break;
                case ModuleType.MissileGun:
                    if (weaponType != WeaponType.MissileGun)
                    {
                        weaponType = WeaponType.MissileGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
                        return true;
                    }
                    break;
                case ModuleType.ArcGun:
                    if (weaponType != WeaponType.ArcGun)
                    {
                        weaponType = WeaponType.ArcGun;
                        weapon = ModuleFactory.FindIWeapon(ShipType, weaponType);
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
    public void AddMoney(long add)
    {
        MoneyPool.Money.Add(add);
        MoneyPool.Score.Add(add);
    }
    public void SubMoney(long sub)
    {
        MoneyPool.Money.Sub(sub);
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
            this.runningState = running;
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
            this.runningState = running;
            whatInteractingWith = (GameObj?)obj;
            shipState = value;
            return true;
        }
    }
    public bool StartThread(long stateNum, RunningStateType runningState)
    {
        lock (actionLock)
        {
            if (this.StateNum == stateNum)
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
            CanMove.SetReturnOri(false);
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
    public Ship(XY initPos, int initRadius, ShipType shipType, MoneyPool moneyPool) :
        base(initPos, initRadius, GameObjType.Ship)
    {
        this.CanMove.SetReturnOri(true);
        this.Occupation = OccupationFactory.FindIOccupation(this.ShipType = shipType);
        this.ViewRange = this.Occupation.ViewRange;
        this.HP = new(this.Occupation.MaxHp);
        this.Armor = new(this.Occupation.BaseArmor);
        this.Shield = new(this.Occupation.BaseShield);
        this.MoveSpeed.SetReturnOri(this.orgMoveSpeed = Occupation.MoveSpeed);
        this.MoneyPool = moneyPool;
        (this.producerType, this.constructorType, this.armorType, this.shieldType, this.weaponType) = this.ShipType switch
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
        (this.producer, this.constructor, this.armor, this.shield, this.weapon) = (
            ModuleFactory.FindIProducer(this.ShipType, this.producerType),
            ModuleFactory.FindIConstructor(this.ShipType, this.constructorType),
            ModuleFactory.FindIArmor(this.ShipType, this.armorType),
            ModuleFactory.FindIShield(this.ShipType, this.shieldType),
            ModuleFactory.FindIWeapon(this.ShipType, this.weaponType)
        );
    }
}
