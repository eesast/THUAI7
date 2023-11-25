#include <optional>
#include <string>
#include "AI.h"
#include "API.h"
#include "utils.hpp"
#include "structures.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

#define PI 3.14159265358979323846

ShipDebugAPI::ShipDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t playerID) :
    logic(logic)
{
    std::string fileName = "logs/api-" + std::to_string(playerID) + "-log.txt";
    auto fileLogger = std::make_shared<spdlog::sinks::basic_file_sink_mt>(fileName, true);
    auto printLogger = std::make_shared<spdlog::sinks::stdout_color_sink_mt>();
    std::string pattern = "[api " + std::to_string(playerID) + "] [%H:%M:%S.%e] [%l] %v";
    fileLogger->set_pattern(pattern);
    printLogger->set_pattern(pattern);
    if (file)
        fileLogger->set_level(spdlog::level::trace);
    else
        fileLogger->set_level(spdlog::level::off);
    if (print)
        printLogger->set_level(spdlog::level::info);
    else
        printLogger->set_level(spdlog::level::off);
    if (warnOnly)
        printLogger->set_level(spdlog::level::warn);
    logger = std::make_unique<spdlog::logger>("apiLogger", spdlog::sinks_init_list{fileLogger, printLogger});
    logger->flush_on(spdlog::level::warn);
}

HomeDebugAPI::HomeDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t playerID) :
    logic(logic)
{
    std::string fileName = "logs/api-" + std::to_string(playerID) + "-log.txt";
    auto fileLogger = std::make_shared<spdlog::sinks::basic_file_sink_mt>(fileName, true);
    auto printLogger = std::make_shared<spdlog::sinks::stdout_color_sink_mt>();
    std::string pattern = "[api" + std::to_string(playerID) + "] [%H:%M:%S.%e] [%l] %v";
    fileLogger->set_pattern(pattern);
    printLogger->set_pattern(pattern);
    if (file)
        fileLogger->set_level(spdlog::level::trace);
    else
        fileLogger->set_level(spdlog::level::off);
    if (print)
        printLogger->set_level(spdlog::level::info);
    else
        printLogger->set_level(spdlog::level::off);
    if (warnOnly)
        printLogger->set_level(spdlog::level::warn);
    logger = std::make_unique<spdlog::logger>("apiLogger", spdlog::sinks_init_list{fileLogger, printLogger});
}

void ShipDebugAPI::StartTimer()
{
    startPoint = std::chrono::system_clock::now();
    std::time_t t = std::chrono::system_clock::to_time_t(startPoint);
    logger->info("=== AI.play() ===");
    logger->info("StartTimer: {}", std::ctime(&t));
}

void HomeDebugAPI::StartTimer()
{
    startPoint = std::chrono::system_clock::now();
    std::time_t t = std::chrono::system_clock::to_time_t(startPoint);
    logger->info("=== AI.play() ===");
    logger->info("StartTimer: {}", std::ctime(&t));
}

void ShipDebugAPI::EndTimer()
{
    logger->info("Time elapsed: {}ms", Time::TimeSinceStart(startPoint));
}

void HomeDebugAPI::EndTimer()
{
    logger->info("Time elapsed: {}ms", Time::TimeSinceStart(startPoint));
}

int32_t ShipDebugAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

int32_t TrickerDebugAPI::GetFrameCount() const
{
    return logic.GetCounter();
}

std::future<bool> ShipDebugAPI::Move(int64_t timeInMilliseconds, double angleInRadian)
{
    logger->info("Move: timeInMilliseconds = {}, angleInRadian = {}, called at {}ms", timeInMilliseconds, angleInRadian, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.Move(timeInMilliseconds, angleInRadian);
                        if (!result)
                            logger->warn("Move: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::MoveDown(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, 0);
}

std::future<bool> ShipDebugAPI::MoveRight(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 0.5);
}

std::future<bool> ShipDebugAPI::MoveUp(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI);
}

std::future<bool> ShipDebugAPI::MoveLeft(int64_t timeInMilliseconds)
{
    return Move(timeInMilliseconds, PI * 1.5);
}

std::future<bool> ShipDebugAPI::SendTextMessage(int64_t toID, std::string message)
{
    logger->info("SendTextMessage: toID = {}, message = {}, called at {}ms", toID, message, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { auto result = logic.SendMessage(toID, std::move(message), false);
                        if (!result)
                            logger->warn("SendTextMessage: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::SendBinaryMessage(int64_t toID, std::string message)
{
    logger->info("SendBinaryMessage: toID = {}, message = {}, called at {}ms", toID, message, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=, message = std::move(message)]()
                      { auto result = logic.SendMessage(toID, std::move(message), true);
                        if (!result)
                            logger->warn("SendBinaryMessage: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

bool ShipDebugAPI::HaveMessage()
{
    logger->info("HaveMessage: called at {}ms", Time::TimeSinceStart(startPoint));
    auto result = logic.HaveMessage();
    if (!result)
        logger->warn("HaveMessage: failed at {}ms", Time::TimeSinceStart(startPoint));
    return result;
}

std::pair<int64_t, std::string> ShipDebugAPI::GetMessage()
{
    logger->info("GetMessage: called at {}ms", Time::TimeSinceStart(startPoint));
    auto result = logic.GetMessage();
    if (result.first == -1)
        logger->warn("GetMessage: failed at {}ms", Time::TimeSinceStart(startPoint));
    return result;
}


bool ShipDebugAPI::Wait()
{
    logger->info("Wait: called at {}ms", Time::TimeSinceStart(startPoint));
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

bool HomeDebugAPI::Wait()
{
    logger->info("Wait: called at {}ms", Time::TimeSinceStart(startPoint));
    if (logic.GetCounter() == -1)
        return false;
    else
        return logic.WaitThread();
}

std::future<bool> ShipDebugAPI::InstallModule(const THUAI7::ModuleType type, const THUAI7::ModuleLevel level) const
{
    logger->info("Installmodule: ModuleType:{} ModuleLevel:{}, called at {}ms", type,level, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.StartRouseMate(mateID);
                        if (!result)
                            logger->warn("StartRouseMate: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::Attack() const
{
    return logic.Attack();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> ShipDebugAPI::GetShips() const
{
    return logic.GetShips();
}

std::vector<std::shared_ptr<const THUAI7::Tricker>> TrickerDebugAPI::GetTrickers() const
{
    return logic.GetTrickers();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> TrickerDebugAPI::GetShips() const
{
    return logic.GetShips();
}

std::vector<std::shared_ptr<const THUAI7::Prop>> ShipDebugAPI::GetProps() const
{
    return logic.GetProps();
}

std::vector<std::shared_ptr<const THUAI7::Prop>> TrickerDebugAPI::GetProps() const
{
    return logic.GetProps();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> ShipDebugAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> TrickerDebugAPI::GetBullets() const
{
    return logic.GetBullets();
}

std::vector<std::vector<THUAI7::PlaceType>> ShipDebugAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

THUAI7::PlaceType ShipDebugAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

THUAI7::PlaceType TrickerDebugAPI::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    return logic.GetPlaceType(cellX, cellY);
}

std::vector<std::vector<THUAI7::PlaceType>> TrickerDebugAPI::GetFullMap() const
{
    return logic.GetFullMap();
}

bool ShipDebugAPI::IsDoorOpen(int32_t cellX, int32_t cellY) const
{
    return logic.IsDoorOpen(cellX, cellY);
}

bool TrickerDebugAPI::IsDoorOpen(int32_t cellX, int32_t cellY) const
{
    return logic.IsDoorOpen(cellX, cellY);
}

int32_t ShipDebugAPI::GetClassroomProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetClassroomProgress(cellX, cellY);
}

int32_t TrickerDebugAPI::GetClassroomProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetClassroomProgress(cellX, cellY);
}

int32_t ShipDebugAPI::GetChestProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetChestProgress(cellX, cellY);
}

int32_t TrickerDebugAPI::GetChestProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetChestProgress(cellX, cellY);
}

int32_t ShipDebugAPI::GetDoorProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetDoorProgress(cellX, cellY);
}

int32_t TrickerDebugAPI::GetDoorProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetDoorProgress(cellX, cellY);
}

THUAI7::HiddenGateState ShipDebugAPI::GetHiddenGateState(int32_t cellX, int32_t cellY) const
{
    return logic.GetHiddenGateState(cellX, cellY);
}

THUAI7::HiddenGateState TrickerDebugAPI::GetHiddenGateState(int32_t cellX, int32_t cellY) const
{
    return logic.GetHiddenGateState(cellX, cellY);
}

int32_t ShipDebugAPI::GetGateProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetGateProgress(cellX, cellY);
}

int32_t TrickerDebugAPI::GetGateProgress(int32_t cellX, int32_t cellY) const
{
    return logic.GetGateProgress(cellX, cellY);
}

std::shared_ptr<const THUAI7::GameInfo> ShipDebugAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::shared_ptr<const THUAI7::GameInfo> TrickerDebugAPI::GetGameInfo() const
{
    return logic.GetGameInfo();
}

std::vector<int64_t> ShipDebugAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::vector<int64_t> TrickerDebugAPI::GetPlayerGUIDs() const
{
    return logic.GetPlayerGUIDs();
}

std::future<bool> ShipDebugAPI::StartLearning()
{
    logger->info("StartLearning: called at {}ms", Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [this]()
                      { auto result = logic.StartLearning();
                        if (!result)
                            logger->warn("StartLearning: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::StartRouseMate(int64_t mateID)
{
    logger->info("StartRouseMate: mate id={}, called at {}ms", mateID, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.StartRouseMate(mateID);
                        if (!result)
                            logger->warn("StartRouseMate: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::StartEncourageMate(int64_t mateID)
{
    logger->info("StartEncourageMate: mate id={}, called at {}ms", mateID, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.StartEncourageMate(mateID);
                        if (!result)
                            logger->warn("StartEncourageMate: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::Graduate()
{
    logger->info("Graduate: called at {}ms", Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [this]()
                      { auto result = logic.Graduate();
                        if (!result)
                            logger->warn("Graduate: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::shared_ptr<const THUAI7::Ship> ShipDebugAPI::GetSelfInfo() const
{
    return logic.ShipGetSelfInfo();
}

std::future<bool> TrickerDebugAPI::Attack(double angleInRadian)
{
    logger->info("Attack: angleInRadian = {}, called at {}ms", angleInRadian, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.Attack(angleInRadian);
                        if (!result)
                            logger->warn("Attack: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::future<bool> ShipDebugAPI::Attack(double angleInRadian)
{
    logger->info("Attack: angleInRadian = {}, called at {}ms", angleInRadian, Time::TimeSinceStart(startPoint));
    return std::async(std::launch::async, [=]()
                      { auto result = logic.Attack(angleInRadian);
                        if (!result)
                            logger->warn("Attack: failed at {}ms", Time::TimeSinceStart(startPoint));
                        return result; });
}

std::shared_ptr<const THUAI7::Tricker> TrickerDebugAPI::GetSelfInfo() const
{
    return logic.TrickerGetSelfInfo();
}

bool ShipDebugAPI::HaveView(int32_t gridX, int32_t gridY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(gridX, gridY, selfInfo->x, selfInfo->y, selfInfo->viewRange);
}

bool TrickerDebugAPI::HaveView(int32_t gridX, int32_t gridY) const
{
    auto selfInfo = GetSelfInfo();
    return logic.HaveView(gridX, gridY, selfInfo->x, selfInfo->y, selfInfo->viewRange);
}

void ShipDebugAPI::Print(std::string str) const
{
    logger->info(str);
}

void TrickerDebugAPI::Print(std::string str) const
{
    logger->info(str);
}

void ShipDebugAPI::PrintShip() const
{
    for (const auto& Ship : logic.GetShips())
    {
        logger->info("******Ship Info******");
        logger->info("playerID={}, GUID={}, x={}, y={}", Ship->playerID, Ship->guid, Ship->x, Ship->y);
        logger->info("speed={}, view range={}, radius={}", Ship->speed, Ship->viewRange, Ship->radius);
        std::string skillTime = "";
        for (const auto& time : Ship->timeUntilSkillAvailable)
            skillTime += std::to_string(time) + ", ";
        logger->info("score={}, facing direction={}, skill time={}", Ship->score, Ship->facingDirection, skillTime);
        std::string props = "";
        for (const auto& prop : Ship->props)
            props += THUAI7::propTypeDict[prop] + ", ";
        logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[Ship->playerState], THUAI7::bulletTypeDict[Ship->bulletType], props);
        logger->info("type={}, determination={}, addiction={}, danger alert={}", THUAI7::ShipTypeDict[Ship->ShipType], Ship->determination, Ship->addiction, Ship->dangerAlert);
        logger->info("learning speed={}, encourage speed={}, encourage progress={}, rouse progress={}", Ship->learningSpeed, Ship->encourageSpeed, Ship->encourageProgress, Ship->rouseProgress);
        std::string ShipBuff = "";
        for (const auto& buff : Ship->buff)
            ShipBuff += THUAI7::ShipBuffDict[buff] + ", ";
        logger->info("buff={}", ShipBuff);
        logger->info("************************\n");
    }
}

void TrickerDebugAPI::PrintShip() const
{
    for (const auto& Ship : logic.GetShips())
    {
        logger->info("******Ship Info******");
        logger->info("playerID={}, GUID={}, x={}, y={}", Ship->playerID, Ship->guid, Ship->x, Ship->y);
        logger->info("speed={}, view range={}, radius={}", Ship->speed, Ship->viewRange, Ship->radius);
        std::string skillTime = "";
        for (const auto& time : Ship->timeUntilSkillAvailable)
            skillTime += std::to_string(time) + ", ";
        logger->info("score={}, facing direction={}, skill time={}", Ship->score, Ship->facingDirection, skillTime);
        std::string props = "";
        for (const auto& prop : Ship->props)
            props += THUAI7::propTypeDict[prop] + ", ";
        logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[Ship->playerState], THUAI7::bulletTypeDict[Ship->bulletType], props);
        logger->info("type={}, determination={}, addiction={}, danger alert={}", THUAI7::ShipTypeDict[Ship->ShipType], Ship->determination, Ship->addiction, Ship->dangerAlert);
        logger->info("learning speed={}, encourage speed={}, encourage progress={}, rouse progress={}", Ship->learningSpeed, Ship->encourageSpeed, Ship->encourageProgress, Ship->rouseProgress);
        std::string ShipBuff = "";
        for (const auto& buff : Ship->buff)
            ShipBuff += THUAI7::ShipBuffDict[buff] + ", ";
        logger->info("buff={}", ShipBuff);
        logger->info("************************\n");
    }
}

void ShipDebugAPI::PrintTricker() const
{
    for (const auto& tricker : logic.GetTrickers())
    {
        logger->info("******Tricker Info******");
        logger->info("playerID={}, GUID={}, x={}, y={}", tricker->playerID, tricker->guid, tricker->x, tricker->y);
        logger->info("speed={}, view range={},  radius={}", tricker->speed, tricker->viewRange, tricker->radius);
        std::string skillTime = "";
        for (const auto& time : tricker->timeUntilSkillAvailable)
            skillTime += std::to_string(time) + ", ";
        logger->info("score={}, facing direction={}, skill time={}", tricker->score, tricker->facingDirection, skillTime);
        std::string props = "";
        for (const auto& prop : tricker->props)
            props += THUAI7::propTypeDict[prop] + ", ";
        logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[tricker->playerState], THUAI7::bulletTypeDict[tricker->bulletType], props);
        logger->info("type={}, trick desire={}, class volume={}", THUAI7::trickerTypeDict[tricker->trickerType], tricker->trickDesire, tricker->classVolume);
        std::string trickerBuff = "";
        for (const auto& buff : tricker->buff)
            trickerBuff += THUAI7::trickerBuffDict[buff] + ", ";
        logger->info("buff={}", trickerBuff);
        logger->info("************************\n");
    }
}

void TrickerDebugAPI::PrintTricker() const
{
    for (auto tricker : logic.GetTrickers())
    {
        logger->info("******Tricker Info******");
        logger->info("playerID={}, GUID={}, x={}, y={}", tricker->playerID, tricker->guid, tricker->x, tricker->y);
        logger->info("speed={}, view range={},  radius={}", tricker->speed, tricker->viewRange, tricker->radius);
        std::string skillTime = "";
        for (const auto& time : tricker->timeUntilSkillAvailable)
            skillTime += std::to_string(time) + ", ";
        logger->info("score={}, facing direction={}, skill time={}", tricker->score, tricker->facingDirection, skillTime);
        std::string props = "";
        for (const auto& prop : tricker->props)
            props += THUAI7::propTypeDict[prop] + ", ";
        logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[tricker->playerState], THUAI7::bulletTypeDict[tricker->bulletType], props);
        logger->info("type={}, trick desire={}, class volume={}", THUAI7::trickerTypeDict[tricker->trickerType], tricker->trickDesire, tricker->classVolume);
        std::string trickerBuff = "";
        for (const auto& buff : tricker->buff)
            trickerBuff += THUAI7::trickerBuffDict[buff] + ", ";
        logger->info("buff={}", trickerBuff);
        logger->info("************************\n");
    }
}

void ShipDebugAPI::PrintProp() const
{
    for (auto prop : logic.GetProps())
    {
        logger->info("******Prop Info******");
        logger->info("GUID={}, x={}, y={}, facing direction={}", prop->guid, prop->x, prop->y, prop->facingDirection);
        logger->info("*********************\n");
    }
}

void TrickerDebugAPI::PrintProp() const
{
    for (auto prop : logic.GetProps())
    {
        logger->info("******Prop Info******");
        logger->info("GUID={}, x={}, y={}, facing direction={}", prop->guid, prop->x, prop->y, prop->facingDirection);
        logger->info("*********************\n");
    }
}

void ShipDebugAPI::PrintSelfInfo() const
{
    auto Ship = logic.ShipGetSelfInfo();
    logger->info("******Self Info******");
    logger->info("playerID={}, GUID={}, x={}, y={}", Ship->playerID, Ship->guid, Ship->x, Ship->y);
    logger->info("speed={}, view range={}, radius={}", Ship->speed, Ship->viewRange, Ship->radius);
    std::string skillTime = "";
    for (const auto& time : Ship->timeUntilSkillAvailable)
        skillTime += std::to_string(time) + ", ";
    logger->info("score={}, facing direction={}, skill time={}", Ship->score, Ship->facingDirection, skillTime);
    std::string props = "";
    for (const auto& prop : Ship->props)
        props += THUAI7::propTypeDict[prop] + ", ";
    logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[Ship->playerState], THUAI7::bulletTypeDict[Ship->bulletType], props);
    logger->info("type={}, determination={}, addiction={}, danger alert={}", THUAI7::ShipTypeDict[Ship->ShipType], Ship->determination, Ship->addiction, Ship->dangerAlert);
    logger->info("learning speed={}, encourage speed={}, encourage progress={}, rouse progress={}", Ship->learningSpeed, Ship->encourageSpeed, Ship->encourageProgress, Ship->rouseProgress);
    std::string ShipBuff = "";
    for (const auto& buff : Ship->buff)
        ShipBuff += THUAI7::ShipBuffDict[buff] + ", ";
    logger->info("buff={}", ShipBuff);
    logger->info("*********************\n");
}

void TrickerDebugAPI::PrintSelfInfo() const
{
    auto tricker = logic.TrickerGetSelfInfo();
    logger->info("******Self Info******");
    logger->info("playerID={}, GUID={}, x={}, y={}", tricker->playerID, tricker->guid, tricker->x, tricker->y);
    logger->info("speed={}, view range={}, radius={}", tricker->speed, tricker->viewRange, tricker->radius);
    std::string skillTime = "";
    for (const auto& time : tricker->timeUntilSkillAvailable)
        skillTime += std::to_string(time) + ", ";
    logger->info("score={}, facing direction={}, skill time={}", tricker->score, tricker->facingDirection, skillTime);
    std::string props = "";
    for (const auto& prop : tricker->props)
        props += THUAI7::propTypeDict[prop] + ", ";
    logger->info("state={}, bullet={}, props={}", THUAI7::playerStateDict[tricker->playerState], THUAI7::bulletTypeDict[tricker->bulletType], props);
    logger->info("type={}, trick desire={}, class volume={}", THUAI7::trickerTypeDict[tricker->trickerType], tricker->trickDesire, tricker->classVolume);
    std::string trickerBuff = "";
    for (const auto& buff : tricker->buff)
        trickerBuff += THUAI7::trickerBuffDict[buff] + ", ";
    logger->info("buff={}", trickerBuff);
    logger->info("*********************\n");
}

void ShipDebugAPI::Play(IAI& ai)
{
    ai.play(*this);
}

void TrickerDebugAPI::Play(IAI& ai)
{
    ai.play(*this);
}
