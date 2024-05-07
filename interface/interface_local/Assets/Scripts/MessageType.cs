using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState
{
    NULL_GAME_STATE,
    GAME_START,
    GAME_RUNNING,
    GAME_END,
}

public enum PlaceType // 地图中的所有建筑/地点类型
{
    SPACE,
    RUIN,
    SHADOW,
    ASTEROID,
    RESOURCE,
    CONSTRUCTION,
    WORMHOLE,
    HOME,
    NULL_PLACE_TYPE,
}

public enum ShapeType
{
    NULL_SHAPE_TYPE,
    CIRCLE,
    SQUARE,
}


public enum PlayerType
{
    NULL_PLAYER_TYPE,
    SHIP,
    TEAM,
}
[Serializable]
public enum ShipType
{
    NULL_SHIP_TYPE,
    CIVILIAN_SHIP,
    MILITARY_SHIP,
    FLAG_SHIP,
}

public enum ShipState
{
    NULL_STATUS,
    IDLE,
    PRODUCING,
    CONSTRUCTING,
    RECOVERING,
    RECYCLING,
    ATTACKING,
    SWINGING,
    STUNNED,
    MOVING,
}

public enum WeaponType
{
    NULL_WEAPON_TYPE,
    LASERGUN,
    PLASMAGUN,
    SHELLGUN,
    MISSILEGUN,
    ARCGUN,
}

public enum ConstructorType
{
    NULL_CONSTRUCTOR_TYPE,
    CONSTRUCTOR1,
    CONSTRUCTOR2,
    CONSTRUCTOR3,
}

public enum ArmorType
{
    NULL_ARMOR_TYPE,
    ARMOR1,
    ARMOR2,
    ARMOR3,
}

public enum ShieldType
{
    NULL_SHIELD_TYPE,
    SHIELD1,
    SHIELD2,
    SHIELD3,
}

public enum ProducerType
{
    NULL_PRODUCER_TYPE,
    PRODUCER1,
    PRODUCER2,
    PRODUCER3,
}

public enum ModuleType
{
    NULL_MODULE_TYPE,
    MODULE_PRODUCER1,
    MODULE_PRODUCER2,
    MODULE_PRODUCER3,
    MODULE_CONSTRUCTOR1,
    MODULE_CONSTRUCTOR2,
    MODULE_CONSTRUCTOR3,
    MODULE_ARMOR1,
    MODULE_ARMOR2,
    MODULE_ARMOR3,
    MODULE_SHIELD1,
    MODULE_SHIELD2,
    MODULE_SHIELD3,
    MODULE_LASERGUN,
    MODULE_PLASMAGUN,
    MODULE_SHELLGUN,
    MODULE_MISSILEGUN,
    MODULE_ARCGUN,
}

public enum BulletType
{
    NULL_BULLET_TYPE,
    LASER,
    PLASMA,
    SHELL,
    MISSILE,
    ARC,
}

public enum ConstructionType
{
    NULL_CONSTRUCTION_TYPE,
    FACTORY,
    COMMUNITY,
    FORT,
}

public enum NewsType
{
    NULL_NEWS_TYPE,
    TEXT,
    BINARY,
}

public enum PlayerTeam
{
    RED,
    BLUE,
    NULL_TEAM,
}