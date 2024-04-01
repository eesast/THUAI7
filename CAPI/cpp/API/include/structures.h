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
        Ground = 2,
        Wall = 3,
        Grass = 4,
        River = 5,
        Garbage = 6,
        Construction = 7,
        Bridge = 8,
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
        Sweeper = 1,
        Team = 2,
    };

    enum class SweeperType : unsigned char
    {
        NullSweeperType = 0,
        CivilianSweeper = 1,
        MilitarySweeper = 2,
        FlagSweeper = 3,
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

    enum class SweeperState : unsigned char
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
        RecycleBank = 1,
        ChargeStation = 2,
        SignalTower = 3,
    };

    enum class MessageOfObj : unsigned char
    {
        NullMessageOfObj = 0,
        SweeperMessage = 1,
        BulletMessage = 2,
        RecycleBankMessage = 3,
        ChargeStationMessage = 4,
        SignalTowerMessage = 5,
        BridgeMessage = 6,
        HomeMessage = 7,
        GarbageMessage = 8,
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

    struct Sweeper
    {
        int32_t x;         // x坐标
        int32_t y;         // y坐标
        int32_t speed;     // 移动速度
        int32_t hp;        // 血量
        int32_t armor;     // 装甲
        int32_t shield;    // 护盾
        int64_t playerID;  // 船的id
        int64_t teamID;
        int64_t guid;               // 全局唯一ID
        SweeperState sweeperState;  // 船所处状态
        SweeperType sweeperType;
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
        int32_t score;
        int32_t energy;
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
        int32_t bombRange;
        double explodeRange;  // 炸弹爆炸范围
        int32_t speed;        // 子弹速度
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

    struct GameMap
    {
        // x,y,id,hp
        std::map<std::pair<int32_t, int32_t>, std::pair<int64_t, int32_t>> recycleBankState;
        std::map<std::pair<int32_t, int32_t>, std::pair<int64_t, int32_t>> chargeStationState;
        std::map<std::pair<int32_t, int32_t>, std::pair<int64_t, int32_t>> signalTowerState;
        std::map<std::pair<int32_t, int32_t>, std::pair<int64_t, int32_t>> homeState;
        std::map<std::pair<int32_t, int32_t>, int32_t> bridgeState;
        std::map<std::pair<int32_t, int32_t>, int32_t> garbageState;
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

    inline std::map<SweeperType, std::string> sweeperTypeDict{
        {SweeperType::NullSweeperType, "NullSweeperType"},
        {SweeperType::CivilianSweeper, "CivilianSweeper"},
        {SweeperType::MilitarySweeper, "MilitarySweeper"},
        {SweeperType::FlagSweeper, "FlagSweeper"},
    };

    inline std::map<SweeperState, std::string> sweeperStateDict{
        {SweeperState::NullStatus, "NullState"},
        {SweeperState::Idle, "Idle"},
        {SweeperState::Producing, "Producing"},
        {SweeperState::Constructing, "Constructing"},
        {SweeperState::Recovering, "Recoverying"},
        {SweeperState::Recycling, "Recycling"},
        {SweeperState::Attacking, "Attacking"},
        {SweeperState::Swinging, "Swinging"},
        {SweeperState::Stunned, "Stunned"},
    };

    inline std::map<PlayerTeam, std::string> playerTeamDict{
        {PlayerTeam::NullTeam, "NullTeam"},
        {PlayerTeam::Red, "Red"},
        {PlayerTeam::Blue, "Blue"},
    };

    inline std::map<PlaceType, std::string> placeTypeDict{
        {PlaceType::NullPlaceType, "NullPlaceType"},
        {PlaceType::Home, "Home"},
        {PlaceType::Ground, "Ground"},
        {PlaceType::Wall, "Wall"},
        {PlaceType::Grass, "Grass"},
        {PlaceType::River, "River"},
        {PlaceType::Garbage, "Garbage"},
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

    inline std::map<MessageOfObj, std::string> messageOfObjDict{
        {MessageOfObj::NullMessageOfObj, "NullMessageOfObj"},
        {MessageOfObj::SweeperMessage, "SweeperMessage"},
        {MessageOfObj::BulletMessage, "BulletMessage"},
        {MessageOfObj::RecycleBankMessage, "RecycleBankMessage"},
        {MessageOfObj::ChargeStationMessage, "ChargeStationMessage"},
        {MessageOfObj::SignalTowerMessage, "SignalTowerMessage"},
        {MessageOfObj::BridgeMessage, "BridgeMessage"},
        {MessageOfObj::HomeMessage, "HomeMessage"},
        {MessageOfObj::GarbageMessage, "GarbageMessage"},
        {MessageOfObj::MapMessage, "MapMessage"},
        {MessageOfObj::NewsMessage, "NewsMessage"},
        {MessageOfObj::BombedBulletMessage, "BombedBulletMessage"},
        {MessageOfObj::TeamMessage, "TeamMessage"},
    };
}  // namespace THUAI7

#endif
