#pragma once
#ifndef CONSTANTS_H
#define CONSTANTS_H

#ifndef SCCI
#define SCCI static const constexpr inline
#endif

#undef GetMessage
#undef SendMessage
#undef PeekMessage

namespace Constants
{
    SCCI int32_t frameDuration = 50;  // 每帧毫秒数
    // 地图相关
    SCCI int32_t numOfGridPerCell = 1000;  // 单位坐标数
    SCCI int32_t rows = 50;                // 地图行数
    SCCI int32_t cols = 50;                // 地图列数
    SCCI int32_t maxGarbageProgress = 200;
    SCCI int32_t maxBridgeHp = 18000;
    SCCI double robPercent = 0.2;  // 击杀获得经济比率
    SCCI int32_t DestroyBuildingBonus = 200;
    SCCI double recoverMultiplier = 1.2;
    SCCI double recycleMultiplier = 0.5;
    struct Home
    {
        SCCI int32_t maxHp = 24000;
        SCCI int32_t energySpeed = 1;
    };
    struct RecycleBank
    {
        SCCI int32_t maxHp = 8000;
        SCCI int32_t energySpeed = 3;
    };
    struct ChargeStation
    {
        SCCI int32_t maxHp = 6000;
    };
    struct SignalTower
    {
        SCCI int32_t maxHp = 12000;
        SCCI int32_t attackRange = 8000;
        SCCI int32_t damage = 1200;
    };

    // 船
    SCCI int32_t sizeofSweeper = 800;
    struct CivilianSweeper
    {
        SCCI int32_t maxHp = 3000;
        SCCI int32_t basicArmor = 0;
        SCCI int32_t basicShield = 0;
        SCCI int32_t Speed = 3000;
        SCCI int32_t Cost = 40;
    };
    struct MilitarySweeper
    {
        SCCI int32_t maxHp = 4000;
        SCCI int32_t basicArmor = 400;
        SCCI int32_t basicShield = 400;
        SCCI int32_t Speed = 2800;
        SCCI int32_t Cost = 120;
    };
    struct FlagSweeper
    {
        SCCI int32_t maxHp = 12000;
        SCCI int32_t basicArmor = 800;
        SCCI int32_t basicShield = 800;
        SCCI int32_t Speed = 2700;
        SCCI int32_t Cost = 500;
    };

    // 模块
    struct Producer
    {
        SCCI int32_t energySpeed1 = 5;
        SCCI int32_t energySpeed2 = 7;
        SCCI int32_t energySpeed3 = 10;
        SCCI int32_t Cost1 = 0;
        SCCI int32_t Cost2 = 40;
        SCCI int32_t Cost3 = 80;
    };
    struct Constructor
    {
        SCCI int32_t constructSpeed1 = 500;
        SCCI int32_t constructSpeed2 = 750;
        SCCI int32_t constructSpeed3 = 1000;
        SCCI int32_t Cost1 = 0;
        SCCI int32_t Cost2 = 40;
        SCCI int32_t Cost3 = 80;
    };
    struct Armor
    {
        SCCI int32_t armor1 = 2000;
        SCCI int32_t armor2 = 3000;
        SCCI int32_t armor3 = 4000;
        SCCI int32_t Cost1 = 60;
        SCCI int32_t Cost2 = 120;
        SCCI int32_t Cost3 = 180;
    };
    struct Shield
    {
        SCCI int32_t shield1 = 2000;
        SCCI int32_t shield2 = 3000;
        SCCI int32_t shield3 = 4000;
        SCCI int32_t Cost1 = 60;
        SCCI int32_t Cost2 = 120;
        SCCI int32_t Cost3 = 180;
    };
    struct Weapon
    {
        SCCI int32_t LaserCost = 0;
        SCCI int32_t PlasmaCost = 120;
        SCCI int32_t ShellCost = 130;
        SCCI int32_t MissileCost = 180;
        SCCI int32_t ArcCost = 240;
    };

    // 子弹
    struct Laser
    {
        SCCI int32_t Damage = 1200;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 1.5;
        SCCI double ShieldDamageMultiplier = 0.6;
        SCCI int32_t Speed = 20000;
        SCCI int32_t CastTime = 300;  // ms
        SCCI int32_t BackSwing = 300;
    };
    struct Plasma
    {
        SCCI int32_t Damage = 1300;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 2.0;
        SCCI double ShieldDamageMultiplier = 0.4;
        SCCI int32_t Speed = 10000;
        SCCI int32_t CastTime = 400;  // ms
        SCCI int32_t BackSwing = 400;
    };
    struct Shell
    {
        SCCI int32_t Damage = 1800;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 0.4;
        SCCI double ShieldDamageMultiplier = 1.5;
        SCCI int32_t Speed = 8000;
        SCCI int32_t CastTime = 200;  // ms
        SCCI int32_t BackSwing = 200;
    };
    struct Missile
    {
        SCCI int32_t Damage = 1600;
        SCCI int32_t AttackRange = 8000;
        SCCI int32_t ExplodeRange = 1600;
        SCCI double ArmorDamageMultiplier = 1.0;
        SCCI double ShieldDamageMultiplier = 0.4;
        SCCI int32_t Speed = 6000;
        SCCI int32_t CastTime = 600;  // ms
        SCCI int32_t BackSwing = 600;
    };
    struct Arc
    {
        SCCI int32_t Damage = 3200;  // 100-3200
        SCCI int32_t AttackRange = 8000;
        SCCI double ArmorDamageMultiplier = 2.0;
        SCCI double ShieldDamageMultiplier = 2.0;
        SCCI int32_t Speed = 8000;
        SCCI int32_t CastTime = 600;  // ms
        SCCI int32_t BackSwing = 600;
    };

    // SCCI int32_t basicStudentAlertnessRadius = 15 * numOfGridPerCell;
    // SCCI int32_t basicTrickerAlertnessRadius = 17 * numOfGridPerCell;
    // SCCI int32_t basicStudentViewRange = 10 * numOfGridPerCell;
    // SCCI int32_t basicTrickerViewRange = 13 * numOfGridPerCell;
}  // namespace Constants
#endif
