#pragma once
#ifndef STRUCTURES_H
#define STRUCTURES_H

#include <cstdint>
#include <array>
#include <map>
#include <vector>
#include <string>

#undef GetMessage
#undef SendMessage
#undef PeekMessage

namespace THUAI7
{
    // 游戏状态
    enum class GameState : unsigned char
    {
        NullGameState = 0,
        GameStart = 1,
        GameRunning = 2,
        GameEnd = 3,
    };
    // 所有NullXXXType均为错误类型，其余为可能出现的正常类型

    // 位置标志
    enum class PlaceType : unsigned char
    {
        NullPlaceType = 0,
        Home = 1,
        Space = 2,
        Ruin = 3,
        Shadow = 4,
        Asteroid = 5,
        Resource = 6,
        Construction = 7,
        Wormhole = 8,
    };

    // 形状标志
    enum class ShapeType : unsigned char
    {
        NullShapeType = 0,
        Circle = 1,
        Square = 2,
    };

    enum class PlayerTeam : unsigned char
    {
        Red = 0,
        Blue = 1,
        NullTeam = 2,
    };

    enum class PlayerType : unsigned char
    {
        NullPlayerType = 0,
        Ship = 1,
        Team = 2,
    };

    enum class ShipType : unsigned char
    {
        NullShipType = 0,
        CivilianShip = 1,
        MilitaryShip = 2,
        FlagShip = 3,
    };

    enum class WeaponType : unsigned char
    {
        NullWeaponType = 0,
        LaserGun = 1,
        PlasmaGun = 2,
        ShellGun = 3,
        MissileGun = 4,
        ArcGun = 5,
    };

    enum class ConstructorType : unsigned char
    {
        NullConstructorType = 0,
        Constructor1 = 1,
        Constructor2 = 2,
        Constructor3 = 3,
    };

    enum class ArmorType : unsigned char
    {
        NullArmorType = 0,
        Armor1 = 1,
        Armor2 = 2,
        Armor3 = 3,
    };

    enum class ShieldType : unsigned char
    {
        NullShieldType = 0,
        Shield1 = 1,
        Shield2 = 2,
        Shield3 = 3,
    };

    enum class ProducerType : unsigned char
    {
        NullProducerType = 0,
        Producer1 = 1,
        Producer2 = 2,
        Producer3 = 3,
    };

    enum class ModuleType : unsigned char
    {
        NullModuleType = 0,
        ModuleProducer1 = 1,
        ModuleProducer2 = 2,
        ModuleProducer3 = 3,
        ModuleConstructor1 = 4,
        ModuleConstructor2 = 5,
        ModuleConstructor3 = 6,
        ModuleArmor1 = 7,
        ModuleArmor2 = 8,
        ModuleArmor3 = 9,
        ModuleShield1 = 10,
        ModuleShield2 = 11,
        ModuleShield3 = 12,
        ModuleLaserGun = 13,
        ModulePlasmaGun = 14,
        ModuleShellGun = 15,
        ModuleMissileGun = 16,
        ModuleArcGun = 17,
    };

    enum class ShipState : unsigned char
    {
        NullStatus = 0,
        Idle = 1,
        Producing = 2,
        Constructing = 3,
        Recovering = 4,
        Recycling = 5,
        Attacking = 6,
        Swinging = 7,
        Stunned = 8,
        Moving = 9,
    };

    enum class BulletType : unsigned char
    {
        NullBulletType = 0,
        Laser = 1,
        Plasma = 2,
        Shell = 3,
        Missile = 4,
        Arc = 5,
    };

    enum class ConstructionType : unsigned char
    {
        NullConstructionType = 0,
        Factory = 1,
        Community = 2,
        Fort = 3,
    };

    enum class MessageOfObj : unsigned char
    {
        NullMessageOfObj = 0,
        ShipMessage = 1,
        BulletMessage = 2,
        FactoryMessage = 3,
        CommunityMessage = 4,
        FortMessage = 5,
        WormholeMessage = 6,
        HomeMessage = 7,
        ResourceMessage = 8,
        MapMessage = 9,
        NewsMessage = 10,
        BombedBulletMessage = 11,
        TeamMessage = 12,
    };

    enum class NewsType : unsigned char
    {
        NullNewsType = 0,
        TextMessage = 1,
        BinaryMessage = 2,
    };

    struct Ship
    {
        int32_t x;         // x坐标
        int32_t y;         // y坐标
        int32_t speed;     // 移动速度
        int32_t hp;        // 血量
        int32_t armor;     // 装甲
        int32_t shield;    // 护盾
        int64_t playerID;  // 船的id
        int64_t teamID;
        int64_t guid;         // 全局唯一ID
        ShipState shipState;  // 船所处状态
        ShipType shipType;
        int32_t viewRange;
        ProducerType producerType;
        ConstructorType constructorType;
        ArmorType armorType;
        ShieldType shieldType;
        WeaponType weaponType;
        double facingDirection;  // 朝向
    };

    struct Team
    {
        int64_t playerID;
        int64_t teamID;
        int64_t score;
        int64_t energy;
    };

    struct Home
    {
        int32_t x;
        int32_t y;
        int32_t hp;
        int64_t teamID;
        int64_t guid;
    };

    struct Bullet
    {
        int32_t x;               // x坐标
        int32_t y;               // y坐标
        double facingDirection;  // 朝向
        int64_t guid;            //
        int64_t teamID;          // 子弹所属队伍
        BulletType bulletType;   // 子弹类型
        int32_t damage;          // 伤害值
        int32_t attackRange;
        double bombRange;  // 炸弹爆炸范围
        int32_t speed;     // 子弹速度
    };

    struct ConstructionState
    {
        int64_t teamID;
        int32_t hp;
        ConstructionType constructionType;
        ConstructionState(std::pair<int64_t, int32_t> teamHP, ConstructionType type) :
            teamID(teamHP.first),
            hp(teamHP.second),
            constructionType(type)
        {
        }
    };

    // struct BombedBullet
    // {
    //     BulletType bulletType,
    //     int32_t x,
    //     int32_t y,
    //     double facingDirection,
    //     int64_t mappingID,
    //     double bombRange,
    // };

    using cellxy_t = std::pair<int32_t, int32_t>;

    struct GameMap
    {
        // x,y,id,hp
        std::map<cellxy_t, std::pair<int64_t, int32_t>> factoryState;
        std::map<cellxy_t, std::pair<int64_t, int32_t>> communityState;
        std::map<cellxy_t, std::pair<int64_t, int32_t>> fortState;
        std::map<cellxy_t, std::pair<int64_t, int32_t>> homeState;
        std::map<cellxy_t, int32_t> wormholeState;
        std::map<cellxy_t, int32_t> resourceState;
    };

    struct GameInfo
    {
        int32_t gameTime;
        int32_t redScore;
        int32_t redEnergy;
        int32_t redHomeHp;
        int32_t blueScore;
        int32_t blueEnergy;
        int32_t blueHomeHp;
    };

    // 仅供DEBUG使用，名称可改动
    // 还没写完，后面待续

    inline std::map<GameState, std::string> gameStateDict{
        {GameState::NullGameState, "NullGameState"},
        {GameState::GameStart, "GameStart"},
        {GameState::GameRunning, "GameRunning"},
        {GameState::GameEnd, "GameEnd"},
    };

    inline std::map<ShipType, std::string> shipTypeDict{
        {ShipType::NullShipType, "NullShipType"},
        {ShipType::CivilianShip, "CivilianShip"},
        {ShipType::MilitaryShip, "MilitaryShip"},
        {ShipType::FlagShip, "FlagShip"},
    };

    inline std::map<ShipState, std::string> shipStateDict{
        {ShipState::NullStatus, "NullState"},
        {ShipState::Idle, "Idle"},
        {ShipState::Producing, "Producing"},
        {ShipState::Constructing, "Constructing"},
        {ShipState::Recovering, "Recoverying"},
        {ShipState::Recycling, "Recycling"},
        {ShipState::Attacking, "Attacking"},
        {ShipState::Swinging, "Swinging"},
        {ShipState::Stunned, "Stunned"},
    };

    inline std::map<PlayerTeam, std::string> playerTeamDict{
        {PlayerTeam::NullTeam, "NullTeam"},
        {PlayerTeam::Red, "Red"},
        {PlayerTeam::Blue, "Blue"},
    };

    inline std::map<PlaceType, std::string> placeTypeDict{
        {PlaceType::NullPlaceType, "NullPlaceType"},
        {PlaceType::Home, "Home"},
        {PlaceType::Space, "Space"},
        {PlaceType::Ruin, "Ruin"},
        {PlaceType::Shadow, "Shadow"},
        {PlaceType::Asteroid, "Asteroid"},
        {PlaceType::Resource, "Resource"},
        {PlaceType::Construction, "Construction"},
    };

    inline std::map<BulletType, std::string> bulletTypeDict{
        {BulletType::NullBulletType, "NullBulletType"},
        {BulletType::Laser, "Laser"},
        {BulletType::Plasma, "Plasma"},
        {BulletType::Shell, "Shell"},
        {BulletType::Missile, "Missile"},
        {BulletType::Arc, "Arc"},
    };

    inline std::map<ConstructionType, std::string> constructionDict{
        {ConstructionType::NullConstructionType, "NullConstructionType"},
        {ConstructionType::Community, "Community"},
        {ConstructionType::Fort, "Fort"},
        {ConstructionType::Factory, "Factory"},
    };

    inline std::map<ModuleType, std::string> moduleTypeDict{
        {ModuleType::NullModuleType, "NullModuleType"},
        {ModuleType::ModuleProducer1, "ModuleProducer1"},
        {ModuleType::ModuleProducer2, "ModuleProducer2"},
        {ModuleType::ModuleProducer3, "ModuleProducer3"},
        {ModuleType::ModuleConstructor1, "ModuleConstructor1"},
        {ModuleType::ModuleConstructor2, "ModuleConstructor2"},
        {ModuleType::ModuleConstructor3, "ModuleConstructor3"},
        {ModuleType::ModuleArmor1, "ModuleArmor1"},
        {ModuleType::ModuleArmor2, "ModuleArmor2"},
        {ModuleType::ModuleArmor3, "ModuleArmor3"},
        {ModuleType::ModuleShield1, "ModuleShield1"},
        {ModuleType::ModuleShield2, "ModuleShield2"},
        {ModuleType::ModuleShield3, "ModuleShield3"},
        {ModuleType::ModuleLaserGun, "ModuleLaserGun"},
        {ModuleType::ModulePlasmaGun, "ModulePlasmaGun"},
        {ModuleType::ModuleShellGun, "ModuleShellGun"},
        {ModuleType::ModuleMissileGun, "ModuleMissileGun"},
        {ModuleType::ModuleArcGun, "ModuleArcGun"},
    };

    inline std::map<MessageOfObj, std::string> messageOfObjDict{
        {MessageOfObj::NullMessageOfObj, "NullMessageOfObj"},
        {MessageOfObj::ShipMessage, "ShipMessage"},
        {MessageOfObj::BulletMessage, "BulletMessage"},
        {MessageOfObj::FactoryMessage, "FactoryMessage"},
        {MessageOfObj::CommunityMessage, "CommunityMessage"},
        {MessageOfObj::FortMessage, "FortMessage"},
        {MessageOfObj::WormholeMessage, "WormholeMessage"},
        {MessageOfObj::HomeMessage, "HomeMessage"},
        {MessageOfObj::ResourceMessage, "ResourceMessage"},
        {MessageOfObj::MapMessage, "MapMessage"},
        {MessageOfObj::NewsMessage, "NewsMessage"},
        {MessageOfObj::BombedBulletMessage, "BombedBulletMessage"},
        {MessageOfObj::TeamMessage, "TeamMessage"},
    };
}  // namespace THUAI7

#endif
