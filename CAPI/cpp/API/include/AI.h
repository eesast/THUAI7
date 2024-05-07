#pragma once
#ifndef AI_H
#define AI_H

#include "API.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

class IAI
{
public:
    virtual ~IAI() = default;
    IAI() = default;
    virtual void play(IShipAPI& api) = 0;
    virtual void play(ITeamAPI& api) = 0;
};

using CreateAIFunc = std::unique_ptr<IAI> (*)(int32_t playerID);

class AI : public IAI
{
public:
    AI(int32_t pID) :
        IAI(),
        playerID(pID)
    {
    }
    void play(IShipAPI& api) override;
    void play(ITeamAPI& api) override;

private:
    int32_t playerID;
};

#endif
