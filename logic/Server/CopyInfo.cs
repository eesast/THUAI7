using Protobuf;
using GameClass.GameObj;
using Preparation.Utility;
using Gaming;
using Preparation.Interface;
using System.Runtime.Versioning;

namespace Server
{

    public static class CopyInfo
    {
        public static MessageOfObj? Auto(GameObj gameObj, long time)
        {
            switch (gameObj.Type)
            {
                case Preparation.Utility.GameObjType.Ship:
                    return Ship((Ship)gameObj, time);
                case Preparation.Utility.GameObjType.Home:
                    return Home((GameClass.GameObj.Areas.Home)gameObj, time);
                case Preparation.Utility.GameObjType.Bullet:
                    return Bullet((Bullet)gameObj);
                case Preparation.Utility.GameObjType.BombedBullet:
                    return BombedBullet((BombedBullet)gameObj);
                case Preparation.Utility.GameObjType.Resource:
                    return Resource((GameClass.GameObj.Areas.Resource)gameObj);
                case Preparation.Utility.GameObjType.Construction:
                    GameClass.GameObj.Areas.Construction construction = (GameClass.GameObj.Areas.Construction)gameObj;
                    if (construction.ConstructionType == Preparation.Utility.ConstructionType.Factory)
                        return Factory(construction);
                    else if (construction.ConstructionType == Preparation.Utility.ConstructionType.Community)
                        return Community(construction);
                    else if (construction.ConstructionType == Preparation.Utility.ConstructionType.Fort)
                        return Fort(construction);
                    return null;
                case Preparation.Utility.GameObjType.Wormhole:
                    return Wormhole((GameClass.GameObj.Areas.Wormhole)gameObj);
                default: return null;
            }
        }
        public static MessageOfObj? Auto(MessageOfNews news)
        {
            MessageOfObj objMsg = new()
            {
                NewsMessage = news
            };
            return objMsg;
        }

        private static MessageOfObj? Ship(Ship player, long time)
        {
            MessageOfObj msg = new()
            {
                ShipMessage = new()
                {
                    X = player.Position.x,
                    Y = player.Position.y,
                    Speed = player.MoveSpeed,
                    Hp = (int)player.HP,
                    Armor = (int)player.Armor,
                    Shield = (int)player.Shield,
                    TeamId = player.TeamID,
                    PlayerId = player.ShipID,
                    Guid = player.ID,
                    ShipState = Transformation.ShipStateToProto(player.ShipState),
                    ShipType = Transformation.ShipTypeToProto(player.ShipType),
                    ViewRange = player.ViewRange,
                    ConstructorType = Transformation.ConstructorToProto(player.ConstructorModuleType),
                    ArmorType = Transformation.ArmorToProto(player.ArmorModuleType),
                    ShieldType = Transformation.ShieldToProto(player.ShieldModuleType),
                    WeaponType = Transformation.WeaponToProto(player.WeaponModuleType),
                    FacingDirection = player.FacingDirection.Angle(),
                }
            };
            return msg;
        }

        private static MessageOfObj? Home(GameClass.GameObj.Areas.Home player, long time)
        {
            MessageOfObj msg = new()
            {
                HomeMessage = new()
                {
                    X = player.Position.x,
                    Y = player.Position.y,
                    Hp = (int)player.HP,
                    TeamId = player.TeamID,
                }
            };
            return msg;
        }

        private static MessageOfObj Bullet(Bullet bullet)
        {
            MessageOfObj msg = new()
            {
                BulletMessage = new()
                {
                    Type = Transformation.BulletToProto(bullet.TypeOfBullet),
                    X = bullet.Position.x,
                    Y = bullet.Position.y,
                    FacingDirection = bullet.FacingDirection.Angle(),
                    Damage = bullet.AP,
                    TeamId = bullet.Parent.TeamID,
                    Guid = bullet.ID,
                    BombRange = bullet.BulletBombRange,
                    Speed = bullet.Speed
                }
            };
            return msg;
        }

        private static MessageOfObj BombedBullet(BombedBullet bombedBullet)
        {
            MessageOfObj msg = new()
            {
                BombedBulletMessage = new()
                {
                    Type = Transformation.BulletToProto(bombedBullet.bulletHasBombed.TypeOfBullet),
                    X = bombedBullet.bulletHasBombed.Position.x,
                    Y = bombedBullet.bulletHasBombed.Position.y,
                    FacingDirection = bombedBullet.facingDirection.Angle(),
                    MappingId = bombedBullet.MappingID,
                    BombRange = bombedBullet.bulletHasBombed.BulletBombRange
                }
            };
            //   Debugger.Output(bombedBullet, bombedBullet.Place.ToString()+" "+bombedBullet.Position.ToString());
            return msg;
        }

        private static MessageOfObj Resource(GameClass.GameObj.Areas.Resource resource)
        {
            MessageOfObj msg = new()
            {
                ResourceMessage = new()
                {
                    X = resource.Position.x,
                    Y = resource.Position.y,
                    Progress = (int)resource.HP,
                }
            };
            return msg;
        }
        private static MessageOfObj Factory(GameClass.GameObj.Areas.Construction construction)
        {
            MessageOfObj msg = new()
            {
                FactoryMessage = new()
                {
                    X = construction.Position.x,
                    Y = construction.Position.y,
                    Hp = (int)construction.HP,
                    TeamId = construction.TeamID,
                }
            };
            return msg;
        }

        private static MessageOfObj Community(GameClass.GameObj.Areas.Construction construction)
        {
            MessageOfObj msg = new()
            {
                CommunityMessage = new()
                {
                    X = construction.Position.x,
                    Y = construction.Position.y,
                    Hp = (int)construction.HP,
                    TeamId = construction.TeamID,
                }
            };
            return msg;
        }

        private static MessageOfObj Fort(GameClass.GameObj.Areas.Construction construction)
        {
            MessageOfObj msg = new()
            {
                FortMessage = new()
                {
                    X = construction.Position.x,
                    Y = construction.Position.y,
                    Hp = (int)construction.HP,
                    TeamId = construction.TeamID,
                }
            };
            return msg;
        }
        private static MessageOfObj Wormhole(GameClass.GameObj.Areas.Wormhole wormhole)
        {
            MessageOfObj msg = new()
            {
                WormholeMessage = new()
                {
                    X = wormhole.Position.x,
                    Y = wormhole.Position.y,
                    Hp = (int)wormhole.HP,
                }
            };
            return msg;
        }
    }
}
