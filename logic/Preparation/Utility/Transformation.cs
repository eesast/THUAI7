#region using Proto
using ProtoPlace = Protobuf.PlaceType;
using ProtoShape = Protobuf.ShapeType;
using ProtoPlayer = Protobuf.PlayerType;
using ProtoShip = Protobuf.ShipType;
using ProtoShipState = Protobuf.ShipState;
using ProtoWeapon = Protobuf.WeaponType;
using ProtoConstructor = Protobuf.ConstructorType;
using ProtoArmor = Protobuf.ArmorType;
using ProtoShield = Protobuf.ShieldType;
using ProtoProducer = Protobuf.ProducerType;
using ProtoBullet = Protobuf.BulletType;
using ProtoConstruction = Protobuf.ConstructionType;
#endregion

namespace Preparation.Utility;

public static class Transformation
{
    #region Place
    public static ProtoPlace PlaceTypeToProto(PlaceType placeType) => placeType switch
    {
        PlaceType.Null => ProtoPlace.Space,
        PlaceType.Ruin => ProtoPlace.Ruin,
        PlaceType.Shadow => ProtoPlace.Shadow,
        PlaceType.Asteroid => ProtoPlace.Asteroid,
        PlaceType.Resource => ProtoPlace.Resource,
        PlaceType.Construction => ProtoPlace.Construction,
        PlaceType.Wormhole => ProtoPlace.Wormhole,
        PlaceType.Home => ProtoPlace.HomePlace,
        _ => ProtoPlace.NullPlaceType
    };
    public static PlaceType PlaceTypeFromProto(ProtoPlace placeType) => placeType switch
    {
        ProtoPlace.Space => PlaceType.Null,
        ProtoPlace.Ruin => PlaceType.Ruin,
        ProtoPlace.Shadow => PlaceType.Shadow,
        ProtoPlace.Asteroid => PlaceType.Asteroid,
        ProtoPlace.Resource => PlaceType.Resource,
        ProtoPlace.Construction => PlaceType.Construction,
        ProtoPlace.Wormhole => PlaceType.Wormhole,
        ProtoPlace.HomePlace => PlaceType.Home,
        _ => PlaceType.Null
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
    #region Player
    public static ProtoPlayer PlayerToProto(GameObjType playerType) => playerType switch
    {
        GameObjType.Ship => ProtoPlayer.Ship,
        GameObjType.Home => ProtoPlayer.Home,
        _ => ProtoPlayer.NullPlayerType
    };
    #endregion
    #region Ship
    public static ProtoShip ShipToProto(ShipType shipType) => shipType switch
    {
        ShipType.CivilShip => ProtoShip.CivilianShip,
        ShipType.WarShip => ProtoShip.MilitaryShip,
        ShipType.FlagShip => ProtoShip.FlagShip,
        _ => ProtoShip.NullShipType
    };
    public static ShipType ShipFromProto(ProtoShip shipType) => shipType switch
    {
        ProtoShip.CivilianShip => ShipType.CivilShip,
        ProtoShip.MilitaryShip => ShipType.WarShip,
        ProtoShip.FlagShip => ShipType.FlagShip,
        _ => ShipType.Null
    };
    #endregion
    #region ShipState
    public static ProtoShipState ShipStateToProto(ShipStateType shipStateType) => shipStateType switch
    {
        ShipStateType.Null => ProtoShipState.Idle,
        ShipStateType.Producing => ProtoShipState.Producing,
        ShipStateType.Constructing => ProtoShipState.Constructing,
        ShipStateType.Recovering => ProtoShipState.Recovering,
        ShipStateType.Recycling => ProtoShipState.Recycling,
        ShipStateType.Attacking => ProtoShipState.Attacking,
        ShipStateType.Swinging => ProtoShipState.Swinging,
        ShipStateType.Stunned => ProtoShipState.Stunned,
        ShipStateType.Moving => ProtoShipState.Moving,
        _ => ProtoShipState.NullStatus
    };
    public static ShipStateType ShipStateFromProto(ProtoShipState shipStateType) => shipStateType switch
    {
        ProtoShipState.Producing => ShipStateType.Producing,
        ProtoShipState.Constructing => ShipStateType.Constructing,
        ProtoShipState.Recovering => ShipStateType.Recovering,
        ProtoShipState.Recycling => ShipStateType.Recycling,
        ProtoShipState.Attacking => ShipStateType.Attacking,
        ProtoShipState.Swinging => ShipStateType.Swinging,
        ProtoShipState.Stunned => ShipStateType.Stunned,
        ProtoShipState.Moving => ShipStateType.Moving,
        _ => ShipStateType.Null
    };
    #endregion
    #region Weapon
    public static ProtoWeapon WeaponToProto(GunType weaponType) => weaponType switch
    {
        GunType.LaserGun => ProtoWeapon.Lasergun,
        GunType.PlasmaGun => ProtoWeapon.Plasmagun,
        GunType.ShellGun => ProtoWeapon.Shellgun,
        GunType.MissileGun => ProtoWeapon.Missilegun,
        GunType.ArcGun => ProtoWeapon.Arcgun,
        _ => ProtoWeapon.NullWeaponType
    };
    public static GunType WeaponFromProto(ProtoWeapon weaponType) => weaponType switch
    {
        ProtoWeapon.Lasergun => GunType.LaserGun,
        ProtoWeapon.Plasmagun => GunType.PlasmaGun,
        ProtoWeapon.Shellgun => GunType.ShellGun,
        ProtoWeapon.Missilegun => GunType.MissileGun,
        ProtoWeapon.Arcgun => GunType.ArcGun,
        _ => GunType.Null
    };
    #endregion
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
        ConstructionType.Factory => ProtoConstruction.Factory,
        ConstructionType.Community => ProtoConstruction.Community,
        ConstructionType.Fort => ProtoConstruction.Fort,
        _ => ProtoConstruction.NullConstructionType
    };
    public static ConstructionType ConstructionFromProto(ProtoConstruction constructionType) => constructionType switch
    {
        ProtoConstruction.Factory => ConstructionType.Factory,
        ProtoConstruction.Community => ConstructionType.Community,
        ProtoConstruction.Fort => ConstructionType.Fort,
        _ => ConstructionType.Null
    };
    #endregion
}
