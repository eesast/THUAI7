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

    inline bool HaveView(int32_t x, int32_t y, int32_t newX, int32_t newY, int32_t viewRange, std::vector<std::vector<THUAI7::PlaceType>>& map)
    {
        int32_t deltaX = newX - x;
        int32_t deltaY = newY - y;
        double distance = deltaX * deltaX + deltaY * deltaY;
        THUAI7::PlaceType myPlace = map[GridToCell(x)][GridToCell(y)];
        THUAI7::PlaceType newPlace = map[GridToCell(newX)][GridToCell(newY)];
        if (newPlace == THUAI7::PlaceType::Grass && myPlace != THUAI7::PlaceType::Grass)
            return false;
        int32_t divide = std::max(std::abs(deltaX), std::abs(deltaY)) / 100;
        if (divide == 0)
            return true;
        double dx = deltaX / divide;
        double dy = deltaY / divide;
        double myX = double(x);
        double myY = double(y);
        if (newPlace == THUAI7::PlaceType::Grass && myPlace == THUAI7::PlaceType::Grass)
            for (int32_t i = 0; i < divide; i++)
            {
                myX += dx;
                myY += dy;
                if (map[GridToCell(myX)][GridToCell(myY)] != THUAI7::PlaceType::Grass)
                    return false;
            }
        else
            for (int32_t i = 0; i < divide; i++)
            {
                myX += dx;
                myY += dy;
                if (map[GridToCell(myX)][GridToCell(myY)] == THUAI7::PlaceType::Wall)
                    return false;
            }
        return true;
    }
}  // namespace AssistFunction

// 需要修改
namespace Proto2THUAI7
{
    // 用于将Protobuf中的枚举转换为THUAI7的枚举
    inline std::map<protobuf::GameState, THUAI7::GameState> gameStateDict{
        {protobuf::GameState::NULL_GAME_STATE, THUAI7::GameState::NullGameState},
        {protobuf::GameState::GAME_START, THUAI7::GameState::GameStart},
        {protobuf::GameState::GAME_RUNNING, THUAI7::GameState::GameRunning},
        {protobuf::GameState::GAME_END, THUAI7::GameState::GameEnd},
    };

    inline std::map<protobuf::PlaceType, THUAI7::PlaceType> placeTypeDict{
        {protobuf::PlaceType::NULL_PLACE_TYPE, THUAI7::PlaceType::NullPlaceType},
        {protobuf::PlaceType::HOME, THUAI7::PlaceType::Home},
        {protobuf::PlaceType::GROUND, THUAI7::PlaceType::Ground},
        {protobuf::PlaceType::WALL, THUAI7::PlaceType::Wall},
        {protobuf::PlaceType::GRASS, THUAI7::PlaceType::Grass},
        {protobuf::PlaceType::RIVER, THUAI7::PlaceType::River},
        {protobuf::PlaceType::GARBAGE, THUAI7::PlaceType::Garbage},
        {protobuf::PlaceType::CONSTRUCTION, THUAI7::PlaceType::Construction},
        {protobuf::PlaceType::BRIDGE, THUAI7::PlaceType::Bridge},
    };

    inline std::map<protobuf::ShapeType, THUAI7::ShapeType> shapeTypeDict{
        {protobuf::ShapeType::NULL_SHAPE_TYPE, THUAI7::ShapeType::NullShapeType},
        {protobuf::ShapeType::CIRCLE, THUAI7::ShapeType::Circle},
        {protobuf::ShapeType::SQUARE, THUAI7::ShapeType::Square},
    };

    inline std::map<protobuf::PlayerType, THUAI7::PlayerType> playerTypeDict{
        {protobuf::PlayerType::NULL_PLAYER_TYPE, THUAI7::PlayerType::NullPlayerType},
        {protobuf::PlayerType::SWEEPER, THUAI7::PlayerType::Sweeper},
        {protobuf::PlayerType::TEAM, THUAI7::PlayerType::Team},
    };

    inline std::map<protobuf::SweeperType, THUAI7::SweeperType> SweeperTypeDict{
        {protobuf::SweeperType::NULL_SWEEPER_TYPE, THUAI7::SweeperType::NullSweeperType},
        {protobuf::SweeperType::CIVILIAN_SWEEPER, THUAI7::SweeperType::CivilianSweeper},
        {protobuf::SweeperType::MILITARY_SWEEPER, THUAI7::SweeperType::MilitarySweeper},
        {protobuf::SweeperType::FLAG_SWEEPER, THUAI7::SweeperType::FlagSweeper},
    };

    inline std::map<protobuf::SweeperState, THUAI7::SweeperState> SweeperStateDict{
        {protobuf::SweeperState::NULL_STATUS, THUAI7::SweeperState::NullStatus},
        {protobuf::SweeperState::IDLE, THUAI7::SweeperState::Idle},
        {protobuf::SweeperState::PRODUCING, THUAI7::SweeperState::Producing},
        {protobuf::SweeperState::CONSTRUCTING, THUAI7::SweeperState::Constructing},
        {protobuf::SweeperState::RECOVERING, THUAI7::SweeperState::Recovering},
        {protobuf::SweeperState::RECYCLING, THUAI7::SweeperState::Recycling},
        {protobuf::SweeperState::ATTACKING, THUAI7::SweeperState::Attacking},
        {protobuf::SweeperState::SWINGING, THUAI7::SweeperState::Swinging},
        {protobuf::SweeperState::STUNNED, THUAI7::SweeperState::Stunned},
        {protobuf::SweeperState::MOVING, THUAI7::SweeperState::Moving},
    };

    inline std::map<protobuf::WeaponType, THUAI7::WeaponType> weaponTypeDict{
        {protobuf::WeaponType::NULL_WEAPON_TYPE, THUAI7::WeaponType::NullWeaponType},
        {protobuf::WeaponType::LASERGUN, THUAI7::WeaponType::LaserGun},
        {protobuf::WeaponType::PLASMAGUN, THUAI7::WeaponType::PlasmaGun},
        {protobuf::WeaponType::SHELLGUN, THUAI7::WeaponType::ShellGun},
        {protobuf::WeaponType::MISSILEGUN, THUAI7::WeaponType::MissileGun},
        {protobuf::WeaponType::ARCGUN, THUAI7::WeaponType::ArcGun},
    };

    inline std::map<protobuf::ConstructorType, THUAI7::ConstructorType> constructorTypeDict{
        {protobuf::ConstructorType::NULL_CONSTRUCTOR_TYPE, THUAI7::ConstructorType::NullConstructorType},
        {protobuf::ConstructorType::CONSTRUCTOR1, THUAI7::ConstructorType::Constructor1},
        {protobuf::ConstructorType::CONSTRUCTOR2, THUAI7::ConstructorType::Constructor2},
        {protobuf::ConstructorType::CONSTRUCTOR3, THUAI7::ConstructorType::Constructor3},
    };

    inline std::map<protobuf::ArmorType, THUAI7::ArmorType> armorTypeDict{
        {protobuf::ArmorType::NULL_ARMOR_TYPE, THUAI7::ArmorType::NullArmorType},
        {protobuf::ArmorType::ARMOR1, THUAI7::ArmorType::Armor1},
        {protobuf::ArmorType::ARMOR2, THUAI7::ArmorType::Armor2},
        {protobuf::ArmorType::ARMOR3, THUAI7::ArmorType::Armor3},
    };

    inline std::map<protobuf::ShieldType, THUAI7::ShieldType> shieldTypeDict{
        {protobuf::ShieldType::NULL_SHIELD_TYPE, THUAI7::ShieldType::NullShieldType},
        {protobuf::ShieldType::SHIELD1, THUAI7::ShieldType::Shield1},
        {protobuf::ShieldType::SHIELD2, THUAI7::ShieldType::Shield2},
        {protobuf::ShieldType::SHIELD3, THUAI7::ShieldType::Shield3},
    };

    inline std::map<protobuf::ProducerType, THUAI7::ProducerType> producerTypeDict{
        {protobuf::ProducerType::NULL_PRODUCER_TYPE, THUAI7::ProducerType::NullProducerType},
        {protobuf::ProducerType::PRODUCER1, THUAI7::ProducerType::Producer1},
        {protobuf::ProducerType::PRODUCER2, THUAI7::ProducerType::Producer2},
        {protobuf::ProducerType::PRODUCER3, THUAI7::ProducerType::Producer3},
    };

    inline std::map<protobuf::ModuleType, THUAI7::ModuleType> moduleTypeDict{
        {protobuf::ModuleType::NULL_MODULE_TYPE, THUAI7::ModuleType::NullModuleType},
        {protobuf::ModuleType::MODULE_PRODUCER1, THUAI7::ModuleType::ModuleProducer1},
        {protobuf::ModuleType::MODULE_PRODUCER2, THUAI7::ModuleType::ModuleProducer2},
        {protobuf::ModuleType::MODULE_PRODUCER3, THUAI7::ModuleType::ModuleProducer3},
        {protobuf::ModuleType::MODULE_CONSTRUCTOR1, THUAI7::ModuleType::ModuleConstructor1},
        {protobuf::ModuleType::MODULE_CONSTRUCTOR2, THUAI7::ModuleType::ModuleConstructor2},
        {protobuf::ModuleType::MODULE_CONSTRUCTOR3, THUAI7::ModuleType::ModuleConstructor3},
        {protobuf::ModuleType::MODULE_ARMOR1, THUAI7::ModuleType::ModuleArmor1},
        {protobuf::ModuleType::MODULE_ARMOR2, THUAI7::ModuleType::ModuleArmor2},
        {protobuf::ModuleType::MODULE_ARMOR3, THUAI7::ModuleType::ModuleArmor3},
        {protobuf::ModuleType::MODULE_SHIELD1, THUAI7::ModuleType::ModuleShield1},
        {protobuf::ModuleType::MODULE_SHIELD2, THUAI7::ModuleType::ModuleShield2},
        {protobuf::ModuleType::MODULE_SHIELD3, THUAI7::ModuleType::ModuleShield3},
        {protobuf::ModuleType::MODULE_LASERGUN, THUAI7::ModuleType::ModuleLaserGun},
        {protobuf::ModuleType::MODULE_PLASMAGUN, THUAI7::ModuleType::ModulePlasmaGun},
        {protobuf::ModuleType::MODULE_SHELLGUN, THUAI7::ModuleType::ModuleShellGun},
        {protobuf::ModuleType::MODULE_MISSILEGUN, THUAI7::ModuleType::ModuleMissileGun},
        {protobuf::ModuleType::MODULE_ARCGUN, THUAI7::ModuleType::ModuleArcGun},
    };

    inline std::map<protobuf::BulletType, THUAI7::BulletType> bulletTypeDict{
        {protobuf::BulletType::NULL_BULLET_TYPE, THUAI7::BulletType::NullBulletType},
        {protobuf::BulletType::LASER, THUAI7::BulletType::Laser},
        {protobuf::BulletType::PLASMA, THUAI7::BulletType::Plasma},
        {protobuf::BulletType::SHELL, THUAI7::BulletType::Shell},
        {protobuf::BulletType::MISSILE, THUAI7::BulletType::Missile},
        {protobuf::BulletType::ARC, THUAI7::BulletType::Arc},
    };

    inline std::map<protobuf::ConstructionType, THUAI7::ConstructionType> constructionTypeDict{
        {protobuf::ConstructionType::NULL_CONSTRUCTION_TYPE, THUAI7::ConstructionType::NullConstructionType},
        {protobuf::ConstructionType::RECYCLEBANK, THUAI7::ConstructionType::RecycleBank},
        {protobuf::ConstructionType::CHARGESTATION, THUAI7::ConstructionType::ChargeStation},
        {protobuf::ConstructionType::SIGNALTOWER, THUAI7::ConstructionType::SignalTower},
    };

    inline std::map<protobuf::PlayerTeam, THUAI7::PlayerTeam> playerTeamDict{
        {protobuf::PlayerTeam::NULL_TEAM, THUAI7::PlayerTeam::NullTeam},
        {protobuf::PlayerTeam::RED, THUAI7::PlayerTeam::Red},
        {protobuf::PlayerTeam::BLUE, THUAI7::PlayerTeam::Blue},
    };

    inline std::map<protobuf::MessageOfObj::MessageOfObjCase, THUAI7::MessageOfObj> messageOfObjDict{
        {protobuf::MessageOfObj::MessageOfObjCase::kSweeperMessage, THUAI7::MessageOfObj::SweeperMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kBulletMessage, THUAI7::MessageOfObj::BulletMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kRecyclebankMessage, THUAI7::MessageOfObj::RecycleBankMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kChargestationMessage, THUAI7::MessageOfObj::ChargeStationMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kSignaltowerMessage, THUAI7::MessageOfObj::SignalTowerMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kBridgeMessage, THUAI7::MessageOfObj::BridgeMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kHomeMessage, THUAI7::MessageOfObj::HomeMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kGarbageMessage, THUAI7::MessageOfObj::GarbageMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kMapMessage, THUAI7::MessageOfObj::MapMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kNewsMessage, THUAI7::MessageOfObj::NewsMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kBombedBulletMessage, THUAI7::MessageOfObj::BombedBulletMessage},
        {protobuf::MessageOfObj::MessageOfObjCase::kTeamMessage, THUAI7::MessageOfObj::TeamMessage},
    };

    inline std::map<protobuf::MessageOfNews::NewsCase, THUAI7::NewsType> newsTypeDict{
        {protobuf::MessageOfNews::NewsCase::NEWS_NOT_SET, THUAI7::NewsType::NullNewsType},
        {protobuf::MessageOfNews::NewsCase::kTextMessage, THUAI7::NewsType::TextMessage},
        {protobuf::MessageOfNews::NewsCase::kBinaryMessage, THUAI7::NewsType::BinaryMessage},
    };

    // 用于将Protobuf中的类转换为THUAI7的类
    inline std::shared_ptr<THUAI7::Sweeper> Protobuf2THUAI7Sweeper(const protobuf::MessageOfSweeper& SweeperMsg)
    {
        auto Sweeper = std::make_shared<THUAI7::Sweeper>();
        Sweeper->x = SweeperMsg.x();
        Sweeper->y = SweeperMsg.y();
        Sweeper->speed = SweeperMsg.speed();
        Sweeper->hp = SweeperMsg.hp();
        Sweeper->armor = SweeperMsg.armor();
        Sweeper->shield = SweeperMsg.shield();
        Sweeper->teamID = SweeperMsg.team_id();
        Sweeper->playerID = SweeperMsg.player_id();
        Sweeper->guid = SweeperMsg.guid();
        Sweeper->sweeperState = SweeperStateDict[SweeperMsg.sweeper_state()];
        Sweeper->sweeperType = SweeperTypeDict[SweeperMsg.sweeper_type()];
        Sweeper->viewRange = SweeperMsg.view_range();
        Sweeper->producerType = producerTypeDict[SweeperMsg.producer_type()];
        Sweeper->constructorType = constructorTypeDict[SweeperMsg.constructor_type()];
        Sweeper->armorType = armorTypeDict[SweeperMsg.armor_type()];
        Sweeper->shieldType = shieldTypeDict[SweeperMsg.shield_type()];
        Sweeper->weaponType = weaponTypeDict[SweeperMsg.weapon_type()];
        Sweeper->facingDirection = SweeperMsg.facing_direction();
        return Sweeper;
    }

    inline std::shared_ptr<THUAI7::Bullet> Protobuf2THUAI7Bullet(const protobuf::MessageOfBullet& bulletMsg)
    {
        auto bullet = std::make_shared<THUAI7::Bullet>();
        bullet->bulletType = bulletTypeDict[bulletMsg.type()];
        bullet->x = bulletMsg.x();
        bullet->y = bulletMsg.y();
        bullet->facingDirection = bulletMsg.facing_direction();
        bullet->damage = bulletMsg.damage();
        bullet->teamID = bulletMsg.team_id();
        bullet->guid = bulletMsg.guid();
        bullet->speed = bulletMsg.speed();
        bullet->bombRange = bulletMsg.bomb_range();
        return bullet;
    }

    inline std::shared_ptr<THUAI7::Home> Protobuf2THUAI7Home(const protobuf::MessageOfHome& homeMsg)
    {
        auto home = std::make_shared<THUAI7::Home>();
        home->x = homeMsg.x();
        home->y = homeMsg.y();
        home->hp = homeMsg.hp();
        home->teamID = homeMsg.team_id();
        return home;
    }

    inline std::shared_ptr<THUAI7::Team> Protobuf2THUAI7Team(const protobuf::MessageOfTeam& teamMsg)
    {
        auto team = std::make_shared<THUAI7::Team>();
        team->playerID = teamMsg.player_id();
        team->teamID = teamMsg.team_id();
        team->score = teamMsg.score();
        team->money = teamMsg.money();
        return team;
    }

    inline std::shared_ptr<THUAI7::GameInfo> Protobuf2THUAI7GameInfo(const protobuf::MessageOfAll& allMsg)
    {
        auto gameInfo = std::make_shared<THUAI7::GameInfo>();
        gameInfo->gameTime = allMsg.game_time();
        gameInfo->redScore = allMsg.red_team_score();
        gameInfo->redMoney = allMsg.red_team_score();
        gameInfo->redHomeHp = allMsg.red_home_hp();
        gameInfo->blueScore = allMsg.blue_team_score();
        gameInfo->blueMoney = allMsg.blue_team_score();
        gameInfo->blueHomeHp = allMsg.blue_home_hp();
        return gameInfo;
    }
}  // namespace Proto2THUAI7
// 辅助函数，用于将proto信息转换为THUAI7信息
namespace THUAI72Proto
{
    // 用于将THUAI7的枚举转换为Protobuf的枚举
    inline std::map<THUAI7::GameState, protobuf::GameState> gameStateDict{
        {THUAI7::GameState::NullGameState, protobuf::GameState::NULL_GAME_STATE},
        {THUAI7::GameState::GameStart, protobuf::GameState::GAME_START},
        {THUAI7::GameState::GameRunning, protobuf::GameState::GAME_RUNNING},
        {THUAI7::GameState::GameEnd, protobuf::GameState::GAME_END},
    };

    inline std::map<THUAI7::PlaceType, protobuf::PlaceType> placeTypeDict{
        {THUAI7::PlaceType::NullPlaceType, protobuf::PlaceType::NULL_PLACE_TYPE},
        {THUAI7::PlaceType::Home, protobuf::PlaceType::HOME},
        {THUAI7::PlaceType::Ground, protobuf::PlaceType::GROUND},
        {THUAI7::PlaceType::Wall, protobuf::PlaceType::WALL},
        {THUAI7::PlaceType::Grass, protobuf::PlaceType::GRASS},
        {THUAI7::PlaceType::River, protobuf::PlaceType::RIVER},
        {THUAI7::PlaceType::Garbage, protobuf::PlaceType::GARBAGE},
        {THUAI7::PlaceType::Construction, protobuf::PlaceType::CONSTRUCTION},
        {THUAI7::PlaceType::Bridge, protobuf::PlaceType::BRIDGE},
    };

    inline std::map<THUAI7::ShapeType, protobuf::ShapeType> shapeTypeDict{
        {THUAI7::ShapeType::NullShapeType, protobuf::ShapeType::NULL_SHAPE_TYPE},
        {THUAI7::ShapeType::Circle, protobuf::ShapeType::CIRCLE},
        {THUAI7::ShapeType::Square, protobuf::ShapeType::SQUARE},
    };

    inline std::map<THUAI7::PlayerType, protobuf::PlayerType> playerTypeDict{
        {THUAI7::PlayerType::NullPlayerType, protobuf::PlayerType::NULL_PLAYER_TYPE},
        {THUAI7::PlayerType::Sweeper, protobuf::PlayerType::SWEEPER},
        {THUAI7::PlayerType::Team, protobuf::PlayerType::TEAM},
    };

    inline std::map<THUAI7::SweeperType, protobuf::SweeperType> SweeperTypeDict{
        {THUAI7::SweeperType::NullSweeperType, protobuf::SweeperType::NULL_SWEEPER_TYPE},
        {THUAI7::SweeperType::CivilianSweeper, protobuf::SweeperType::CIVILIAN_SWEEPER},
        {THUAI7::SweeperType::MilitarySweeper, protobuf::SweeperType::MILITARY_SWEEPER},
        {THUAI7::SweeperType::FlagSweeper, protobuf::SweeperType::FLAG_SWEEPER},
    };

    inline std::map<THUAI7::SweeperState, protobuf::SweeperState> SweeperStateDict{
        {THUAI7::SweeperState::NullStatus, protobuf::SweeperState::NULL_STATUS},
        {THUAI7::SweeperState::Idle, protobuf::SweeperState::IDLE},
        {THUAI7::SweeperState::Producing, protobuf::SweeperState::PRODUCING},
        {THUAI7::SweeperState::Constructing, protobuf::SweeperState::CONSTRUCTING},
        {THUAI7::SweeperState::Recovering, protobuf::SweeperState::RECOVERING},
        {THUAI7::SweeperState::Recycling, protobuf::SweeperState::RECYCLING},
        {THUAI7::SweeperState::Attacking, protobuf::SweeperState::ATTACKING},
        {THUAI7::SweeperState::Swinging, protobuf::SweeperState::SWINGING},
        {THUAI7::SweeperState::Stunned, protobuf::SweeperState::STUNNED},
        {THUAI7::SweeperState::Moving, protobuf::SweeperState::MOVING},
    };

    inline std::map<THUAI7::WeaponType, protobuf::WeaponType> weaponTypeDict{
        {THUAI7::WeaponType::NullWeaponType, protobuf::WeaponType::NULL_WEAPON_TYPE},
        {THUAI7::WeaponType::LaserGun, protobuf::WeaponType::LASERGUN},
        {THUAI7::WeaponType::PlasmaGun, protobuf::WeaponType::PLASMAGUN},
        {THUAI7::WeaponType::ShellGun, protobuf::WeaponType::SHELLGUN},
        {THUAI7::WeaponType::MissileGun, protobuf::WeaponType::MISSILEGUN},
        {THUAI7::WeaponType::ArcGun, protobuf::WeaponType::ARCGUN},
    };

    inline std::map<THUAI7::ConstructorType, protobuf::ConstructorType> constructorTypeDict{
        {THUAI7::ConstructorType::NullConstructorType, protobuf::ConstructorType::NULL_CONSTRUCTOR_TYPE},
        {THUAI7::ConstructorType::Constructor1, protobuf::ConstructorType::CONSTRUCTOR1},
        {THUAI7::ConstructorType::Constructor2, protobuf::ConstructorType::CONSTRUCTOR2},
        {THUAI7::ConstructorType::Constructor3, protobuf::ConstructorType::CONSTRUCTOR3},
    };

    inline std::map<THUAI7::ArmorType, protobuf::ArmorType> armorTypeDict{
        {THUAI7::ArmorType::NullArmorType, protobuf::ArmorType::NULL_ARMOR_TYPE},
        {THUAI7::ArmorType::Armor1, protobuf::ArmorType::ARMOR1},
        {THUAI7::ArmorType::Armor2, protobuf::ArmorType::ARMOR2},
        {THUAI7::ArmorType::Armor3, protobuf::ArmorType::ARMOR3},
    };

    inline std::map<THUAI7::ShieldType, protobuf::ShieldType> shieldTypeDict{
        {THUAI7::ShieldType::NullShieldType, protobuf::ShieldType::NULL_SHIELD_TYPE},
        {THUAI7::ShieldType::Shield1, protobuf::ShieldType::SHIELD1},
        {THUAI7::ShieldType::Shield2, protobuf::ShieldType::SHIELD2},
        {THUAI7::ShieldType::Shield3, protobuf::ShieldType::SHIELD3},
    };

    inline std::map<THUAI7::ProducerType, protobuf::ProducerType> producerTypeDict{
        {THUAI7::ProducerType::NullProducerType, protobuf::ProducerType::NULL_PRODUCER_TYPE},
        {THUAI7::ProducerType::Producer1, protobuf::ProducerType::PRODUCER1},
        {THUAI7::ProducerType::Producer2, protobuf::ProducerType::PRODUCER2},
        {THUAI7::ProducerType::Producer3, protobuf::ProducerType::PRODUCER3},
    };

    inline std::map<THUAI7::ModuleType, protobuf::ModuleType> moduleTypeDict{
        {THUAI7::ModuleType::NullModuleType, protobuf::ModuleType::NULL_MODULE_TYPE},
        {THUAI7::ModuleType::ModuleProducer1, protobuf::ModuleType::MODULE_PRODUCER1},
        {THUAI7::ModuleType::ModuleProducer2, protobuf::ModuleType::MODULE_PRODUCER2},
        {THUAI7::ModuleType::ModuleProducer3, protobuf::ModuleType::MODULE_PRODUCER3},
        {THUAI7::ModuleType::ModuleConstructor1, protobuf::ModuleType::MODULE_CONSTRUCTOR1},
        {THUAI7::ModuleType::ModuleConstructor2, protobuf::ModuleType::MODULE_CONSTRUCTOR2},
        {THUAI7::ModuleType::ModuleConstructor3, protobuf::ModuleType::MODULE_CONSTRUCTOR3},
        {THUAI7::ModuleType::ModuleArmor1, protobuf::ModuleType::MODULE_ARMOR1},
        {THUAI7::ModuleType::ModuleArmor2, protobuf::ModuleType::MODULE_ARMOR2},
        {THUAI7::ModuleType::ModuleArmor3, protobuf::ModuleType::MODULE_ARMOR3},
        {THUAI7::ModuleType::ModuleShield1, protobuf::ModuleType::MODULE_SHIELD1},
        {THUAI7::ModuleType::ModuleShield2, protobuf::ModuleType::MODULE_SHIELD2},
        {THUAI7::ModuleType::ModuleShield3, protobuf::ModuleType::MODULE_SHIELD3},
        {THUAI7::ModuleType::ModuleLaserGun, protobuf::ModuleType::MODULE_LASERGUN},
        {THUAI7::ModuleType::ModulePlasmaGun, protobuf::ModuleType::MODULE_PLASMAGUN},
        {THUAI7::ModuleType::ModuleShellGun, protobuf::ModuleType::MODULE_SHELLGUN},
        {THUAI7::ModuleType::ModuleMissileGun, protobuf::ModuleType::MODULE_MISSILEGUN},
        {THUAI7::ModuleType::ModuleArcGun, protobuf::ModuleType::MODULE_ARCGUN},
    };

    inline std::map<THUAI7::BulletType, protobuf::BulletType> bulletTypeDict{
        {THUAI7::BulletType::NullBulletType, protobuf::BulletType::NULL_BULLET_TYPE},
        {THUAI7::BulletType::Laser, protobuf::BulletType::LASER},
        {THUAI7::BulletType::Plasma, protobuf::BulletType::PLASMA},
        {THUAI7::BulletType::Shell, protobuf::BulletType::SHELL},
        {THUAI7::BulletType::Missile, protobuf::BulletType::MISSILE},
        {THUAI7::BulletType::Arc, protobuf::BulletType::ARC},
    };

    inline std::map<THUAI7::ConstructionType, protobuf::ConstructionType> constructionTypeDict{
        {THUAI7::ConstructionType::NullConstructionType, protobuf::ConstructionType::NULL_CONSTRUCTION_TYPE},
        {THUAI7::ConstructionType::RecycleBank, protobuf::ConstructionType::RECYCLEBANK},
        {THUAI7::ConstructionType::ChargeStation, protobuf::ConstructionType::CHARGESTATION},
        {THUAI7::ConstructionType::SignalTower, protobuf::ConstructionType::SIGNALTOWER},
    };

    inline std::map<THUAI7::PlayerTeam, protobuf::PlayerTeam> playerTeamDict{
        {THUAI7::PlayerTeam::NullTeam, protobuf::PlayerTeam::NULL_TEAM},
        {THUAI7::PlayerTeam::Red, protobuf::PlayerTeam::RED},
        {THUAI7::PlayerTeam::Blue, protobuf::PlayerTeam::BLUE},
    };

    inline std::map<THUAI7::MessageOfObj, protobuf::MessageOfObj::MessageOfObjCase> messageOfObjDict{
        {THUAI7::MessageOfObj::SweeperMessage, protobuf::MessageOfObj::MessageOfObjCase::kSweeperMessage},
        {THUAI7::MessageOfObj::BulletMessage, protobuf::MessageOfObj::MessageOfObjCase::kBulletMessage},
        {THUAI7::MessageOfObj::RecycleBankMessage, protobuf::MessageOfObj::MessageOfObjCase::kRecyclebankMessage},
        {THUAI7::MessageOfObj::ChargeStationMessage, protobuf::MessageOfObj::MessageOfObjCase::kChargestationMessage},
        {THUAI7::MessageOfObj::SignalTowerMessage, protobuf::MessageOfObj::MessageOfObjCase::kSignaltowerMessage},
        {THUAI7::MessageOfObj::BridgeMessage, protobuf::MessageOfObj::MessageOfObjCase::kBridgeMessage},
        {THUAI7::MessageOfObj::HomeMessage, protobuf::MessageOfObj::MessageOfObjCase::kHomeMessage},
        {THUAI7::MessageOfObj::GarbageMessage, protobuf::MessageOfObj::MessageOfObjCase::kGarbageMessage},
        {THUAI7::MessageOfObj::MapMessage, protobuf::MessageOfObj::MessageOfObjCase::kMapMessage},
        {THUAI7::MessageOfObj::NewsMessage, protobuf::MessageOfObj::MessageOfObjCase::kNewsMessage},
        {THUAI7::MessageOfObj::BombedBulletMessage, protobuf::MessageOfObj::MessageOfObjCase::kBombedBulletMessage},
        {THUAI7::MessageOfObj::TeamMessage, protobuf::MessageOfObj::MessageOfObjCase::kTeamMessage},
    };

    inline std::map<THUAI7::NewsType, protobuf::MessageOfNews::NewsCase> newsTypeDict{
        {THUAI7::NewsType::NullNewsType, protobuf::MessageOfNews::NewsCase::NEWS_NOT_SET},
        {THUAI7::NewsType::TextMessage, protobuf::MessageOfNews::NewsCase::kTextMessage},
        {THUAI7::NewsType::BinaryMessage, protobuf::MessageOfNews::NewsCase::kBinaryMessage},
    };

    inline protobuf::MoveMsg THUAI72ProtobufMoveMsg(int64_t playerID, int64_t teamID, int64_t time, double angle)
    {
        protobuf::MoveMsg moveMsg;
        moveMsg.set_time_in_milliseconds(time);
        moveMsg.set_angle(angle);
        moveMsg.set_player_id(playerID);
        moveMsg.set_team_id(teamID);
        return moveMsg;
    }

    inline protobuf::IDMsg THUAI72ProtobufIDMsg(int64_t playerID, int64_t teamID)
    {
        protobuf::IDMsg IDMsg;
        IDMsg.set_player_id(playerID);
        IDMsg.set_team_id(teamID);
        return IDMsg;
    }

    inline protobuf::RecoverMsg THUAI72ProtobufRecoverMsg(int64_t playerID, int64_t recover, int64_t teamID)
    {
        protobuf::RecoverMsg RecoverMsg;
        RecoverMsg.set_player_id(playerID);
        RecoverMsg.set_recover(recover);
        RecoverMsg.set_team_id(teamID);
        return RecoverMsg;
    }

    inline protobuf::ConstructMsg THUAI72ProtobufConstructMsg(int64_t playerID, int64_t teamID, THUAI7::ConstructionType constructionType)
    {
        protobuf::ConstructMsg constructMsg;
        constructMsg.set_player_id(playerID);
        constructMsg.set_team_id(teamID);
        constructMsg.set_construction_type(THUAI72Proto::constructionTypeDict[constructionType]);
        return constructMsg;
    }

    inline protobuf::AttackMsg THUI72ProtobufAttackMsg(int64_t playerID, int64_t teamID, double angle)
    {
        protobuf::AttackMsg attackMsg;
        attackMsg.set_player_id(playerID);
        attackMsg.set_team_id(teamID);
        attackMsg.set_angle(angle);
        return attackMsg;
    }

    inline protobuf::SendMsg THUAI72ProtobufSendMsg(int64_t playerID, int64_t toPlayerID, int64_t teamID, std::string msg, bool binary)
    {
        protobuf::SendMsg sendMsg;
        if (binary)
            sendMsg.set_binary_message(std::move(msg));
        else
            sendMsg.set_text_message(std::move(msg));
        sendMsg.set_to_player_id(toPlayerID);
        sendMsg.set_player_id(playerID);
        sendMsg.set_team_id(teamID);
        return sendMsg;
    }

    inline protobuf::InstallMsg THUAI72ProtobufInstallMsg(int64_t playerID, int64_t teamID, THUAI7::ModuleType moduleType)
    {
        protobuf::InstallMsg installMsg;
        installMsg.set_module_type(THUAI72Proto::moduleTypeDict[moduleType]);
        installMsg.set_player_id(playerID);
        installMsg.set_team_id(teamID);
        return installMsg;
    }

    inline protobuf::BuildSweeperMsg THUAI72ProtobufBuildSweeperMsg(int64_t teamID, THUAI7::SweeperType SweeperType, int32_t x, int32_t y)
    {
        protobuf::BuildSweeperMsg buildSweeperMsg;
        buildSweeperMsg.set_team_id(teamID);
        buildSweeperMsg.set_x(x);
        buildSweeperMsg.set_y(y);
        buildSweeperMsg.set_sweeper_type(THUAI72Proto::SweeperTypeDict[SweeperType]);
        return buildSweeperMsg;
    }

    inline protobuf::PlayerMsg THUAI72ProtobufPlayerMsg(int64_t playerID, int64_t teamID, THUAI7::SweeperType SweeperType, int32_t x, int32_t y)
    {
        protobuf::PlayerMsg playerMsg;
        playerMsg.set_player_id(playerID);
        playerMsg.set_team_id(teamID);
        playerMsg.set_sweeper_type(THUAI72Proto::SweeperTypeDict[SweeperType]);
        playerMsg.set_x(x);
        playerMsg.set_y(y);
        return playerMsg;
    }

    // 用于将THUAI7的类转换为Protobuf的类
}  // namespace THUAI72Proto
namespace Time
{
    inline double TimeSinceStart(const std::chrono::system_clock::time_point& sp)
    {
        std::chrono::system_clock::time_point tp = std::chrono::system_clock::now();
        std::chrono::duration<double, std::milli> time_span = std::chrono::duration_cast<std::chrono::duration<double, std::milli>>(tp - sp);
        return time_span.count();
    }
}  // namespace Time
#endif