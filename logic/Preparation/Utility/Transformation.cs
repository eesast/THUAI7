#region using Proto
using ProtoArmor = Protobuf.ArmorType;
using ProtoBullet = Protobuf.BulletType;
using ProtoConstruction = Protobuf.ConstructionType;
using ProtoConstructor = Protobuf.ConstructorType;
using ProtoModule = Protobuf.ModuleType;
using ProtoPlace = Protobuf.PlaceType;
using ProtoPlayer = Protobuf.PlayerType;
using ProtoProducer = Protobuf.ProducerType;
using ProtoShape = Protobuf.ShapeType;
using ProtoShield = Protobuf.ShieldType;
using ProtoSweeper = Protobuf.SweeperType;
using ProtoSweeperState = Protobuf.SweeperState;
using ProtoWeapon = Protobuf.WeaponType;
#endregion

namespace Preparation.Utility;

public static class Transformation
{
    #region Armor
    public static ProtoArmor ArmorToProto(ArmorType armorType) => armorType switch
    {
        ArmorType.Armor1 => ProtoArmor.Armor1,
        ArmorType.Armor2 => ProtoArmor.Armor2,
        ArmorType.Armor3 => ProtoArmor.Armor3,
        _ => ProtoArmor.NullArmorType
    };
    public static ArmorType ArmorFromProto(ProtoArmor armorType) => armorType switch
    {
        ProtoArmor.Armor1 => ArmorType.Armor1,
        ProtoArmor.Armor2 => ArmorType.Armor2,
        ProtoArmor.Armor3 => ArmorType.Armor3,
        _ => ArmorType.Null
    };
    #endregion
    #region Bullet
    public static ProtoBullet BulletToProto(BulletType bulletType) => bulletType switch
    {
        BulletType.Laser => ProtoBullet.Laser,
        BulletType.Plasma => ProtoBullet.Plasma,
        BulletType.Shell => ProtoBullet.Shell,
        BulletType.Missile => ProtoBullet.Missile,
        BulletType.Arc => ProtoBullet.Arc,
        _ => ProtoBullet.NullBulletType
    };
    public static BulletType BulletFromProto(ProtoBullet bulletType) => bulletType switch
    {
        ProtoBullet.Laser => BulletType.Laser,
        ProtoBullet.Plasma => BulletType.Plasma,
        ProtoBullet.Shell => BulletType.Shell,
        ProtoBullet.Missile => BulletType.Missile,
        ProtoBullet.Arc => BulletType.Arc,
        _ => BulletType.Null
    };
    #endregion
    #region Construction
    public static ProtoConstruction ConstructionToProto(ConstructionType constructionType) => constructionType switch
    {
        ConstructionType.Factory => ProtoConstruction.Recyclebank,
        ConstructionType.Community => ProtoConstruction.Chargestation,
        ConstructionType.Fort => ProtoConstruction.Signaltower,
        _ => ProtoConstruction.NullConstructionType
    };
    public static ConstructionType ConstructionFromProto(ProtoConstruction constructionType) => constructionType switch
    {
        ProtoConstruction.Recyclebank => ConstructionType.Factory,
        ProtoConstruction.Chargestation => ConstructionType.Community,
        ProtoConstruction.Signaltower => ConstructionType.Fort,
        _ => ConstructionType.Null
    };
    #endregion
    #region Constructor
    public static ProtoConstructor ConstructorToProto(ConstructorType constructorType) => constructorType switch
    {
        ConstructorType.Constructor1 => ProtoConstructor.Constructor1,
        ConstructorType.Constructor2 => ProtoConstructor.Constructor2,
        ConstructorType.Constructor3 => ProtoConstructor.Constructor3,
        _ => ProtoConstructor.NullConstructorType
    };
    public static ConstructorType ConstructorFromProto(ProtoConstructor constructorType) => constructorType switch
    {
        ProtoConstructor.Constructor1 => ConstructorType.Constructor1,
        ProtoConstructor.Constructor2 => ConstructorType.Constructor2,
        ProtoConstructor.Constructor3 => ConstructorType.Constructor3,
        _ => ConstructorType.Null
    };
    #endregion
    #region Module
    public static ProtoModule ModuleToProto(ModuleType moduleType) => moduleType switch
    {
        ModuleType.Producer1 => ProtoModule.ModuleProducer1,
        ModuleType.Producer2 => ProtoModule.ModuleProducer2,
        ModuleType.Producer3 => ProtoModule.ModuleProducer3,
        ModuleType.Constructor1 => ProtoModule.ModuleConstructor1,
        ModuleType.Constructor2 => ProtoModule.ModuleConstructor2,
        ModuleType.Constructor3 => ProtoModule.ModuleConstructor3,
        ModuleType.Armor1 => ProtoModule.ModuleArmor1,
        ModuleType.Armor2 => ProtoModule.ModuleArmor2,
        ModuleType.Armor3 => ProtoModule.ModuleArmor3,
        ModuleType.Shield1 => ProtoModule.ModuleShield1,
        ModuleType.Shield2 => ProtoModule.ModuleShield2,
        ModuleType.Shield3 => ProtoModule.ModuleShield3,
        ModuleType.LaserGun => ProtoModule.ModuleLasergun,
        ModuleType.PlasmaGun => ProtoModule.ModulePlasmagun,
        ModuleType.ShellGun => ProtoModule.ModuleShellgun,
        ModuleType.MissileGun => ProtoModule.ModuleMissilegun,
        ModuleType.ArcGun => ProtoModule.ModuleArcgun,
        _ => ProtoModule.NullModuleType
    };
    public static ModuleType ModuleFromProto(ProtoModule moduleType) => moduleType switch
    {
        ProtoModule.ModuleProducer1 => ModuleType.Producer1,
        ProtoModule.ModuleProducer2 => ModuleType.Producer2,
        ProtoModule.ModuleProducer3 => ModuleType.Producer3,
        ProtoModule.ModuleConstructor1 => ModuleType.Constructor1,
        ProtoModule.ModuleConstructor2 => ModuleType.Constructor2,
        ProtoModule.ModuleConstructor3 => ModuleType.Constructor3,
        ProtoModule.ModuleArmor1 => ModuleType.Armor1,
        ProtoModule.ModuleArmor2 => ModuleType.Armor2,
        ProtoModule.ModuleArmor3 => ModuleType.Armor3,
        ProtoModule.ModuleShield1 => ModuleType.Shield1,
        ProtoModule.ModuleShield2 => ModuleType.Shield2,
        ProtoModule.ModuleShield3 => ModuleType.Shield3,
        ProtoModule.ModuleLasergun => ModuleType.LaserGun,
        ProtoModule.ModulePlasmagun => ModuleType.PlasmaGun,
        ProtoModule.ModuleShellgun => ModuleType.ShellGun,
        ProtoModule.ModuleMissilegun => ModuleType.MissileGun,
        ProtoModule.ModuleArcgun => ModuleType.ArcGun,
        _ => ModuleType.Null
    };
    #endregion
    #region Place
    public static ProtoPlace PlaceTypeToProto(PlaceType placeType) => placeType switch
    {
        PlaceType.Null => ProtoPlace.Ground,
        PlaceType.Ruin => ProtoPlace.Wall,
        PlaceType.Shadow => ProtoPlace.Grass,
        PlaceType.Asteroid => ProtoPlace.River,
        PlaceType.Resource => ProtoPlace.Garbage,
        PlaceType.Construction => ProtoPlace.Construction,
        PlaceType.Wormhole => ProtoPlace.Bridge,
        PlaceType.Home => ProtoPlace.Home,
        _ => ProtoPlace.NullPlaceType
    };
    public static PlaceType PlaceTypeFromProto(ProtoPlace placeType) => placeType switch
    {
        ProtoPlace.Ground => PlaceType.Null,
        ProtoPlace.Wall => PlaceType.Ruin,
        ProtoPlace.Grass => PlaceType.Shadow,
        ProtoPlace.River => PlaceType.Asteroid,
        ProtoPlace.Garbage => PlaceType.Resource,
        ProtoPlace.Construction => PlaceType.Construction,
        ProtoPlace.Bridge => PlaceType.Wormhole,
        ProtoPlace.Home => PlaceType.Home,
        _ => PlaceType.Null
    };
    #endregion
    #region Player
    public static ProtoPlayer PlayerToProto(GameObjType playerType) => playerType switch
    {
        GameObjType.Ship => ProtoPlayer.Sweeper,
        GameObjType.Home => ProtoPlayer.Team,
        _ => ProtoPlayer.NullPlayerType
    };
    public static GameObjType PlayerFromProto(ProtoPlayer playerType) => playerType switch
    {
        ProtoPlayer.Sweeper => GameObjType.Ship,
        ProtoPlayer.Team => GameObjType.Home,
        _ => GameObjType.Null
    };
    #endregion
    #region Producer
    public static ProtoProducer ProducerToProto(ProducerType producerType) => producerType switch
    {
        ProducerType.Producer1 => ProtoProducer.Producer1,
        ProducerType.Producer2 => ProtoProducer.Producer2,
        ProducerType.Producer3 => ProtoProducer.Producer3,
        _ => ProtoProducer.NullProducerType
    };
    public static ProducerType ProducerFromProto(ProtoProducer producerType) => producerType switch
    {
        ProtoProducer.Producer1 => ProducerType.Producer1,
        ProtoProducer.Producer2 => ProducerType.Producer2,
        ProtoProducer.Producer3 => ProducerType.Producer3,
        _ => ProducerType.Null
    };
    #endregion
    #region Shape
    public static ProtoShape ShapeToProto(ShapeType shapeType) => shapeType switch
    {
        ShapeType.Circle => ProtoShape.Circle,
        ShapeType.Square => ProtoShape.Square,
        _ => ProtoShape.NullShapeType
    };
    public static ShapeType ShapeFromProto(ProtoShape shapeType) => shapeType switch
    {
        ProtoShape.Circle => ShapeType.Circle,
        ProtoShape.Square => ShapeType.Square,
        _ => ShapeType.Null
    };
    #endregion
    #region Shield
    public static ProtoShield ShieldToProto(ShieldType shieldType) => shieldType switch
    {
        ShieldType.Shield1 => ProtoShield.Shield1,
        ShieldType.Shield2 => ProtoShield.Shield2,
        ShieldType.Shield3 => ProtoShield.Shield3,
        _ => ProtoShield.NullShieldType
    };
    public static ShieldType ShieldFromProto(ProtoShield shieldType) => shieldType switch
    {
        ProtoShield.Shield1 => ShieldType.Shield1,
        ProtoShield.Shield2 => ShieldType.Shield2,
        ProtoShield.Shield3 => ShieldType.Shield3,
        _ => ShieldType.Null
    };
    #endregion
    #region ShipState
    public static ProtoSweeperState ShipStateToProto(ShipStateType shipStateType) => shipStateType switch
    {
        ShipStateType.Null => ProtoSweeperState.Idle,
        ShipStateType.Producing => ProtoSweeperState.Producing,
        ShipStateType.Constructing => ProtoSweeperState.Constructing,
        ShipStateType.Recovering => ProtoSweeperState.Recovering,
        ShipStateType.Recycling => ProtoSweeperState.Recycling,
        ShipStateType.Attacking => ProtoSweeperState.Attacking,
        ShipStateType.Swinging => ProtoSweeperState.Swinging,
        ShipStateType.Stunned => ProtoSweeperState.Stunned,
        ShipStateType.Moving => ProtoSweeperState.Moving,
        _ => ProtoSweeperState.NullStatus
    };
    public static ShipStateType ShipStateFromProto(ProtoSweeperState shipStateType) => shipStateType switch
    {
        ProtoSweeperState.Producing => ShipStateType.Producing,
        ProtoSweeperState.Constructing => ShipStateType.Constructing,
        ProtoSweeperState.Recovering => ShipStateType.Recovering,
        ProtoSweeperState.Recycling => ShipStateType.Recycling,
        ProtoSweeperState.Attacking => ShipStateType.Attacking,
        ProtoSweeperState.Swinging => ShipStateType.Swinging,
        ProtoSweeperState.Stunned => ShipStateType.Stunned,
        ProtoSweeperState.Moving => ShipStateType.Moving,
        _ => ShipStateType.Null
    };
    #endregion
    #region ShipType
    public static ProtoSweeper ShipTypeToProto(ShipType shipType) => shipType switch
    {
        ShipType.CivilShip => ProtoSweeper.CivilianSweeper,
        ShipType.WarShip => ProtoSweeper.MilitarySweeper,
        ShipType.FlagShip => ProtoSweeper.FlagSweeper,
        _ => ProtoSweeper.NullSweeperType
    };
    public static ShipType ShipTypeFromProto(ProtoSweeper shipType) => shipType switch
    {
        ProtoSweeper.CivilianSweeper => ShipType.CivilShip,
        ProtoSweeper.MilitarySweeper => ShipType.WarShip,
        ProtoSweeper.FlagSweeper => ShipType.FlagShip,
        _ => ShipType.Null
    };
    #endregion
    #region Weapon
    public static ProtoWeapon WeaponToProto(WeaponType weaponType) => weaponType switch
    {
        WeaponType.LaserGun => ProtoWeapon.Lasergun,
        WeaponType.PlasmaGun => ProtoWeapon.Plasmagun,
        WeaponType.ShellGun => ProtoWeapon.Shellgun,
        WeaponType.MissileGun => ProtoWeapon.Missilegun,
        WeaponType.ArcGun => ProtoWeapon.Arcgun,
        _ => ProtoWeapon.NullWeaponType
    };
    public static WeaponType WeaponFromProto(ProtoWeapon weaponType) => weaponType switch
    {
        ProtoWeapon.Lasergun => WeaponType.LaserGun,
        ProtoWeapon.Plasmagun => WeaponType.PlasmaGun,
        ProtoWeapon.Shellgun => WeaponType.ShellGun,
        ProtoWeapon.Missilegun => WeaponType.MissileGun,
        ProtoWeapon.Arcgun => WeaponType.ArcGun,
        _ => WeaponType.Null
    };
    #endregion
}
