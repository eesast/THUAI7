using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct MessageOfShip
{
    public float x;
    public float y;
    public float speed;
    public int hp;
    public int armor;
    public int shield;
    public PlayerTeam playerTeam;
    // public int playerId;
    public ShipState shipState;
    public ShipType shipType;
    public int viewRange;
    public ProducerType producerType;
    public ConstructorType constructorType;
    public ArmorType armorType;
    public ShieldType shieldType;
    public WeaponType weaponType;
    // public double facingDirection;
}


[System.Serializable]
public struct MessageOfBullet
{
    public BulletType type;
    // public int x;
    // public int y;
    // public double facing_direction;
    // public int damage;
    public PlayerTeam playerTeam;
    // public int guid;
    // public double bombRange;
    // public int speed;
}
[Serializable]
public struct MessageOfConstruction
{
    public int x;
    public int y;
    public int hp; // 剩余的血量
    public ConstructionType constructionType;
    public PlayerTeam playerTeam;
    public bool constructed;
}

public struct MessageOfWormhole
{
    int x;
    int y;
    int hp; // 剩余的血量
}

public struct MessageOfResource
{
    public int x;
    public int y;
    public int progress; // 采集进度
}
[System.Serializable]
public struct MessageOfBase
{
    public int x;
    public int y;
    public int hp;
    public int economy;
    public int scoreminus;
    public PlayerTeam playerTeam;
    public ShipDic shipNum;
    public int shipTotalNum;
}

public struct MessageOfMap
{
    uint height;
    uint width;
    public struct Row
    {
        PlaceType[] cols;
    }
    Row[] rows;
}

public struct MessageOfTeam
{
    int team_id;
    int player_id;
    int score;
    int money;

}

// public struct MessageOfObj
// {
//     oneof struct_of_obj
//     {
//         MessageOfShip ship_struct;
//         MessageOfBullet bullet_struct;
//         MessageOfFactory factory_struct;
//         MessageOfCommunity community_struct;
//         MessageOfFort fort_struct;
//         MessageOfWormhole wormhole_struct;
//         MessageOfHome home_struct;
//         MessageOfResource resource_struct;
//         MessageOfMap map_struct;
//         MessageOfNews news_struct;
//         MessageOfBombedBullet bombed_bullet_struct;
//         MessageOfTeam team_struct;
//     }
// }

public struct MessageOfAll
{
    int game_time;
    int red_team_score;
    int blue_team_score;
}

public struct MessageToClient
{
    // MessageOfObj obj_struct;
    GameState game_state;
    MessageOfAll all_struct;
}

public struct MoveRes // 如果打算设计撞墙保留平行速度分量，且需要返回值则可用这个（大概没啥用）
{
    int actual_speed;
    double actual_angle;
    bool act_success;
}

public struct BoolRes
{
    bool act_success;
}

public struct ShipInfoRes
{
    MessageOfShip[] ship_info;
}

public struct EcoRes
{
    int economy;
}

// public struct MessageOfNews
// {
//     oneof news // 一条新闻
//     {
//         string text_struct;
//     bytes binary_struct;
// }
// int from_id;
// int to_id;
// }