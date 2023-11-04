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
    public IntNumUpdateEachCD BulletNum { get; }

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
    public IArmor ArmorModule;
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
    public Ship(XY initPos, int initRadius, ShipType shipType) :
        base(initPos, initRadius, GameObjType.Ship)
    {
        this.CanMove.SetReturnOri(true);
        this.Occupation = OccupationFactory.FindIOccupation(this.ShipType = shipType);
        this.ViewRange = this.Occupation.ViewRange;
        this.HP = new(this.Occupation.MaxHp);
        this.Armor = new(this.Occupation.BaseArmor);
        this.Shield = new(this.Occupation.BaseShield);
        this.MoveSpeed.SetReturnOri(this.orgMoveSpeed = Occupation.MoveSpeed);
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
