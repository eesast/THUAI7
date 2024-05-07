using GameClass.GameObj;
using GameClass.GameObj.Areas;
using Preparation.Utility;
using Protobuf;
using Utility = Preparation.Utility;

namespace Server
{

    public static class CopyInfo
    {
        public static MessageOfObj? Auto(GameObj gameObj, long time)
        {
            if (gameObj.IsRemoved == true)
                return null;
            switch (gameObj.Type)
            {
                case GameObjType.Ship:
                    return Ship((Ship)gameObj, time);
                case GameObjType.Home:
                    return Home((Home)gameObj, time);
                case GameObjType.Bullet:
                    return Bullet((Bullet)gameObj);
                case GameObjType.BombedBullet:
                    return BombedBullet((BombedBullet)gameObj);
                case GameObjType.Resource:
                    return Resource((Resource)gameObj);
                case GameObjType.Construction:
                    Construction construction = (Construction)gameObj;
                    if (construction.ConstructionType == Utility.ConstructionType.Factory)
                        return Factory(construction);
                    else if (construction.ConstructionType == Utility.ConstructionType.Community)
                        return Community(construction);
                    else if (construction.ConstructionType == Utility.ConstructionType.Fort)
                        return Fort(construction);
                    return null;
                case GameObjType.Wormhole:
                    return Wormhole((WormholeCell)gameObj);
                default: return null;
            }
        }

        public static MessageOfObj? Auto(Base @base, long time)
        {
            return Base(@base, time);
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
                    PlayerId = player.PlayerID,
                    Guid = player.ID,
                    ShipState = Transformation.ShipStateToProto(player.ShipState),
                    ShipType = Transformation.ShipTypeToProto(player.ShipType),
                    ViewRange = player.ViewRange,
                    ProducerType = Transformation.ProducerToProto(player.ProducerModuleType),
                    ConstructorType = Transformation.ConstructorToProto(player.ConstructorModuleType),
                    ArmorType = Transformation.ArmorToProto(player.ArmorModuleType),
                    ShieldType = Transformation.ShieldToProto(player.ShieldModuleType),
                    WeaponType = Transformation.WeaponToProto(player.WeaponModuleType),
                    FacingDirection = player.FacingDirection.Angle(),
                }
            };
            return msg;
        }

        private static MessageOfObj? Home(Home player, long time)
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

        private static MessageOfObj? Base(Base player, long time)
        {
            MessageOfObj msg = new()
            {
                TeamMessage = new()
                {
                    TeamId = player.TeamID,
                    PlayerId = player.PlayerID,
                    Score = player.MoneyPool.Score,
                    Energy = player.MoneyPool.Money,
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
                    TeamId = bullet.Parent!.TeamID,
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

        private static MessageOfObj Resource(Resource resource)
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
        private static MessageOfObj Factory(Construction construction)
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

        private static MessageOfObj Community(Construction construction)
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

        private static MessageOfObj Fort(Construction construction)
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
        private static MessageOfObj Wormhole(WormholeCell wormhole)
        {
            MessageOfObj msg = new()
            {
                WormholeMessage = new()
                {
                    X = wormhole.Position.x,
                    Y = wormhole.Position.y,
                    Hp = (int)wormhole.Wormhole.HP,
                    Id = wormhole.Wormhole.ID,
                }
            };
            return msg;
        }
    }
}
