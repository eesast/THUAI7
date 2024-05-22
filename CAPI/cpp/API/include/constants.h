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
    SCCI int32_t maxResourceProgress = 32000;
    SCCI int32_t maxWormholeHp = 18000;
    SCCI double robPercent = 0.2;  // 击杀获得经济比率
    SCCI int32_t DestroyBuildingBonus = 200;
    SCCI double recoverMultiplier = 1.2;
    SCCI double recycleMultiplier = 0.5;
    struct Home
    {
        SCCI int32_t maxHp = 48000;
        SCCI int32_t energySpeed = 100;
        SCCI int32_t attackRange = 8000;
        SCCI int32_t damage = 300;
    };
    struct Factory
    {
        SCCI int32_t maxHp = 12000;
        SCCI int32_t energySpeed = 200;
    };
    struct Community
    {
        SCCI int32_t maxHp = 10000;
    };
    struct Fort
    {
        SCCI int32_t maxHp = 16000;
        SCCI int32_t attackRange = 8000;
        SCCI int32_t damage = 300;
    };

    // 船
    SCCI int32_t sizeofShip = 800;
    struct CivilianShip
    {
        SCCI int32_t maxHp = 3000;
        SCCI int32_t basicArmor = 0;
        SCCI int32_t basicShield = 0;
        SCCI int32_t Speed = 3000;
        SCCI int32_t Cost = 4000;
    };
    struct MilitaryShip
    {
        SCCI int32_t maxHp = 4000;
        SCCI int32_t basicArmor = 400;
        SCCI int32_t basicShield = 400;
        SCCI int32_t Speed = 2800;
        SCCI int32_t Cost = 12000;
    };
    struct FlagShip
    {
        SCCI int32_t maxHp = 12000;
        SCCI int32_t basicArmor = 800;
        SCCI int32_t basicShield = 800;
        SCCI int32_t Speed = 2700;
        SCCI int32_t Cost = 50000;
    };

    // 模块
    struct Producer
    {
        SCCI int32_t energySpeed1 = 100;
        SCCI int32_t energySpeed2 = 200;
        SCCI int32_t energySpeed3 = 300;
        SCCI int32_t Cost1 = 0;
        SCCI int32_t Cost2 = 4000;
        SCCI int32_t Cost3 = 8000;
    };
    struct Constructor
    {
        SCCI int32_t constructSpeed1 = 300;
        SCCI int32_t constructSpeed2 = 400;
        SCCI int32_t constructSpeed3 = 500;
        SCCI int32_t Cost1 = 0;
        SCCI int32_t Cost2 = 4000;
        SCCI int32_t Cost3 = 8000;
    };
    struct Armor
    {
        SCCI int32_t armor1 = 2000;
        SCCI int32_t armor2 = 3000;
        SCCI int32_t armor3 = 4000;
        SCCI int32_t Cost1 = 6000;
        SCCI int32_t Cost2 = 12000;
        SCCI int32_t Cost3 = 18000;
    };
    struct Shield
    {
        SCCI int32_t shield1 = 2000;
        SCCI int32_t shield2 = 3000;
        SCCI int32_t shield3 = 4000;
        SCCI int32_t Cost1 = 6000;
        SCCI int32_t Cost2 = 12000;
        SCCI int32_t Cost3 = 18000;
    };
    struct Weapon
    {
        SCCI int32_t LaserCost = 0;
        SCCI int32_t PlasmaCost = 12000;
        SCCI int32_t ShellCost = 13000;
        SCCI int32_t MissileCost = 18000;
        SCCI int32_t ArcCost = 24000;
    };

    // 子弹
    struct Laser
    {
        SCCI int32_t Damage = 800;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 1.5;
        SCCI double ShieldDamageMultiplier = 0.6;
        SCCI int32_t Speed = 20000;
        SCCI int32_t CastTime = 500;  // ms
        SCCI int32_t BackSwing = 1000;
    };
    struct Plasma
    {
        SCCI int32_t Damage = 1000;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 2.0;
        SCCI double ShieldDamageMultiplier = 0.4;
        SCCI int32_t Speed = 10000;
        SCCI int32_t CastTime = 800;  // ms
        SCCI int32_t BackSwing = 1600;
    };
    struct Shell
    {
        SCCI int32_t Damage = 1200;
        SCCI int32_t AttackRange = 4000;
        SCCI double ArmorDamageMultiplier = 0.4;
        SCCI double ShieldDamageMultiplier = 1.5;
        SCCI int32_t Speed = 8000;
        SCCI int32_t CastTime = 500;  // ms
        SCCI int32_t BackSwing = 1000;
    };
    struct Missile
    {
        SCCI int32_t Damage = 1600;
        SCCI int32_t AttackRange = 6000;
        SCCI int32_t ExplodeRange = 1100;
        SCCI double ArmorDamageMultiplier = 1.0;
        SCCI double ShieldDamageMultiplier = 0.4;
        SCCI int32_t Speed = 6000;
        SCCI int32_t CastTime = 1200;  // ms
        SCCI int32_t BackSwing = 1800;
    };
    struct Arc
    {
        SCCI int32_t MinDamage = 800;
        SCCI int32_t MaxDamage = 1600;
        SCCI int32_t AttackRange = 6000;
        SCCI double ArmorDamageMultiplier = 2.0;
        SCCI double ShieldDamageMultiplier = 2.0;
        SCCI int32_t Speed = 8000;
        SCCI int32_t CastTime = 1200;  // ms
        SCCI int32_t BackSwing = 1800;
    };
}  // namespace Constants
#endif
