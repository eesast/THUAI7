using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct MessageOfShip
{
    int x;
    int y;
    int speed;
    int hp;
    int armor;
    int shield;
    int team_id;
    int player_id;
    int guid;
    ShipState ship_state;
    ShipType ship_type;
    int view_range;
    ProducerType producer_type;
    ConstructorType constructor_type;
    ArmorType armor_type;
    ShieldType shield_type;
    WeaponType weapon_type;
    double facing_direction;
}


public struct MessageOfBullet
{
    BulletType type;
    int x;
    int y;
    double facing_direction;
    int damage;
    int team_id;
    int guid;
    double bomb_range;
    int speed;
}

public struct MessageOfBombedBullet  //for Unity，直接继承自THUAI5
{
    BulletType type;
    int x;
    int y;
    double facing_direction;
    int mapping_id;
    double bomb_range;
}

public struct MessageOfFactory
{
    int x;
    int y;
    int hp; // 剩余的血量
    int team_id;
}

public struct MessageOfCommunity
{
    int x;
    int y;
    int hp; // 剩余的血量
    int team_id;
}

public struct MessageOfFort
{
    int x;
    int y;
    int hp; // 剩余的血量
    int team_id;
}

public struct MessageOfWormhole
{
    int x;
    int y;
    int hp; // 剩余的血量
}

public struct MessageOfResource
{
    int x;
    int y;
    int progress; // 采集进度
}

public struct MessageOfHome
{
    int x;
    int y;
    int hp;
    int team_id;
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