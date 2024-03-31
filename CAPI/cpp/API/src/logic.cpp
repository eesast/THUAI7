#include "logic.h"
#include "structures.h"
#include <grpcpp/grpcpp.h>
#include <spdlog/spdlog.h>
#include <spdlog/sinks/basic_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <functional>
#include "utils.hpp"
#include "Communication.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

extern const bool asynchronous;

Logic::Logic(int64_t pID, int64_t tID, THUAI7::PlayerType pType, THUAI7::SweeperType sType) :
    playerID(pID),
    teamID(tID),
    playerType(pType),
    SweeperType(sType)
{
    currentState = &state[0];
    bufferState = &state[1];
    currentState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    currentState->mapInfo = std::make_shared<THUAI7::GameMap>();
    bufferState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    bufferState->mapInfo = std::make_shared<THUAI7::GameMap>();
    if (teamID == 0)
        playerTeam = THUAI7::PlayerTeam::Red;
    if (teamID == 1)
        playerTeam = THUAI7::PlayerTeam::Blue;
    else
        playerTeam = THUAI7::PlayerTeam::NullTeam;
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> Logic::GetSweepers() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Sweeper>> temp(currentState->sweepers.begin(), currentState->sweepers.end());
    logger->debug("Called GetSweepers");
    return temp;
}

std::vector<std::shared_ptr<const THUAI7::Sweeper>> Logic::GetEnemySweepers() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Sweeper>> temp(currentState->enemySweepers.begin(), currentState->enemySweepers.end());
    logger->debug("Called GetEnemySweeper");
    return temp;
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> Logic::GetBullets() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Bullet>> temp(currentState->bullets.begin(), currentState->bullets.end());
    logger->debug("Called GetBullets");
    return temp;
}

std::shared_ptr<const THUAI7::Sweeper> Logic::SweeperGetSelfInfo() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called SweeperGetSelfInfo");
    return currentState->sweeperSelf;
}

std::shared_ptr<const THUAI7::Team> Logic::TeamGetSelfInfo() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called TeamGetSelfInfo");
    return currentState->teamSelf;
}

std::vector<std::vector<THUAI7::PlaceType>> Logic::GetFullMap() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetFullMap");
    return currentState->gameMap;
}

THUAI7::PlaceType Logic::GetPlaceType(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    if (cellX < 0 || uint64_t(cellX) >= currentState->gameMap.size() || cellY < 0 || uint64_t(cellY) >= currentState->gameMap[0].size())
    {
        logger->warn("Invalid position!");
        return THUAI7::PlaceType::NullPlaceType;
    }
    logger->debug("Called GetPlaceType");
    return currentState->gameMap[cellX][cellY];
}

int32_t Logic::GetConstructionHp(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetConstructionHp");
    auto pos = std::make_pair(cellX, cellY);
    auto it = currentState->mapInfo->recycleBankState.find(pos);
    auto it2 = currentState->mapInfo->chargeStationState.find(pos);
    auto it3 = currentState->mapInfo->signalTowerState.find(pos);
    if (it != currentState->mapInfo->recycleBankState.end())
    {
        return currentState->mapInfo->recycleBankState[pos].first;
    }
    else if (it2 != currentState->mapInfo->chargeStationState.end())
        return currentState->mapInfo->chargeStationState[pos].first;
    else if (it3 != currentState->mapInfo->signalTowerState.end())
        return currentState->mapInfo->signalTowerState[pos].first;
    else
    {
        logger->warn("Construction not found");
        return -1;
    }
}

int32_t Logic::GetBridgeHp(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetBridgeHp");
    auto pos = std::make_pair(cellX, cellY);
    auto it = currentState->mapInfo->bridgeState.find(pos);
    if (it != currentState->mapInfo->bridgeState.end())
    {
        return currentState->mapInfo->bridgeState[pos];
    }
    else
    {
        logger->warn("Bridge not found");
        return -1;
    }
}
int32_t Logic::GetHomeHp() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetHomeHp");
    if (playerTeam == THUAI7::PlayerTeam::Red)
        return currentState->gameInfo->redHomeHp;
    else
        return currentState->gameInfo->blueHomeHp;
}

int32_t Logic::GetGarbageState(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetGarbageState");
    auto pos = std::make_pair(cellX, cellY);
    auto it = currentState->mapInfo->garbageState.find(pos);
    if (it != currentState->mapInfo->garbageState.end())
    {
        return currentState->mapInfo->garbageState[pos];
    }
    else
    {
        logger->warn("Garbage not found");
        return -1;
    }
}

int32_t Logic::GetEnergy() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetEnergy");
    if (playerTeam == THUAI7::PlayerTeam::Red)
        return currentState->gameInfo->redEnergy;
    else if (playerTeam == THUAI7::PlayerTeam::Blue)
        return currentState->gameInfo->blueEnergy;
    else
    {
        logger->warn("Invalid playerTeam");
        return -1;
    }
}
int32_t Logic::GetScore() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetScore");
    if (playerTeam == THUAI7::PlayerTeam::Red)
        return currentState->gameInfo->redScore;
    else if (playerTeam == THUAI7::PlayerTeam::Blue)
        return currentState->gameInfo->blueScore;
    else
    {
        logger->warn("Invalid playerTeam");
        return -1;
    }
}

std::shared_ptr<const THUAI7::GameInfo> Logic::GetGameInfo() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetGameInfo");
    return currentState->gameInfo;
}

bool Logic::Move(int64_t time, double angle)
{
    logger->debug("Called Move");
    return pComm->Move(playerID, teamID, time, angle);
}

bool Logic::Send(int64_t toID, std::string message, bool binary)
{
    logger->debug("Called SendMessage");
    return pComm->Send(playerID, teamID, toID, std::move(message), binary);
}

bool Logic::HaveMessage()
{
    logger->debug("Called HaveMessage");
    return !messageQueue.empty();
}

std::pair<int64_t, std::string> Logic::GetMessage()
{
    logger->debug("Called GetMessage");
    auto msg = messageQueue.tryPop();
    if (msg.has_value())
        return msg.value();
    else
    {
        logger->warn("No message");
        return std::make_pair(-1, "");
    }
}

bool Logic::Attack(double angle)
{
    logger->debug("Called Attack");
    return pComm->Attack(playerID, teamID, angle);
}

bool Logic::Recover()
{
    logger->debug("Called Recover");
    // TODO recover
    int64_t recover = 1;
    return pComm->Recover(playerID, recover, teamID);
}

bool Logic::Construct(THUAI7::ConstructionType constructiontype)
{
    logger->debug("Called Construct");
    return pComm->Construct(playerID, teamID, constructiontype);
}

bool Logic::BuildSweeper(THUAI7::SweeperType Sweepertype)
{
    logger->debug("Called BuildSweeper");
    return pComm->BuildSweeper(teamID, Sweepertype);
}

// 等待完成
bool Logic::Recycle(int64_t targetID)
{
    logger->debug("Called Recycle");
    return pComm->Recycle(targetID, teamID);
}

bool Logic::Produce()
{
    logger->debug("Called Produce");
    return pComm->Produce(playerID, teamID);
}

bool Logic::Rebuild(THUAI7::ConstructionType constructionType)
{
    logger->debug("Called Rebuild");
    return pComm->Rebuild(playerID, teamID, constructionType);
}

bool Logic::InstallModule(int64_t playerID, THUAI7::ModuleType moduleType)
{
    logger->debug("Called InstallModule");
    return pComm->InstallModule(playerID, teamID, moduleType);
}

bool Logic::EndAllAction()
{
    logger->debug("Called EndAllAction");
    return pComm->EndAllAction(playerID, teamID);
}

bool Logic::WaitThread()
{
    Update();
    return true;
}

void Logic::ProcessMessage()
{
    auto messageThread = [this]()
    {
        try
        {
            // TODO
            logger->info("Message thread start!");
            pComm->AddPlayer(playerID, teamID, SweeperType);
            while (gameState != THUAI7::GameState::GameEnd)
            {
                auto clientMsg = pComm->GetMessage2Client();  // 在获得新消息之前阻塞
                logger->debug("Get message from server!");
                gameState = Proto2THUAI7::gameStateDict[clientMsg.game_state()];
                switch (gameState)
                {
                    case THUAI7::GameState::GameStart:
                        logger->info("Game Start!");

                        // 读取地图
                        for (const auto& item : clientMsg.obj_message())
                        {
                            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::MapMessage)
                            {
                                auto map = std::vector<std::vector<THUAI7::PlaceType>>();
                                auto mapResult = item.map_message();
                                for (int32_t i = 0; i < item.map_message().rows_size(); i++)
                                {
                                    std::vector<THUAI7::PlaceType> row;
                                    for (int32_t j = 0; j < mapResult.rows(i).cols_size(); j++)
                                    {
                                        if (Proto2THUAI7::placeTypeDict.count(mapResult.rows(i).cols(j)) == 0)
                                            logger->error("Unknown place type!");
                                        row.push_back(Proto2THUAI7::placeTypeDict[mapResult.rows(i).cols(j)]);
                                    }
                                    map.push_back(std::move(row));
                                }
                                bufferState->gameMap = std::move(map);
                                currentState->gameMap = bufferState->gameMap;
                                logger->info("Map loaded!");
                                break;
                            }
                        }
                        if (currentState->gameMap.empty())
                        {
                            logger->error("Map not loaded!");
                            throw std::runtime_error("Map not loaded!");
                        }
                        LoadBuffer(clientMsg);

                        AILoop = true;
                        UnBlockAI();

                        break;
                    case THUAI7::GameState::GameRunning:

                        LoadBuffer(clientMsg);
                        break;
                    default:
                        logger->debug("Unknown GameState!");
                        break;
                }
            }
            {
                std::lock_guard<std::mutex> lock(mtxBuffer);
                bufferUpdated = true;
                counterBuffer = -1;
            }
            cvBuffer.notify_one();
            logger->info("Game End!");
            AILoop = false;
        }
        catch (const std::exception& e)
        {
            std::cerr << "C++ Exception: " << e.what() << std::endl;
            AILoop = false;
        }
        catch (...)
        {
            std::cerr << "Unknown Exception!" << std::endl;
            AILoop = false;
        }
    };
    std::thread(messageThread).detach();
}

void Logic::LoadBufferSelf(const protobuf::MessageToClient& message)
{
    if (playerType == THUAI7::PlayerType::Sweeper)  // 本身是船
    {
        for (const auto& item : message.obj_message())
        {
            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::SweeperMessage)
            {
                if (item.sweeper_message().player_id() == playerID && item.sweeper_message().team_id() == teamID)
                {
                    bufferState->sweeperSelf = Proto2THUAI7::Protobuf2THUAI7Sweeper(item.sweeper_message());
                    bufferState->sweepers.push_back(bufferState->sweeperSelf);
                    logger->debug("Load Self Sweeper!");
                }
            }
        }
    }
    else if (playerType == THUAI7::PlayerType::Team)
    {
        for (const auto& item : message.obj_message())
        {
            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::TeamMessage && item.team_message().team_id() == teamID)
            {
                bufferState->teamSelf = Proto2THUAI7::Protobuf2THUAI7Team(item.team_message());
                logger->debug("Load Self Team!");
            }
            else if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::SweeperMessage && item.team_message().team_id() == teamID)
            {
                std::shared_ptr<THUAI7::Sweeper> Sweeper = Proto2THUAI7::Protobuf2THUAI7Sweeper(item.sweeper_message());
                bufferState->sweepers.push_back(Sweeper);
                logger->debug("Load Sweeper!");
            }
        }
    }
}

void Logic::LoadBufferCase(const protobuf::MessageOfObj& item)
{
    if (playerType == THUAI7::PlayerType::Sweeper)
    {
        int32_t x, y, viewRange;
        x = bufferState->sweeperSelf->x, y = bufferState->sweeperSelf->y, viewRange = bufferState->sweeperSelf->viewRange;
        switch (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()])
        {
            case THUAI7::MessageOfObj::SweeperMessage:
                if (teamID != item.sweeper_message().team_id())
                {
                    if (AssistFunction::HaveView(x, y, item.sweeper_message().x(), item.sweeper_message().y(), viewRange, bufferState->gameMap))
                    {
                        std::shared_ptr<THUAI7::Sweeper> Sweeper = Proto2THUAI7::Protobuf2THUAI7Sweeper(item.sweeper_message());
                        bufferState->enemySweepers.push_back(Sweeper);
                        logger->debug("Load EnemySweeper!");
                    }
                }
                else if (teamID == item.sweeper_message().team_id() && playerID != item.sweeper_message().player_id())
                {
                    std::shared_ptr<THUAI7::Sweeper> Sweeper = Proto2THUAI7::Protobuf2THUAI7Sweeper(item.sweeper_message());
                    bufferState->sweepers.push_back(Sweeper);
                    logger->debug("Load Sweeper!");
                }
                break;
            case THUAI7::MessageOfObj::BulletMessage:
                if (item.bullet_message().team_id() != teamID && AssistFunction::HaveView(x, y, item.bullet_message().x(), item.bullet_message().y(), viewRange, bufferState->gameMap))
                {
                    bufferState->bullets.push_back(Proto2THUAI7::Protobuf2THUAI7Bullet(item.bullet_message()));
                    logger->debug("Load Bullet!");
                }
                break;
            case THUAI7::MessageOfObj::HomeMessage:
                if (item.home_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.home_message().x()), AssistFunction::GridToCell(item.home_message().y()));
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::make_pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.home_message().x(), item.home_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.home_message().x()), AssistFunction::GridToCell(item.home_message().y()));
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::make_pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::RecycleBankMessage:
                if (item.recyclebank_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.recyclebank_message().x()), AssistFunction::GridToCell(item.recyclebank_message().y()));
                    if (bufferState->mapInfo->recycleBankState.count(pos) == 0)
                    {
                        bufferState->mapInfo->recycleBankState.emplace(pos, std::make_pair(item.recyclebank_message().team_id(), item.recyclebank_message().hp()));
                        logger->debug("Load RecycleBank!");
                    }
                    else
                    {
                        bufferState->mapInfo->recycleBankState[pos].second = item.recyclebank_message().hp();
                        logger->debug("Update RecycleBank!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.recyclebank_message().x(), item.recyclebank_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.recyclebank_message().x()), AssistFunction::GridToCell(item.recyclebank_message().y()));
                    if (bufferState->mapInfo->recycleBankState.count(pos) == 0)
                    {
                        bufferState->mapInfo->recycleBankState.emplace(pos, std::make_pair(item.recyclebank_message().team_id(), item.recyclebank_message().hp()));
                        logger->debug("Load RecycleBank!");
                    }
                    else
                    {
                        bufferState->mapInfo->recycleBankState[pos].second = item.recyclebank_message().hp();
                        logger->debug("Update RecycleBank!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::ChargeStationMessage:
                if (item.chargestation_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.chargestation_message().x()), AssistFunction::GridToCell(item.chargestation_message().y()));
                    if (bufferState->mapInfo->chargeStationState.count(pos) == 0)
                    {
                        bufferState->mapInfo->chargeStationState.emplace(pos, std::make_pair(item.chargestation_message().team_id(), item.chargestation_message().hp()));
                        logger->debug("Load ChargeStation!");
                    }
                    else
                    {
                        bufferState->mapInfo->chargeStationState[pos].second = item.chargestation_message().hp();
                        logger->debug("Update ChargeStation!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.chargestation_message().x(), item.chargestation_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.chargestation_message().x()), AssistFunction::GridToCell(item.chargestation_message().y()));
                    if (bufferState->mapInfo->chargeStationState.count(pos) == 0)
                    {
                        bufferState->mapInfo->chargeStationState.emplace(pos, std::make_pair(item.chargestation_message().team_id(), item.chargestation_message().hp()));
                        logger->debug("Load ChargeStation!");
                    }
                    else
                    {
                        bufferState->mapInfo->chargeStationState[pos].second = item.chargestation_message().hp();
                        logger->debug("Update ChargeStation!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::SignalTowerMessage:
                if (item.signaltower_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.signaltower_message().x()), AssistFunction::GridToCell(item.signaltower_message().y()));
                    if (bufferState->mapInfo->signalTowerState.count(pos) == 0)
                    {
                        bufferState->mapInfo->signalTowerState.emplace(pos, std::make_pair(item.signaltower_message().team_id(), item.signaltower_message().hp()));
                        logger->debug("Load SignalTower!");
                    }
                    else
                    {
                        bufferState->mapInfo->signalTowerState[pos].second = item.signaltower_message().hp();
                        logger->debug("Update SignalTower!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.signaltower_message().x(), item.signaltower_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.signaltower_message().x()), AssistFunction::GridToCell(item.signaltower_message().y()));
                    if (bufferState->mapInfo->signalTowerState.count(pos) == 0)
                    {
                        bufferState->mapInfo->signalTowerState.emplace(pos, std::make_pair(item.signaltower_message().team_id(), item.signaltower_message().hp()));
                        logger->debug("Load SignalTower!");
                    }
                    else
                    {
                        bufferState->mapInfo->signalTowerState[pos].second = item.signaltower_message().hp();
                        logger->debug("Update SignalTower!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::BridgeMessage:
                if (AssistFunction::HaveView(x, y, item.bridge_message().x(), item.bridge_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.bridge_message().x()), AssistFunction::GridToCell(item.bridge_message().y()));
                    if (bufferState->mapInfo->bridgeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->bridgeState.emplace(pos, item.bridge_message().hp());
                        logger->debug("Load Bridge!");
                    }
                    else
                    {
                        bufferState->mapInfo->bridgeState[pos] = item.bridge_message().hp();
                        logger->debug("Update Bridge!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::GarbageMessage:
                if (AssistFunction::HaveView(x, y, item.garbage_message().x(), item.garbage_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.garbage_message().x()), AssistFunction::GridToCell(item.garbage_message().y()));
                    if (bufferState->mapInfo->garbageState.count(pos) == 0)
                    {
                        bufferState->mapInfo->garbageState.emplace(pos, item.garbage_message().progress());
                        logger->debug("Load Garbage!");
                    }
                    else
                    {
                        bufferState->mapInfo->garbageState[pos] = item.garbage_message().progress();
                        logger->debug("Update Garbage!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::NewsMessage:
                {
                    auto news = item.news_message();
                    if (news.to_id() == playerID && news.team_id() == teamID)
                    {
                        if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::TextMessage)
                        {
                            messageQueue.emplace(std::make_pair(news.from_id(), news.text_message()));
                            logger->debug("Load Text News!");
                        }
                        else if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::BinaryMessage)
                        {
                            messageQueue.emplace(std::make_pair(news.from_id(), news.binary_message()));
                            logger->debug("Load Binary News!");
                        }
                        else
                            logger->error("Unknown NewsType!");
                    }
                    break;
                }
            case THUAI7::MessageOfObj::NullMessageOfObj:
            default:
                break;
        }
    }
    else if (playerType == THUAI7::PlayerType::Team)
    {
        auto HaveOverView = [&](int32_t targetX, int32_t targetY)
        {
            for (const auto& sweeper : bufferState->sweepers)
            {
                if (AssistFunction::HaveView(sweeper->x, sweeper->y, targetX, targetY, sweeper->viewRange, bufferState->gameMap))
                    return true;
            }
            return false;
        };
        switch (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()])
        {
            case THUAI7::MessageOfObj::SweeperMessage:
                if (item.sweeper_message().team_id() != teamID && HaveOverView(item.sweeper_message().x(), item.sweeper_message().y()))
                {
                    std::shared_ptr<THUAI7::Sweeper> Sweeper = Proto2THUAI7::Protobuf2THUAI7Sweeper(item.sweeper_message());
                    bufferState->enemySweepers.push_back(Sweeper);
                    logger->debug("Load Enemy Sweeper!");
                }
                break;
            case THUAI7::MessageOfObj::HomeMessage:
                if (item.home_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.home_message().x()), AssistFunction::GridToCell(item.home_message().y()));
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::make_pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                else if (HaveOverView(item.home_message().x(), item.home_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.home_message().x()), AssistFunction::GridToCell(item.home_message().y()));
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::make_pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::RecycleBankMessage:
                if (item.recyclebank_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.recyclebank_message().x()), AssistFunction::GridToCell(item.recyclebank_message().y()));
                    if (bufferState->mapInfo->recycleBankState.count(pos) == 0)
                    {
                        bufferState->mapInfo->recycleBankState.emplace(pos, std::make_pair(item.recyclebank_message().team_id(), item.recyclebank_message().hp()));
                        logger->debug("Load RecycleBank!");
                    }
                    else
                    {
                        bufferState->mapInfo->recycleBankState[pos].second = item.recyclebank_message().hp();
                        logger->debug("Update RecycleBank!");
                    }
                }
                else if (HaveOverView(item.recyclebank_message().x(), item.recyclebank_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.recyclebank_message().x()), AssistFunction::GridToCell(item.recyclebank_message().y()));
                    if (bufferState->mapInfo->recycleBankState.count(pos) == 0)
                    {
                        bufferState->mapInfo->recycleBankState.emplace(pos, std::make_pair(item.recyclebank_message().team_id(), item.recyclebank_message().hp()));
                        logger->debug("Load RecycleBank!");
                    }
                    else
                    {
                        bufferState->mapInfo->recycleBankState[pos].second = item.recyclebank_message().hp();
                        logger->debug("Update RecycleBank!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::ChargeStationMessage:
                if (item.chargestation_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.chargestation_message().x()), AssistFunction::GridToCell(item.chargestation_message().y()));
                    if (bufferState->mapInfo->chargeStationState.count(pos) == 0)
                    {
                        bufferState->mapInfo->chargeStationState.emplace(pos, std::make_pair(item.chargestation_message().team_id(), item.chargestation_message().hp()));
                        logger->debug("Load ChargeStation!");
                    }
                    else
                    {
                        bufferState->mapInfo->chargeStationState[pos].second = item.chargestation_message().hp();
                        logger->debug("Update ChargeStation!");
                    }
                }
                else if (HaveOverView(item.chargestation_message().x(), item.chargestation_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.chargestation_message().x()), AssistFunction::GridToCell(item.chargestation_message().y()));
                    if (bufferState->mapInfo->chargeStationState.count(pos) == 0)
                    {
                        bufferState->mapInfo->chargeStationState.emplace(pos, std::make_pair(item.chargestation_message().team_id(), item.chargestation_message().hp()));
                        logger->debug("Load ChargeStation!");
                    }
                    else
                    {
                        bufferState->mapInfo->chargeStationState[pos].second = item.chargestation_message().hp();
                        logger->debug("Update ChargeStation!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::SignalTowerMessage:
                if (item.signaltower_message().team_id() == teamID)
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.signaltower_message().x()), AssistFunction::GridToCell(item.signaltower_message().y()));
                    if (bufferState->mapInfo->signalTowerState.count(pos) == 0)
                    {
                        bufferState->mapInfo->signalTowerState.emplace(pos, std::make_pair(item.signaltower_message().team_id(), item.signaltower_message().hp()));
                        logger->debug("Load SignalTower!");
                    }
                    else
                    {
                        bufferState->mapInfo->signalTowerState[pos].second = item.signaltower_message().hp();
                        logger->debug("Update SignalTower!");
                    }
                }
                else if (HaveOverView(item.signaltower_message().x(), item.signaltower_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.signaltower_message().x()), AssistFunction::GridToCell(item.signaltower_message().y()));
                    if (bufferState->mapInfo->signalTowerState.count(pos) == 0)
                    {
                        bufferState->mapInfo->signalTowerState.emplace(pos, std::make_pair(item.signaltower_message().team_id(), item.signaltower_message().hp()));
                        logger->debug("Load SignalTower!");
                    }
                    else
                    {
                        bufferState->mapInfo->signalTowerState[pos].second = item.signaltower_message().hp();
                        logger->debug("Update SignalTower!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::BridgeMessage:
                if (HaveOverView(item.bridge_message().x(), item.bridge_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.bridge_message().x()), AssistFunction::GridToCell(item.bridge_message().y()));
                    if (bufferState->mapInfo->bridgeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->bridgeState.emplace(pos, item.bridge_message().hp());
                        logger->debug("Load Bridge!");
                    }
                    else
                    {
                        bufferState->mapInfo->bridgeState[pos] = item.bridge_message().hp();
                        logger->debug("Update Bridge!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::GarbageMessage:
                if (HaveOverView(item.garbage_message().x(), item.garbage_message().y()))
                {
                    auto pos = std::make_pair(AssistFunction::GridToCell(item.garbage_message().x()), AssistFunction::GridToCell(item.garbage_message().y()));
                    if (bufferState->mapInfo->garbageState.count(pos) == 0)
                    {
                        bufferState->mapInfo->garbageState.emplace(pos, item.garbage_message().progress());
                        logger->debug("Load Garbage!");
                    }
                    else
                    {
                        bufferState->mapInfo->garbageState[pos] = item.garbage_message().progress();
                        logger->debug("Update Garbage!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::NewsMessage:
                if (item.news_message().team_id() == teamID && item.news_message().to_id() == playerID)
                {
                    auto news = item.news_message();
                    if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::TextMessage)
                    {
                        messageQueue.emplace(std::make_pair(news.from_id(), news.text_message()));
                        logger->debug("Load Text News!");
                    }
                    else if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::BinaryMessage)
                    {
                        messageQueue.emplace(std::make_pair(news.from_id(), news.binary_message()));
                        logger->debug("Load Binary News!");
                    }
                    else
                        logger->error("Unknown NewsType!");
                }
                break;
            case THUAI7::MessageOfObj::NullMessageOfObj:
            default:
                break;
        }
    }
}

void Logic::LoadBuffer(const protobuf::MessageToClient& message)
{
    // 将消息读入到buffer中
    {
        std::lock_guard<std::mutex> lock(mtxBuffer);

        // 清空原有信息
        bufferState->sweepers.clear();
        bufferState->enemySweepers.clear();
        bufferState->bullets.clear();
        bufferState->guids.clear();

        logger->debug("Buffer cleared!");
        // 读取新的信息
        for (const auto& obj : message.obj_message())
            if (Proto2THUAI7::messageOfObjDict[obj.message_of_obj_case()] == THUAI7::MessageOfObj::SweeperMessage)
                bufferState->guids.push_back(obj.sweeper_message().guid());
        // TODO
        // else if (Proto2THUAI7::messageOfObjDict[obj.message_of_obj_case()] == THUAI7::MessageOfObj::HomeMessage)
        //     bufferState->guids.push_back(obj.home_message().guid());
        bufferState->gameInfo = Proto2THUAI7::Protobuf2THUAI7GameInfo(message.all_message());
        LoadBufferSelf(message);
        for (const auto& item : message.obj_message())
            LoadBufferCase(item);
    }
    if (asynchronous)
    {
        {
            std::lock_guard<std::mutex> lock(mtxState);
            std::swap(currentState, bufferState);
            counterState = counterBuffer;
            logger->info("Update State!");
        }
        freshed = true;
    }
    else
        bufferUpdated = true;
    counterBuffer++;
    // 唤醒其他线程
    cvBuffer.notify_one();
}

void Logic::Update() noexcept
{
    if (!asynchronous)
    {
        std::unique_lock<std::mutex> lock(mtxBuffer);

        // 缓冲区被更新之后才可以使用
        cvBuffer.wait(lock, [this]()
                      { return bufferUpdated; });
        {
            std::lock_guard<std::mutex> stateLock(mtxState);
            std::swap(currentState, bufferState);
            counterState = counterBuffer;
        }
        bufferUpdated = false;
        logger->info("Update State!");
    }
}

void Logic::Wait() noexcept
{
    freshed = false;
    {
        std::unique_lock<std::mutex> lock(mtxBuffer);
        cvBuffer.wait(lock, [this]()
                      { return freshed.load(); });
    }
}

void Logic::UnBlockAI()
{
    {
        std::lock_guard<std::mutex> lock(mtxAI);
        AIStart = true;
    }
    cvAI.notify_one();
}

int32_t Logic::GetCounter() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    return counterState;
}

std::vector<int64_t> Logic::GetPlayerGUIDs() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    return currentState->guids;
}

bool Logic::TryConnection()
{
    logger->info("Try to connect to server...");
    return pComm->TryConnection(playerID, teamID);
}

bool Logic::HaveView(int32_t selfX, int32_t selfY, int32_t targetX, int32_t targetY, int32_t viewRange) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    return AssistFunction::HaveView(selfX, selfY, targetX, targetY, viewRange, currentState->gameMap);
}

void Logic::Main(CreateAIFunc createAI, std::string IP, std::string port, bool file, bool print, bool warnOnly)
{
    // 建立日志组件
    auto fileLogger = std::make_shared<spdlog::sinks::basic_file_sink_mt>(fmt::format("logs/logic-{}-log.txt", playerID), true);
    auto printLogger = std::make_shared<spdlog::sinks::stdout_color_sink_mt>();
    std::string pattern = "[logic] [%H:%M:%S.%e] [%l] %v";
    fileLogger->set_pattern(pattern);
    printLogger->set_pattern(pattern);
    if (file)
        fileLogger->set_level(spdlog::level::debug);
    else
        fileLogger->set_level(spdlog::level::off);
    if (print)
        printLogger->set_level(spdlog::level::info);
    else
        printLogger->set_level(spdlog::level::off);
    if (warnOnly)
        printLogger->set_level(spdlog::level::warn);
    logger = std::make_unique<spdlog::logger>("logicLogger", spdlog::sinks_init_list{fileLogger, printLogger});
    logger->flush_on(spdlog::level::warn);

    // 打印当前的调试信息
    logger->info("*********Basic Info*********");
    logger->info("asynchronous: {}", asynchronous);
    logger->info("server: {}:{}", IP, port);
    if (playerType == THUAI7::PlayerType::Sweeper)
        logger->info("Sweeper ID: {}", playerID);
    logger->info("player team: {}", THUAI7::playerTeamDict[playerTeam]);
    logger->info("****************************");

    // 建立与服务器之间通信的组件
    pComm = std::make_unique<Communication>(IP, port);

    // 构造timer
    if (playerType == THUAI7::PlayerType::Sweeper)
    {
        if (!file && !print)
            timer = std::make_unique<SweeperAPI>(*this);
        else
            timer = std::make_unique<SweeperDebugAPI>(*this, file, print, warnOnly, playerID);
    }
    else
    {
        if (!file && !print)
            timer = std::make_unique<TeamAPI>(*this);
        else
            timer = std::make_unique<TeamDebugAPI>(*this, file, print, warnOnly, playerID);
    }

    // 构造AI线程
    auto AIThread = [&]()
    {
        try
        {
            {
                std::unique_lock<std::mutex> lock(mtxAI);
                cvAI.wait(lock, [this]()
                          { return AIStart; });
            }
            auto ai = createAI(playerID);

            while (AILoop)
            {
                if (asynchronous)
                {
                    Wait();
                    timer->StartTimer();
                    timer->Play(*ai);
                    timer->EndTimer();
                }
                else
                {
                    Update();
                    timer->StartTimer();
                    timer->Play(*ai);
                    timer->EndTimer();
                }
            }
        }
        catch (const std::exception& e)
        {
            std::cerr << "C++ Exception: " << e.what() << std::endl;
        }
        catch (...)
        {
            std::cerr << "Unknown Exception!" << std::endl;
        }
    };

    // 连接服务器
    if (TryConnection())
    {
        logger->info("Connect to the server successfully, AI thread will be started.");
        tAI = std::thread(AIThread);
        if (tAI.joinable())
        {
            logger->info("Join the AI thread!");
            // 首先开启处理消息的线程
            ProcessMessage();
            tAI.join();
        }
    }
    else
    {
        AILoop = false;
        logger->error("Connect to the server failed, AI thread will not be started.");
        return;
    }
}
