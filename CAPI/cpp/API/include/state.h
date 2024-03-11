#pragma once
#ifndef STATE_H
#define STATE_H

#include <vector>
#include <array>
#include <map>
#include <memory>

#include "structures.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

// 存储场上的状态
struct State
{
    // 自身信息，根据playerType的不同，可以调用的值也不同。
    std::shared_ptr<THUAI7::Sweeper> SweeperSelf;
    std::shared_ptr<THUAI7::Team> teamSelf;
    std::vector<std::shared_ptr<THUAI7::Sweeper>> Sweepers;
    std::vector<std::shared_ptr<THUAI7::Sweeper>> enemySweepers;
    std::shared_ptr<THUAI7::Team> enemyTeam;

    std::vector<std::shared_ptr<THUAI7::Bullet>> bullets;

    std::vector<std::vector<THUAI7::PlaceType>> gameMap;
    std::shared_ptr<THUAI7::GameMap> mapInfo;
    std::shared_ptr<THUAI7::GameInfo> gameInfo;
    std::vector<int64_t> guids;
};

#endif
