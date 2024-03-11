#include <optional>
#include "AI.h"
#include "API.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

#define PI 3.14159265358979323846

std::future<bool> SweeperAPI::SendTextMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), false); });
}

std::future<bool> TeamAPI::SendTextMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), false); });
}

std::future<bool> SweeperAPI::SendBinaryMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), true); });
}

std::future<bool> TeamAPI::SendBinaryMessage(int64_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), true); });
}

bool SweeperAPI::HaveMessage()
{
    return logic.HaveMessage();
}

bool TeamAPI::HaveMessage()
{
    return logic.HaveMessage();
}

std::pair<int64_t, std::string> SweeperAPI::GetMessage()
{
    return logic.GetMessage();
}

std::pair<int64_t, std::string> TeamAPI::GetMessage()
{
    return logic.GetMessage();
}

int32_t SweeperAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

int32_t TeamAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

bool SweeperAPI::Wait()
{
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

bool TeamAPI::Wait()
{
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

std::future<bool> SweeperAPI::EndAllAction()
{
    return std::async(std::launch::async, [this]()
                      { return logic.EndAllAction(); });
}

std::future<bool> TeamAPI::EndAllAction()
{
    return std::async(std::launch::async, [this]()
                      { return logic.EndAllAction(); });
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> SweeperAPI::GetSweepers() const
{
    return logic.GetSweepers();
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> TeamAPI::GetSweepers() const
{
    return logic.GetSweepers();
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> SweeperAPI::GetEnemySweepers() const
{
    return logic.GetEnemySweepers();
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> TeamAPI::GetEnemySweepers() const
{
    return logic.GetEnemySweepers();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> SweeperAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> TeamAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::vector<THUAI7::PlaceType>> SweeperAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

std::vector<std::vector<THUAI7::PlaceType>> TeamAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

THUAI7::PlaceType SweeperAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

THUAI7::PlaceType TeamAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

int32_t SweeperAPI::GetConstructionHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetConstructionHp(cellX, cellY);
}

int32_t TeamAPI::GetConstructionHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetConstructionHp(cellX, cellY);
}

int32_t SweeperAPI::GetBridgeHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetBridgeHp(cellX, cellY);
}

int32_t TeamAPI::GetBridgeHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetBridgeHp(cellX, cellY);
}

int32_t SweeperAPI::GetGarbageState(int32_t cellX, int32_t cellY) const
{
    return logic.GetGarbageState(cellX, cellY);
}

int32_t TeamAPI::GetGarbageState(int32_t cellX, int32_t cellY) const
{
    return logic.GetGarbageState(cellX, cellY);
}

int32_t SweeperAPI::GetHomeHp() const
{
    return logic.GetHomeHp();
}

int32_t TeamAPI::GetHomeHp() const
{
    return logic.GetHomeHp();
}

std::shared_ptr<const THUAI7::GameInfo> SweeperAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::shared_ptr<const THUAI7::GameInfo> TeamAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::vector<int64_t> SweeperAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::vector<int64_t> TeamAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::shared_ptr<const THUAI7::Sweeper> SweeperAPI::GetSelfInfo() const
{
    return logic.SweeperGetSelfInfo();
}

std::shared_ptr<const THUAI7::Team> TeamAPI::GetSelfInfo() const
{
    return logic.TeamGetSelfInfo();
}

int32_t SweeperAPI::GetScore() const
{
    return logic.GetScore();
}

int32_t TeamAPI::GetScore() const
{
    return logic.GetScore();
}

int32_t SweeperAPI::GetMoney() const
{
    return logic.GetMoney();
}

int32_t TeamAPI::GetMoney() const
{
    return logic.GetMoney();
}

// Sweeper独有
std::future<bool> SweeperAPI::Move(int64_t timeInMilliseconds, double angleInRadian)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Move(timeInMilliseconds, angleInRadian); });
}

std::future<bool> SweeperAPI::MoveDown(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, 0);
}

std::future<bool> SweeperAPI::MoveRight(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 0.5);
}

std::future<bool> SweeperAPI::MoveUp(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI);
}

std::future<bool> SweeperAPI::MoveLeft(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 1.5);
}

std::future<bool> SweeperAPI::Attack(double angleInRadian)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Attack(angleInRadian); });
}

std::future<bool> SweeperAPI::Recover()
{
    return std::async(std::launch::async, [=]()
                      { return logic.Recover(); });
}

std::future<bool> SweeperAPI::Produce()
{
    return std::async(std::launch::async, [=]()
                      { return logic.Produce(); });
}

std::future<bool> SweeperAPI::Rebuild(THUAI7::ConstructionType constructionType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Rebuild(constructionType); });
}

std::future<bool> SweeperAPI::Construct(THUAI7::ConstructionType constructionType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Construct(constructionType); });
}

bool SweeperAPI::HaveView(int32_t gridX, int32_t gridY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(gridX, gridY, selfInfo->x, selfInfo->y, selfInfo->viewRange);
}

// Team独有
std::future<bool> TeamAPI::InstallModule(int64_t playerID, const THUAI7::ModuleType moduleType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.InstallModule(playerID, moduleType); });
}

std::future<bool> TeamAPI::Recycle(int64_t playerID)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Recycle(playerID); });
}

std::future<bool> TeamAPI::BuildSweeper(THUAI7::SweeperType SweeperType, int32_t x, int32_t y)
{
    return std::async(std::launch::async, [=]()
                      { return logic.BuildSweeper(SweeperType, x, y); });
}

void SweeperAPI::Play(IAI& ai)
{
    ai.play(*this);
}

void TeamAPI::Play(IAI& ai)
{
    ai.play(*this);
}
