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


namespace Preparation.Utility;

public static class Transformation
{
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
}
