#include <vector>
#include <thread>
#include <array>
#include "AI.h"
#include "constants.h"
// 注意不要使用conio.h，Windows.h等非标准库

// 为假则play()期间确保游戏状态不更新，为真则只保证游戏状态在调用相关方法时不更新，大致一帧更新一次
extern const bool asynchronous = false;

// 选手需要依次将player0到player4的职业在这里定义

extern const std::array<THUAI7::ShipType, 8> shipType = {
    THUAI7::ShipType::CivilianShip,
    THUAI7::ShipType::CivilianShip,
    THUAI7::ShipType::CivilianShip,
    THUAI7::ShipType::MilitaryShip,
    THUAI7::ShipType::MilitaryShip,
    THUAI7::ShipType::MilitaryShip,
    THUAI7::ShipType::MilitaryShip,
    THUAI7::ShipType::FlagShip,
    };

// 可以在AI.cpp内部声明变量与函数

void AI::play(IShipAPI& api)  
{
    //  0,1,2为民用舰船
    if (this->playerID == 0)
    {
        // 玩家0执行操作
    }
    else if (this->playerID == 1)
    {
        // 玩家1执行操作
    }
    else if (this->playerID == 2)
    {
        // 玩家2执行操作
    }

    // 3，4，5，6为军用舰船
    else if (this->playerID == 3)
    {
        // 玩家3执行操作
    }
    else if (this->playerID == 4)
    {
        // 玩家4执行操作
    }
    else if (this->playerID == 5)
    {
        // 玩家5执行操作
    }
    else if (this->playerID == 6)
    {
        // 玩家6执行操作
    }

    // 7为旗舰
    else if (this->playerID == 7)
    {
        // 玩家7执行操作
    }   
}

void AI::play(IHomeAPI& api)
{
    //执行大本营操作
    auto self = api.GetSelfInfo();
    api.PrintSelfInfo();
}
