// 杂项函数
#pragma once
#ifndef UTILS_HPP
#define UTILS_HPP

#include <cstdint>
#include <cmath>
#include <map>
#include <vector>
#include "Message2Clients.pb.h"
#include "Message2Server.pb.h"
#include "MessageType.pb.h"

#include "structures.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

namespace AssistFunction
{

    constexpr int32_t numOfGridPerCell = 1000;

    [[nodiscard]] constexpr inline int32_t GridToCell(int32_t grid) noexcept
    {
        return grid / numOfGridPerCell;
    }

    [[nodiscard]] constexpr inline int32_t GridToCell(double grid) noexcept
    {
        return int(grid) / numOfGridPerCell;
    }

    inline bool HaveView(int32_t x, int32_t y, int32_t newX, int32_t newY, std::vector<std::vector<THUAI6::PlaceType>>& map)
    {
        int32_t deltaX = newX - x;
        int32_t deltaY = newY - y;
        double distance = deltaX * deltaX + deltaY * deltaY;
        THUAI7::PlaceType myPlace = map[GridToCell(x)][GridToCell(y)];
        THUAI7::PlaceType newPlace = map[GridToCell(newX)][GridToCell(newY)];
        if (newPlace == THUAI7::PlaceType::Shadow && myPlace != THUAI6::PlaceType::Shadow)  
            return false;
        int32_t divide = std::max(std::abs(deltaX), std::abs(deltaY)) / 100;
        if (divide == 0)
            return true;
        double dx = deltaX / divide;
        double dy = deltaY / divide;
        double myX = double(x);
        double myY = double(y);
        if (newPlace == THUAI7::PlaceType::Shadow && myPlace == THUAI7::PlaceType::Shadow)  
            for (int32_t i = 0; i < divide; i++)
            {
                myX += dx;
                myY += dy;
                if (map[GridToCell(myX)][GridToCell(myY)] != THUAI7::PlaceType::Shadow)
                    return false;
            }
        else  
            for (int32_t i = 0; i < divide; i++)
            {
                myX += dx;
                myY += dy;
                if (map[GridToCell(myX)][GridToCell(myY)] == THUAI6::PlaceType::Ruin)
                    return false;
            }
        return true;

    }
}  // namespace AssistFunction


//需要修改
namespace Proto2THUAI7
{
    // 用于将Protobuf中的枚举转换为THUAI7的枚举
    inline std::map<protobuf::PlaceType, THUAI7::PlaceType> placeTypeDict{
        {protobuf::PlaceType::NULL_PLACE_TYPE, THUAI7::PlaceType::NullPlaceType},
        {protobuf::PlaceType::HOME, THUAI7::PlaceType::Home},
        {protobuf::PlaceType::SPACE, THUAI7::PlaceType::Space},
        {protobuf::PlaceType::RUIN, THUAI7::PlaceType::Ruin},
        {protobuf::PlaceType::SHADOW, THUAI7::PlaceType::Shadow},
        {protobuf::PlaceType::ASTEROID, THUAI7::PlaceType::Asteroid},
        {protobuf::PlaceType::RESOURCE, THUAI7::PlaceType::Resource},
        {protobuf::PlaceType::BUILDING, THUAI7::PlaceType::Building},
        {protobuf::PlaceType::WORMHOLE, THUAI7::PlaceType::Wormhole},
    };

    inline std::map<protobuf::ShapeType, THUAI7::ShapeType> shapeTypeDict{
        {protobuf::ShapeType::NULL_SHAPE_TYPE, THUAI7::ShapeType::NullShapeType},
        {protobuf::ShapeType::CIRCLE, THUAI7::ShapeType::Circle},
        {protobuf::ShapeType::SQUARE, THUAI7::ShapeType::Square},
    };

    inline std::map<protobuf::PlayerTeam, THUAI7::PlayerTeam> playerTeamDict{
        {protobuf::PlayerTeam::NULL_TEAM, THUAI7::PlayerTeam::NullTeam},
        {protobuf::PlayerTeam::UP, THUAI7::PlayerTeam::Up},
        {protobuf::PlayerTeam::DOWN, THUAI7::PlayerTeam::Down},
    };

    inline std::map<protobuf::ShipType, THUAI7::ShipType> shipTypeDict{
        {protobuf::ShipType::NULL_SHIP_TYPE, THUAI7::ShipType::NullShipType},
        {protobuf::ShipType::CIVILIAN_SHIP, THUAI7::ShipType::CivilianShip},
        {protobuf::ShipType::MILITARY_SHIP, THUAI7::ShipType::MilitaryShip},
        {protobuf::ShipType::FLAG_SHIP, THUAI7::ShipType::FlagShip},
    };

    inline std::map<protobuf::ShipState, THUAI7::ShipState> shipStateDict{
        {protobuf::ShipState::NULL_SHIP_STATE, THUAI7::ShipState::NullShipState},
        {protobuf::ShipState::IDLE, THUAI7::ShipState::Idle},
        {protobuf::ShipState::PRODUCING, THUAI7::ShipState::Producing},
        {protobuf::ShipState::CONSTRUCTING, THUAI7::ShipState::Constructing},
        {protobuf::ShipState::RECOVERING, THUAI7::ShipState::Recovering},
        {protobuf::ShipState::RECYCLING, THUAI7::ShipState::Recycling},
        {protobuf::ShipState::ATTACKING, THUAI7::ShipState::Attacking},
        {protobuf::ShipState::SWINGING, THUAI7::ShipState::Swinging},
        {protobuf::ShipState::STUUNED, THUAI7::ShipState::Stuuned},
    };

    inline std::map<protobuf::GameState, THUAI7::GameState> gameStateDict{
        {protobuf::GameState::NULL_GAME_STATE, THUAI7::GameState::NullGameState},
        {protobuf::GameState::GAME_START, THUAI7::GameState::GameStart},
        {protobuf::GameState::GAME_RUNNING, THUAI7::GameState::GameRunning},
        {protobuf::GameState::GAME_END, THUAI7::GameState::GameEnd},
    };

    inline std::map<protobuf::BulletType, THUAI7::BulletType> bulletTypeDict{
        {protobuf::BulletType::NULL_BULLET_TYPE, THUAI7::BulletType::NullBulletType},
        {protobuf::BulletType::LASER, THUAI7::BulletType::Laser},
        {protobuf::BulletType::PLASMA, THUAI7::BulletType::Plasma},
        {protobuf::BulletType::SHELL, THUAI7::BulletType::Shell},
        {protobuf::BulletType::MISSILE, THUAI7::BulletType::Missile},
        {protobuf::BulletType::ELECTRICARC, THUAI7::BulletType::ElectricArc},
    };

    inline std::map<protobuf::MessageOfObj::MessageOfObjCase, THUAI7::MessageOfObj> messageOfObjDict{
        {protobuf::MessageOfObj::MessageOfObjCase::kShipMessage, THUAI7::MessageOfObj::ShipMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kBulletMessage, THUAI7::MessageOfObj::BulletMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kFactoryMessage, THUAI7::MessageOfObj::FactoryMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kCommunityMessage, THUAI7::MessageOfObj::CommunityMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kFortressMessage, THUAI7::MessageOfObj::FortressMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kWormholeMessage, THUAI7::MessageOfObj::WormholeMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kHomeMessage, THUAI7::MessageOfObj::HomeMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kResourceMessage, THUAI7::MessageOfObj::ResourceMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kMapMessage, THUAI7::MessageOfObj::MapMessage},
    };

    inline std::map<protobuf::MessageOfNews::NewsCase, THUAI7::NewsType> newsTypeDict{
        {protobuf::MessageOfNews::NewsCase::NEWS_NOT_SET, THUAI7::NewsType::NullNewsType},
        {protobuf::MessageOfNews::NewsCase::kTextMessage, THUAI7::NewsType::TextMessage},
        {protobuf::MessageOfNews::NewsCase::kBinaryMessage, THUAI7::NewsType::BinaryMessage},
    };

    // 用于将Protobuf中的类转换为THUAI7的类
    inline std::shared_ptr<THUAI7::Ship> Protobuf2THUAI7Ship(const protobuf::MessageOfShip& shipMsg)
    {
        auto ship = std::make_shared<THUAI7::Ship>();
        ship->x = shipMsg.x();
        ship->y = shipMsg.y();
        ship->speed = shipMsg.speed();
        ship->hp = shipMsg.hp();
        ship->team = playerTeamDict[shipMsg.team()];
        ship->facingDirection = shipMsg.facing_direction();
        ship->shipState = shipStateDict[shipMsg.ship_state()];
        ship->shipType = shipTypeDict[shipMsg.ship_type()];
        ship->guid = shipMsg.guid();
        ship->playerTeam = playerTeamDict[shipMsg.player_team()];
        ship->economy=shipMsg.economy();
        for(int32_t i=0;i<=shipMsg.module_size();i++)
        {
            ship->module.push_back(Protobuf2THUAI7Module(shipMsg.module(i)));
        }
        return ship;
    }

    inline std::shared_ptr<THUAI7::Module> Protobuf2THUAI7Module(const protobuf::MessageOfModule& moduleMsg)
    {
        auto module = std::make_shared<THUAI7::Module>();
        module->moduleType = moduleMsg.module_type();
        module->moduleLevel = moduleMsg.module_label();
        return module;
    }

    inline std::shared_ptr<THUAI7::Bullet> Protobuf2THUAI7Bullet(const protobuf::MessageOfBullet& bulletMsg)
    {
        auto bullet = std::make_shared<THUAI7::Bullet>();
        bullet->bulletType=bulletTypeDict[bulletMsg.bullet_type()];
        bullet->x = bulletMsg.x();
        bullet->y = bulletMsg.y();
        bullet->speed = bulletMsg.speed();
        bullet->facingDirection = bulletMsg.facing_direction();
        bullet->guid = bulletMsg.guid();
        bullet->armorRate= bulletMsg.armor_rate();
        bullet->team = playerTeamDict[bulletMsg.team()];
        bullet->shieldRate = bulletMsg.shield_rate();
        bullet->bombRange = bulletMsg.bomb_range();
        bullet->radius = bulletMsg.radius();
        return bullet;
    }
    
    inline std::shared_ptr<THUAI7::Home> Protobuf2THUAI7Home(const protobuf::MessageOfHome& homeMsg)
    {
        auto home = std::make_shared<THUAI7::Home>();
        home->x = homeMsg.x();
        home->y = homeMsg.y();
        home->hp = homeMsg.hp();
        home->guid = homeMsg.guid();
        home->team = playerTeamDict[homeMsg.team()];
        home->economy = homeMsg.economy();
        return home;
    }
    
    
    inline std::shared_ptr<THUAI7::Map> Protobuf2THUAI7Map(const protobuf::MessageOfMap& mapMsg)
    {
        auto map = std::make_shared<THUAI7::Map>();
        map->x = mapMsg.x();
        map->y = mapMsg.y();
        map->placeType = placeTypeDict[mapMsg.place_type()];
        map->shapeType = shapeTypeDict[mapMsg.shape_type()];
        map->guid = mapMsg.guid();
        map->playerID = mapMsg.player_id();
        map->viewRange = mapMsg.view_range();
        map->radius = mapMsg.radius();
        return map;
    }

    inline std::shared_ptr<THUAI7::News> Protobuf2THUAI7News(const protobuf::MessageOfNews& newsMsg)
    {
        auto news = std::make_shared<THUAI7::News>();
        news->newsType = newsTypeDict[newsMsg.news_case()];
        news->textMessage = newsMsg.text_message();
        news->binaryMessage = newsMsg.binary_message();
        return news;
    }

  
    

    inline std::shared_ptr<THUAI7::GameInfo> Protobuf2THUAI7GameInfo(const protobuf::MessageOfAll& allMsg)
    {
        auto gameInfo = std::make_shared<THUAI7::GameInfo>();
        gameInfo->gameTime = gameInfoMsg.game_time();
        gameInfo->upEconomy=gameInfoMsg.up_economy();
        gameInfo->downEconomy=gameInfoMsg.down_economy();
    }

    inline std::shared_ptr<THUAI7::Message2Clients> Protobuf2THUAI7Message2Clients(const protobuf::Message2Clients& message2Clients)
    {
        auto message = std::make_shared<THUAI7::Message2Clients>();
        message->messageType = message2Clients.message_type();
        message->message = message2Clients.message();
        return message;
    }

    inline std::shared_ptr<THUAI7::Message2Server> Protobuf2THUAI7Message2Server(const protobuf::Message2Server& message2Server)
    {
        auto message = std::make_shared<THUAI7::Message2Server>();
        message->messageType = message2Server.message_type();
        message->message = message2Server.message();
        return message;
    }

    inline std::shared_ptr<THUAI7::Message2Clients> Protobuf2THUAI7Message2Clients(const std::string& message2Clients)
    {
        protobuf::Message2Clients message2ClientsMsg;
        message2ClientsMsg.ParseFromString(message2Clients);
        return Protobuf2THUAI7Message2Clients(message2ClientsMsg);
    }





}
// 辅助函数，用于将proto信息转换为THUAI6信息
namespace THUAI72Proto
{
    // 用于将THUAI7的枚举转换为Protobuf的枚举
    inline std::map<THUAI7::PlaceType, protobuf::PlaceType> placeTypeDict{
        {THUAI7::PlaceType::NullPlaceType, protobuf::PlaceType::NULL_PLACE_TYPE},
        {THUAI7::PlaceType::Home, protobuf::PlaceType::HOME},
        {THUAI7::PlaceType::Space, protobuf::PlaceType::SPACE},
        {THUAI7::PlaceType::Ruin, protobuf::PlaceType::RUIN},
        {THUAI7::PlaceType::Shadow, protobuf::PlaceType::SHADOW},
        {THUAI7::PlaceType::Asteroid, protobuf::PlaceType::ASTEROID},
        {THUAI7::PlaceType::Resource, protobuf::PlaceType::RESOURCE},
        {THUAI7::PlaceType::Building, protobuf::PlaceType::BUILDING},
        {THUAI7::PlaceType::Wormhole, protobuf::PlaceType::WORMHOLE},
    };

    inline std::map<THUAI7::ShapeType, protobuf::ShapeType> shapeTypeDict{
        {THUAI7::ShapeType::NullShapeType, protobuf::ShapeType::NULL_SHAPE_TYPE},
        {THUAI7::ShapeType::Circle, protobuf::ShapeType::CIRCLE},
        {THUAI7::ShapeType::Square, protobuf::ShapeType::SQUARE},
    };

    inline std::map<THUAI7::PlayerTeam, protobuf::PlayerTeam> playerTeamDict{
        {THUAI7::PlayerTeam::NullTeam, protobuf::PlayerTeam::NULL_TEAM},
        {THUAI7::PlayerTeam::Up, protobuf::PlayerTeam::UP},
        {THUAI7::PlayerTeam::Down, protobuf::PlayerTeam::DOWN},
    };

    inline std::map<THUAI7::ShipType, protobuf::ShipType> shipTypeDict{
        {THUAI7::ShipType::NullShipType, protobuf::ShipType::NULL_SHIP_TYPE},
        {THUAI7::ShipType::CivilianShip, protobuf::ShipType::CIVILIAN_SHIP},
        {THUAI7::ShipType::MilitaryShip, protobuf::ShipType::MILITARY_SHIP},
        {THUAI7::ShipType::FlagShip, protobuf::ShipType::FLAG_SHIP},
    };

    inline std::map<THUAI7::ShipState, protobuf::ShipState> shipStateDict{
        {THUAI7::ShipState::NullShipState, protobuf::ShipState::NULL_SHIP_STATE},
        {THUAI7::ShipState::Idle, protobuf::ShipState::IDLE},
        {THUAI7::ShipState::Producing, protobuf::ShipState::PRODUCING},
        {THUAI7::ShipState::Constructing, protobuf::ShipState::CONSTRUCTING},
        {THUAI7::ShipState::Recovering, protobuf::ShipState::RECOVERING},
        {THUAI7::ShipState::Recycling, protobuf::ShipState::RECYCLING},
        {THUAI7::ShipState::Attacking, protobuf::ShipState::ATTACKING},
        {THUAI7::ShipState::Swinging, protobuf::ShipState::SWINGING},
        {THUAI7::ShipState::Stunned, protobuf::ShipState::STUNNED},
 
    };

    inline std::map<THUAI7::GameState, protobuf::GameState> gameStateDict{
        {THUAI7::GameState::NullGameState, protobuf::GameState::NULL_GAME_STATE},
        {THUAI7::GameState::GameStart, protobuf::GameState::GAME_START},
        {THUAI7::GameState::GameRunning, protobuf::GameState::GAME_RUNNING},
        {THUAI7::GameState::GameEnd, protobuf::GameState::GAME_END},
    };

    inline std::map<THUAI7::BulletType, protobuf::BulletType> bulletTypeDict{
        {THUAI7::BulletType::NullBulletType, protobuf::BulletType::NULL_BULLET_TYPE},
        {THUAI7::BulletType::Laser, protobuf::BulletType::LASER},
        {THUAI7::BulletType::Plasma, protobuf::BulletType::PLASMA},
        {THUAI7::BulletType::Shell, protobuf::BulletType::SHELL},
        {THUAI7::BulletType::Missile, protobuf::BulletType::MISSILE},
        {THUAI7::BulletType::ElectricArc, protobuf::BulletType::ELECTRIC_ARC},
    };

    inline std::map<THUAI7::MessageOfObj, protobuf::MessageOfObj::MessageOfObjCase> messageOfObjDict{
        {THUAI7::MessageOfObj::NullMessageOfObj, protobuf::MessageOfObj::MessageOfObjCase::MESSAGE_OF_OBJ_NOT_SET},
        {THUAI7::MessageOfObj::ShipMessage, protobuf::MessageOfObj::MessageOfObjCase::kShipMessage},
        {THUAI7::MessageOfObj::BulletMessage, protobuf::MessageOfObj::MessageOfObjCase::kBulletMessage},
        {THUAI7::MessageOfObj::FactoryMessage, protobuf::MessageOfObj::MessageOfObjCase::kFactoryMessage},
        {THUAI7::MessageOfObj::CommunityMessage, protobuf::MessageOfObj::MessageOfObjCase::kCommunityMessage},
        {THUAI7::MessageOfObj::FortressMessage, protobuf::MessageOfObj::MessageOfObjCase::kFortressMessage},
        {THUAI7::MessageOfObj::WormholeMessage, protobuf::MessageOfObj::MessageOfObjCase::kWormholeMessage},
        {THUAI7::MessageOfObj::HomeMessage, protobuf::MessageOfObj::MessageOfObjCase::kHomeMessage},
        {THUAI7::MessageOfObj::MapMessage, protobuf::MessageOfObj::MessageOfObjCase::kMapMessage},
    };

    inline std::map<THUAI7::NewsType, protobuf::MessageOfNews::NewsCase> newsTypeDict{
        {THUAI7::NewsType::NullNewsType, protobuf::MessageOfNews::NewsCase::NEWS_NOT_SET},
        {THUAI7::NewsType::TextMessage, protobuf::MessageOfNews::NewsCase::kTextMessage},
        {THUAI7::NewsType::BinaryMessage, protobuf::MessageOfNews::NewsCase::kBinaryMessage},
    };

    inline protobuf::MoveMsg THUAI72ProtobufMove(int64_t time, double angle, int64_t id)
    {
        protobuf::MoveMsg moveMsg;
        moveMsg.set_time_in_milliseconds(time);
        moveMsg.set_angle(angle);
        moveMsg.set_ship_id(id);
        return moveMsg;
    }

    inline protobuf::RecoverMsg THUAI72ProtobufRecover(int64_t id)
    {
        protobuf::RecoverMsg recoverMsg;
        recoverMsg.set_ship_id(id);
        return recoverMsg;
    }

    inline protobuf::TargetMsg THUAI72ProtobufTarget(int64_t id, int32_t x, int32_t y)
    {
        protobuf::TargetMsg targetMsg;
        targetMsg.set_x(x);
        targetMsg.set_y(y);
        targetMsg.set_ship_id(id);
        return targetMsg;
    }

    inline protobuf::SendMsg THUAI72ProtobufSend(std::string msg, int64_t toID, bool binary, int64_t id)
    {
        protobuf::SendMsg sendMsg;
        if (binary)
            sendMsg.set_binary_message(std::move(msg));
        else
            sendMsg.set_text_message(std::move(msg));
        sendMsg.set_to_player_id(toID);
        sendMsg.set_player_id(id);
        return sendMsg;
    }

    inline protobuf::ConstructMsg THUAI72ProtobufConstruct(int64_t id, int32_t x, int32_t y)
    {
        protobuf::ConstructMsg constructMsg;
        constructMsg.set_x(x);
        constructMsg.set_y(y);
        constructMsg.set_building_id(id);
        return constructMsg;
    }

    inline protobuf::InstallMsg THUAI72ProtobufInstall(THUAI7::Module module, int64_t id)
    {
        protobuf::InstallMsg installMsg;
        installMsg.set_module_type(module.moduleType);
        installMsg.set_module_level(module.ModuleLevel);
        installMsg.set_ship_id(id);
        return installMsg;
    }

    inline protobuf::IDMsg THUAI72ProtobufID(int64_t shipID)
    {
        protobuf::IDMsg idMsg;
        idMsg.set_ship_id(shipID);
        return idMsg;
    }

    inline protobuf::ShipMsg THUAI72ProtobufShip(int64_t shipID, THUAI7::ShipType shipType, THUAI7::PlayerTeam playerTeam)
    {
        protobuf::ShipMsg shipMsg;
        shipMsg.set_ship_id(shipID);
        shipMsg.set_ship_type(shipTypeDict[shipType]);
        if (playerTeam == THUAI7::PlayerTeam::Up)
        {
            shipMsg.set_player_team(protobuf::PlayerTeam::UP);
        }
        else if (playerTeam == THUAI7::PlayerTeam::Down)
        {
            shipMsg.set_player_team(protobuf::PlayerTeam::DOWN);
        }
        return shipMsg;
    }

    // 用于将THUAI7的类转换为Protobuf的类
}



#endif