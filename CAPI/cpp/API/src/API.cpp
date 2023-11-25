#include <optional>
#include "AI.h"
#include "API.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

#define PI 3.14159265358979323846

int32_t ShipAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

int32_t HomeAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

std::future<bool> ShipAPI::Move(int64_t timeInMilliseconds, double angleInRadian)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Move(timeInMilliseconds, angleInRadian); });
}

std::future<bool> ShipAPI::MoveDown(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, 0);
}

std::future<bool> ShipAPI::MoveRight(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 0.5);
}

std::future<bool> ShipAPI::MoveUp(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI);
}

std::future<bool> ShipAPI::MoveLeft(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 1.5);
}



std::future<bool> StudentAPI::InstallModule(const THUAI7::ModuleType type,const THUAI7::ModuleLevel level)
{
    return std::async(std::launch::async, [=]()
                      { return logic.InstallModule(const THUAI7::ModuleType type,const THUAI7::ModuleLevel level); });
}

int32_t StudentAPI::GetBuildingHp(int32_t cellX, int32_t cellY)
{
    return logic.GetBuildingHp(int32_t cellX, int32_t cellY);
}

int32_t HomeAPI::GetEconomy() const
{
    return logic.GetEconomy();
}

std::future<bool> ShipAPI::EndAllAction()
{
    return std::async(std::launch::async, [this]()
                      { return logic.EndAllAction(); });
}



std::future<bool> ShipAPI::SendTextMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.SendMessage(toID, std::move(message), false); });
}

std::future<bool> HomeAPI::SendTextMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.SendMessage(toID, std::move(message), false); });
}

std::future<bool> ShipAPI::SendBinaryMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.SendMessage(toID, std::move(message), false); });
}

std::future<bool> HomeAPI::SendBinaryMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.SendMessage(toID, std::move(message), false); });
}

bool ShipAPI::HaveMessage()
{
    return logic.HaveMessage();
}

bool HomeAPI::HaveMessage()
{
    return logic.HaveMessage();
}

std::pair<int64_t, std::string> ShipAPI::GetMessage()
{
    return logic.GetMessage();
}

std::pair<int64_t, std::string> HomeAPI::GetMessage()
{
    return logic.GetMessage();
}

bool ShipAPI::Wait()
{
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

bool HomeAPI::Wait()
{
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> ShipAPI::GetEnemyShip() const
{
    return logic.GetEnemyShip();
}



std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips()
{
    return logic.GetShips();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> ShipAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::shared_ptr<const THUAI7::Ship> ShipGetSelfInfo()
{
    return logic.ShipGetSelfInfo();
}



std::vector<std::vector<THUAI7::PlaceType>> ShipAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

THUAI7::PlaceType ShipAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

THUIAI7::PlaceType HomeAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}


std::vector<std::vector<THUAI7::PlaceType>> HomeAPI::GetFullMap() const
{
    return logic.GetFullMap();
}




std::shared_ptr<const THUAI7::GameInfo> ShipAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::shared_ptr<const THUAI7::GameInfo> HomeAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}


std::future<bool> ShipAPI::Attack(double angleInRadian)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Attack(angleInRadian); });
}


bool StudentAPI::HaveView(int32_t gridX, int32_t gridY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(gridX, gridY, selfInfo->x, selfInfo->y);
}

std::vector<int64_t> GetShipGUIDs()
{
    return logic.GetShipGUIDs();
}

bool HomeAPI::HaveView(int32_t gridX, int32_t gridY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(gridX, gridY, selfInfo->x, selfInfo->y);
}

void ShipAPI::Play(IAI& ai)
{
    ai.play(*this);
}

