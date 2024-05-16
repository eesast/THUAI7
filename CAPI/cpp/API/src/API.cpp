#include "AI.h"
#include "API.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

#define PI 3.14159265358979323846

std::future<bool> ShipAPI::SendTextMessage(int32_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), false); });
}

std::future<bool> TeamAPI::SendTextMessage(int32_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), false); });
}

std::future<bool> ShipAPI::SendBinaryMessage(int32_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), true); });
}

std::future<bool> TeamAPI::SendBinaryMessage(int32_t toID, std::string message)
{
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { return logic.Send(toID, std::move(message), true); });
}

bool ShipAPI::HaveMessage()
{
    return logic.HaveMessage();
}

bool TeamAPI::HaveMessage()
{
    return logic.HaveMessage();
}

std::pair<int32_t, std::string> ShipAPI::GetMessage()
{
    return logic.GetMessage();
}

std::pair<int32_t, std::string> TeamAPI::GetMessage()
{
    return logic.GetMessage();
}

int32_t ShipAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

int32_t TeamAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

bool ShipAPI::Wait()
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

std::future<bool> ShipAPI::EndAllAction()
{
    return std::async(std::launch::async, [this]()
                      { return logic.EndAllAction(); });
}

std::future<bool> TeamAPI::EndAllAction()
{
    return std::async(std::launch::async, [this]()
                      { return logic.EndAllAction(); });
}

std::vector<std::shared_ptr<const THUAI7::Ship>> ShipAPI::GetShips() const
{
    return logic.GetShips();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> TeamAPI::GetShips() const
{
    return logic.GetShips();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> ShipAPI::GetEnemyShips() const
{
    return logic.GetEnemyShips();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> TeamAPI::GetEnemyShips() const
{
    return logic.GetEnemyShips();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> ShipAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> TeamAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::vector<THUAI7::PlaceType>> ShipAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

std::vector<std::vector<THUAI7::PlaceType>> TeamAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

THUAI7::PlaceType ShipAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

THUAI7::PlaceType TeamAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

std::optional<THUAI7::ConstructionState> ShipAPI::GetConstructionState(int32_t cellX, int32_t cellY) const
{
    return logic.GetConstructionState(cellX, cellY);
}

std::optional<THUAI7::ConstructionState> TeamAPI::GetConstructionState(int32_t cellX, int32_t cellY) const
{
    return logic.GetConstructionState(cellX, cellY);
}

int32_t ShipAPI::GetWormholeHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetWormholeHp(cellX, cellY);
}

int32_t TeamAPI::GetWormholeHp(int32_t cellX, int32_t cellY) const
{
    return logic.GetWormholeHp(cellX, cellY);
}

int32_t ShipAPI::GetResourceState(int32_t cellX, int32_t cellY) const
{
    return logic.GetResourceState(cellX, cellY);
}

int32_t TeamAPI::GetResourceState(int32_t cellX, int32_t cellY) const
{
    return logic.GetResourceState(cellX, cellY);
}

int32_t ShipAPI::GetHomeHp() const
{
    return logic.GetHomeHp();
}

int32_t TeamAPI::GetHomeHp() const
{
    return logic.GetHomeHp();
}

std::shared_ptr<const THUAI7::GameInfo> ShipAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::shared_ptr<const THUAI7::GameInfo> TeamAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::vector<int64_t> ShipAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::vector<int64_t> TeamAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::shared_ptr<const THUAI7::Ship> ShipAPI::GetSelfInfo() const
{
    return logic.ShipGetSelfInfo();
}

std::shared_ptr<const THUAI7::Team> TeamAPI::GetSelfInfo() const
{
    return logic.TeamGetSelfInfo();
}

int32_t ShipAPI::GetScore() const
{
    return logic.GetScore();
}

int32_t TeamAPI::GetScore() const
{
    return logic.GetScore();
}

int32_t ShipAPI::GetEnergy() const
{
    return logic.GetEnergy();
}

int32_t TeamAPI::GetEnergy() const
{
    return logic.GetEnergy();
}

// Ship独有
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

std::future<bool> ShipAPI::Attack(double angleInRadian)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Attack(angleInRadian); });
}

std::future<bool> ShipAPI::Recover(int64_t recover)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Recover(recover); });
}

std::future<bool> ShipAPI::Produce()
{
    return std::async(std::launch::async, [=]()
                      { return logic.Produce(); });
}

std::future<bool> ShipAPI::RepairWormhole()
{
    return std::async(std::launch::async, [=]()
                      { return logic.RepairWormhole(); });
}

std::future<bool> ShipAPI::RepairHome()
{
    return std::async(std::launch::async, [=]()
                      { return logic.RepairHome(); });
}

std::future<bool> ShipAPI::Rebuild(THUAI7::ConstructionType constructionType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Rebuild(constructionType); });
}

std::future<bool> ShipAPI::Construct(THUAI7::ConstructionType constructionType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Construct(constructionType); });
}

bool ShipAPI::HaveView(int32_t targetX, int32_t targetY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(selfInfo->x, selfInfo->y, targetX, targetY, selfInfo->viewRange);
}

// Team独有
std::future<bool> TeamAPI::InstallModule(int32_t playerID, const THUAI7::ModuleType moduleType)
{
    return std::async(std::launch::async, [=]()
                      { return logic.InstallModule(playerID, moduleType); });
}

std::future<bool> TeamAPI::Recycle(int32_t playerID)
{
    return std::async(std::launch::async, [=]()
                      { return logic.Recycle(playerID); });
}

std::future<bool> TeamAPI::BuildShip(THUAI7::ShipType ShipType, int32_t birthIndex)
{
    return std::async(std::launch::async, [=]()
                      { return logic.BuildShip(ShipType, birthIndex); });
}

void ShipAPI::Play(IAI& ai)
{
    ai.play(*this);
}

void TeamAPI::Play(IAI& ai)
{
    ai.play(*this);
}
