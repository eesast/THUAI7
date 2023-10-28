using Preparation.Interface;
using Preparation.Utility;
using GameClass.GameObj.Modules;
using GameClass.GameObj.Occupations;

namespace GameClass.GameObj;

public class Ship : Movable, IShip
{
    public AtomicLong TeamID { get; } = new AtomicLong(long.MaxValue);
    public AtomicLong ShipID { get; } = new AtomicLong(long.MaxValue);
    public override bool IsRigid => true;
    public override ShapeType Shape => ShapeType.Circle;
    private readonly int viewRange;
    public int ViewRange => viewRange;
    public override bool IgnoreCollideExecutor(IGameObj targetObj)
    {
        if (IsRemoved)
            return true;
        if (targetObj.Type == GameObjType.Ship && XY.DistanceCeil3(targetObj.Position, this.Position) < this.Radius + targetObj.Radius - GameData.AdjustLength)
            return true;
        return false;
    }
    public LongWithVariableRange HP { get; }
    public LongWithVariableRange Armor { get; }
    public LongWithVariableRange Shield { get; }
    private ShipType shipType = ShipType.Null;
    public ShipType ShipType => shipType;
    private ShipStateType shipState = ShipStateType.Null;
    public ShipStateType ShipState => shipState;
    private readonly IOccupation occupation;
    public IOccupation Occupation => occupation;
    public IntNumUpdateByCD BulletNum { get; }
    private IProducer? producer = null;
    public IProducer? ProducerModule => producer;
    private IConstructor? constructor = null;
    public IConstructor? ConstructorModule => constructor;
    private IArmor? armor = null;
    public IArmor? ArmorModule => armor;
    private IShield? shield = null;
    public IShield? ShieldModule => shield;
    private IWeapon? weapon = null;
    public IWeapon? WeaponModule => weapon;
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
        this.occupation = OccupationFactory.FindIOccupation(shipType);
        this.viewRange = occupation.ViewRange;
        this.HP = new(Occupation.MaxHp);
        this.MoveSpeed.SetReturnOri(this.orgMoveSpeed = Occupation.MoveSpeed);
        this.shipType = shipType;
        switch (shipType)
        {
            case ShipType.CivilShip:
                this.producer = new CivilProducer1();
                this.constructor = new CivilConstructor1();
                this.armor = null;
                this.shield = null;
                this.weapon = null;
                break;
            case ShipType.WarShip:
                this.producer = null;
                this.constructor = null;
                this.armor = null;
                this.shield = null;
                this.weapon = new WarLaserGun();
                break;
            case ShipType.FlagShip:
                this.producer = null;
                this.constructor = null;
                this.armor = null;
                this.shield = null;
                this.weapon = new FlagLaserGun();
                break;
        }
    }
}
