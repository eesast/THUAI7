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

Logic::Logic(int32_t pID, int32_t tID, THUAI7::PlayerType pType, THUAI7::ShipType sType) :
    playerID(pID),
    teamID(tID),
    playerType(pType),
    ShipType(sType)
{
    currentState = &state[0];
    bufferState = &state[1];
    currentState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    currentState->mapInfo = std::make_shared<THUAI7::GameMap>();
    bufferState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    bufferState->mapInfo = std::make_shared<THUAI7::GameMap>();
    if (teamID == 0)
        playerTeam = THUAI7::PlayerTeam::Red;
    else if (teamID == 1)
        playerTeam = THUAI7::PlayerTeam::Blue;
    else
        playerTeam = THUAI7::PlayerTeam::NullTeam;
}

std::vector<std::shared_ptr<const THUAI7::Ship>> Logic::GetShips() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Ship>> temp(currentState->ships.begin(), currentState->ships.end());
    logger->debug("Called GetShips");
    return temp;
}

std::vector<std::shared_ptr<const THUAI7::Ship>> Logic::GetEnemyShips() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Ship>> temp(currentState->enemyShips.begin(), currentState->enemyShips.end());
    logger->debug("Called GetEnemyShip");
    return temp;
}

std::vector<std::shared_ptr<const THUAI7::Bullet>> Logic::GetBullets() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Bullet>> temp(currentState->bullets.begin(), currentState->bullets.end());
    logger->debug("Called GetBullets");
    return temp;
}

std::shared_ptr<const THUAI7::Ship> Logic::ShipGetSelfInfo() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called ShipGetSelfInfo");
    return currentState->shipSelf;
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

std::optional<THUAI7::ConstructionState> Logic::GetConstructionState(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetConstructionState");
    auto pos = THUAI7::cellxy_t(cellX, cellY);
    auto it = currentState->mapInfo->factoryState.find(pos);
    auto it2 = currentState->mapInfo->communityState.find(pos);
    auto it3 = currentState->mapInfo->fortState.find(pos);
    if (it != currentState->mapInfo->factoryState.end())
    {
        return std::make_optional<THUAI7::ConstructionState>(currentState->mapInfo->factoryState[pos], THUAI7::ConstructionType::Factory);
    }
    else if (it2 != currentState->mapInfo->communityState.end())
        return std::make_optional<THUAI7::ConstructionState>(currentState->mapInfo->communityState[pos], THUAI7::ConstructionType::Community);
    else if (it3 != currentState->mapInfo->fortState.end())
        return std::make_optional<THUAI7::ConstructionState>(currentState->mapInfo->fortState[pos], THUAI7::ConstructionType::Fort);
    else
    {
        logger->warn("Construction not found");
        return std::nullopt;
    }
}

int32_t Logic::GetWormholeHp(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetWormholeHp");
    auto pos = THUAI7::cellxy_t(cellX, cellY);
    auto it = currentState->mapInfo->wormholeState.find(pos);
    if (it != currentState->mapInfo->wormholeState.end())
    {
        return currentState->mapInfo->wormholeState[pos];
    }
    else
    {
        logger->warn("Wormhole not found");
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

int32_t Logic::GetResourceState(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetResourceState");
    auto pos = THUAI7::cellxy_t(cellX, cellY);
    auto it = currentState->mapInfo->resourceState.find(pos);
    if (it != currentState->mapInfo->resourceState.end())
    {
        return currentState->mapInfo->resourceState[pos];
    }
    else
    {
        logger->warn("Resource not found");
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

bool Logic::Send(int32_t toID, std::string message, bool binary)
{
    logger->debug("Called SendMessage");
    return pComm->Send(playerID, toID, teamID, std::move(message), binary);
}

bool Logic::HaveMessage()
{
    logger->debug("Called HaveMessage");
    return !messageQueue.empty();
}

std::pair<int32_t, std::string> Logic::GetMessage()
{
    logger->debug("Called GetMessage");
    auto msg = messageQueue.tryPop();
    if (msg.has_value())
        return msg.value();
    else
    {
        logger->warn("No message");
        return std::pair(-1, std::string(""));
    }
}

bool Logic::Attack(double angle)
{
    logger->debug("Called Attack");
    return pComm->Attack(playerID, teamID, angle);
}

bool Logic::Recover(int64_t recover)
{
    logger->debug("Called Recover");
    return pComm->Recover(playerID, recover, teamID);
}

bool Logic::Construct(THUAI7::ConstructionType constructiontype)
{
    logger->debug("Called Construct");
    return pComm->Construct(playerID, teamID, constructiontype);
}

bool Logic::BuildShip(THUAI7::ShipType Shiptype, int32_t birthIndex)
{
    logger->debug("Called BuildShip");
    return pComm->BuildShip(teamID, Shiptype, birthIndex);
}

// 等待完成
bool Logic::Recycle(int32_t targetID)
{
    logger->debug("Called Recycle");
    return pComm->Recycle(targetID, teamID);
}

bool Logic::Produce()
{
    logger->debug("Called Produce");
    return pComm->Produce(playerID, teamID);
}

bool Logic::RepairWormhole()
{
    logger->debug("Called RepairWormhole");
    return pComm->RepairWormhole(playerID, teamID);
}

bool Logic::RepairHome()
{
    logger->debug("Called RepairHome");
    return pComm->RepairHome(playerID, teamID);
}

bool Logic::Rebuild(THUAI7::ConstructionType constructionType)
{
    logger->debug("Called Rebuild");
    return pComm->Rebuild(playerID, teamID, constructionType);
}

bool Logic::InstallModule(int32_t playerID, THUAI7::ModuleType moduleType)
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
    if (asynchronous)
        Wait();
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
            pComm->AddPlayer(playerID, teamID, ShipType);
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
                                auto& mapResult = item.map_message();
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
    if (playerType == THUAI7::PlayerType::Ship)  // 本身是船
    {
        for (const auto& item : message.obj_message())
        {
            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage)
            {
                if (item.ship_message().player_id() == playerID && item.ship_message().team_id() == teamID)
                {
                    bufferState->shipSelf = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                    bufferState->ships.push_back(bufferState->shipSelf);
                    logger->debug("Load Self Ship!");
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
            else if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage && item.ship_message().team_id() == teamID)
            {
                std::shared_ptr<THUAI7::Ship> Ship = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                bufferState->ships.push_back(Ship);
                logger->debug("Load Ship!");
            }
        }
    }
}

void Logic::LoadBufferCase(const protobuf::MessageOfObj& item)
{
    if (playerType == THUAI7::PlayerType::Ship)
    {
        int32_t x, y, viewRange;
        x = bufferState->shipSelf->x, y = bufferState->shipSelf->y, viewRange = bufferState->shipSelf->viewRange;
        switch (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()])
        {
            case THUAI7::MessageOfObj::ShipMessage:
                if (teamID != item.ship_message().team_id())
                {
                    if (AssistFunction::HaveView(x, y, item.ship_message().x(), item.ship_message().y(), viewRange, bufferState->gameMap))
                    {
                        std::shared_ptr<THUAI7::Ship> Ship = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                        bufferState->enemyShips.push_back(Ship);
                        logger->debug("Load EnemyShip!");
                    }
                }
                else if (teamID == item.ship_message().team_id() && playerID != item.ship_message().player_id())
                {
                    std::shared_ptr<THUAI7::Ship> Ship = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                    bufferState->ships.push_back(Ship);
                    logger->debug("Load Ship!");
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
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.home_message().x()),
                        AssistFunction::GridToCell(item.home_message().y())
                    );
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::pair(item.home_message().team_id(), item.home_message().hp()));
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
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.home_message().x()),
                        AssistFunction::GridToCell(item.home_message().y())
                    );
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::FactoryMessage:
                if (item.factory_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.factory_message().x()),
                        AssistFunction::GridToCell(item.factory_message().y())
                    );
                    if (bufferState->mapInfo->factoryState.count(pos) == 0)
                    {
                        bufferState->mapInfo->factoryState.emplace(pos, std::pair(item.factory_message().team_id(), item.factory_message().hp()));
                        logger->debug("Load Factory!");
                    }
                    else
                    {
                        bufferState->mapInfo->factoryState[pos].first = item.factory_message().team_id();
                        bufferState->mapInfo->factoryState[pos].second = item.factory_message().hp();
                        logger->debug("Update Factory!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.factory_message().x(), item.factory_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.factory_message().x()),
                        AssistFunction::GridToCell(item.factory_message().y())
                    );
                    if (bufferState->mapInfo->factoryState.count(pos) == 0)
                    {
                        bufferState->mapInfo->factoryState.emplace(pos, std::pair(item.factory_message().team_id(), item.factory_message().hp()));
                        logger->debug("Load Factory!");
                    }
                    else
                    {
                        bufferState->mapInfo->factoryState[pos].first = item.factory_message().team_id();
                        bufferState->mapInfo->factoryState[pos].second = item.factory_message().hp();
                        logger->debug("Update Factory!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::CommunityMessage:
                if (item.community_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.community_message().x()),
                        AssistFunction::GridToCell(item.community_message().y())
                    );
                    if (bufferState->mapInfo->communityState.count(pos) == 0)
                    {
                        bufferState->mapInfo->communityState.emplace(pos, std::pair(item.community_message().team_id(), item.community_message().hp()));
                        logger->debug("Load Community!");
                    }
                    else
                    {
                        bufferState->mapInfo->communityState[pos].first = item.community_message().team_id();
                        bufferState->mapInfo->communityState[pos].second = item.community_message().hp();
                        logger->debug("Update Community!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.community_message().x(), item.community_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.community_message().x()),
                        AssistFunction::GridToCell(item.community_message().y())
                    );
                    if (bufferState->mapInfo->communityState.count(pos) == 0)
                    {
                        bufferState->mapInfo->communityState.emplace(pos, std::pair(item.community_message().team_id(), item.community_message().hp()));
                        logger->debug("Load Community!");
                    }
                    else
                    {
                        bufferState->mapInfo->communityState[pos].first = item.community_message().team_id();
                        bufferState->mapInfo->communityState[pos].second = item.community_message().hp();
                        logger->debug("Update Community!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::FortMessage:
                if (item.fort_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.fort_message().x()),
                        AssistFunction::GridToCell(item.fort_message().y())
                    );
                    if (bufferState->mapInfo->fortState.count(pos) == 0)
                    {
                        bufferState->mapInfo->fortState.emplace(pos, std::pair(item.fort_message().team_id(), item.fort_message().hp()));
                        logger->debug("Load Fort!");
                    }
                    else
                    {
                        bufferState->mapInfo->fortState[pos].first = item.fort_message().team_id();
                        bufferState->mapInfo->fortState[pos].second = item.fort_message().hp();
                        logger->debug("Update Fort!");
                    }
                }
                else if (AssistFunction::HaveView(x, y, item.fort_message().x(), item.fort_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.fort_message().x()),
                        AssistFunction::GridToCell(item.fort_message().y())
                    );
                    if (bufferState->mapInfo->fortState.count(pos) == 0)
                    {
                        bufferState->mapInfo->fortState.emplace(pos, std::pair(item.fort_message().team_id(), item.fort_message().hp()));
                        logger->debug("Load Fort!");
                    }
                    else
                    {
                        bufferState->mapInfo->fortState[pos].first = item.fort_message().team_id();
                        bufferState->mapInfo->fortState[pos].second = item.fort_message().hp();
                        logger->debug("Update Fort!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::WormholeMessage:
                if (AssistFunction::HaveView(x, y, item.wormhole_message().x(), item.wormhole_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.wormhole_message().x()),
                        AssistFunction::GridToCell(item.wormhole_message().y())
                    );
                    if (bufferState->mapInfo->wormholeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->wormholeState.emplace(pos, item.wormhole_message().hp());
                        logger->debug("Load Wormhole!");
                    }
                    else
                    {
                        bufferState->mapInfo->wormholeState[pos] = item.wormhole_message().hp();
                        logger->debug("Update Wormhole!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::ResourceMessage:
                if (AssistFunction::HaveView(x, y, item.resource_message().x(), item.resource_message().y(), viewRange, bufferState->gameMap))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.resource_message().x()),
                        AssistFunction::GridToCell(item.resource_message().y())
                    );
                    if (bufferState->mapInfo->resourceState.count(pos) == 0)
                    {
                        bufferState->mapInfo->resourceState.emplace(pos, item.resource_message().progress());
                        logger->debug("Load Resource!");
                    }
                    else
                    {
                        bufferState->mapInfo->resourceState[pos] = item.resource_message().progress();
                        logger->debug("Update Resource!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::NewsMessage:
                {
                    auto& news = item.news_message();
                    if (news.to_id() == playerID && news.team_id() == teamID)
                    {
                        if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::TextMessage)
                        {
                            messageQueue.emplace(std::pair(news.from_id(), news.text_message()));
                            logger->debug("Load Text News!");
                        }
                        else if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::BinaryMessage)
                        {
                            messageQueue.emplace(std::pair(news.from_id(), news.binary_message()));
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
            for (const auto& ship : bufferState->ships)
            {
                if (AssistFunction::HaveView(ship->x, ship->y, targetX, targetY, ship->viewRange, bufferState->gameMap))
                    return true;
            }
            return false;
        };
        switch (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()])
        {
            case THUAI7::MessageOfObj::ShipMessage:
                if (item.ship_message().team_id() != teamID && HaveOverView(item.ship_message().x(), item.ship_message().y()))
                {
                    std::shared_ptr<THUAI7::Ship> Ship = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                    bufferState->enemyShips.push_back(Ship);
                    logger->debug("Load Enemy Ship!");
                }
                break;
            case THUAI7::MessageOfObj::HomeMessage:
                if (item.home_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.home_message().x()),
                        AssistFunction::GridToCell(item.home_message().y())
                    );
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::pair(item.home_message().team_id(), item.home_message().hp()));
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
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.home_message().x()),
                        AssistFunction::GridToCell(item.home_message().y())
                    );
                    if (bufferState->mapInfo->homeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->homeState.emplace(pos, std::pair(item.home_message().team_id(), item.home_message().hp()));
                        logger->debug("Load Home!");
                    }
                    else
                    {
                        bufferState->mapInfo->homeState[pos].second = item.home_message().hp();
                        logger->debug("Update Home!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::FactoryMessage:
                if (item.factory_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.factory_message().x()),
                        AssistFunction::GridToCell(item.factory_message().y())
                    );
                    if (bufferState->mapInfo->factoryState.count(pos) == 0)
                    {
                        bufferState->mapInfo->factoryState.emplace(pos, std::pair(item.factory_message().team_id(), item.factory_message().hp()));
                        logger->debug("Load Factory!");
                    }
                    else
                    {
                        bufferState->mapInfo->factoryState[pos].first = item.factory_message().team_id();
                        bufferState->mapInfo->factoryState[pos].second = item.factory_message().hp();
                        logger->debug("Update Factory!");
                    }
                }
                else if (HaveOverView(item.factory_message().x(), item.factory_message().y()))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.factory_message().x()),
                        AssistFunction::GridToCell(item.factory_message().y())
                    );
                    if (bufferState->mapInfo->factoryState.count(pos) == 0)
                    {
                        bufferState->mapInfo->factoryState.emplace(pos, std::pair(item.factory_message().team_id(), item.factory_message().hp()));
                        logger->debug("Load Factory!");
                    }
                    else
                    {
                        bufferState->mapInfo->factoryState[pos].first = item.factory_message().team_id();
                        bufferState->mapInfo->factoryState[pos].second = item.factory_message().hp();
                        logger->debug("Update Factory!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::CommunityMessage:
                if (item.community_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.community_message().x()),
                        AssistFunction::GridToCell(item.community_message().y())
                    );
                    if (bufferState->mapInfo->communityState.count(pos) == 0)
                    {
                        bufferState->mapInfo->communityState.emplace(pos, std::pair(item.community_message().team_id(), item.community_message().hp()));
                        logger->debug("Load Community!");
                    }
                    else
                    {
                        bufferState->mapInfo->communityState[pos].first = item.community_message().team_id();
                        bufferState->mapInfo->communityState[pos].second = item.community_message().hp();
                        logger->debug("Update Community!");
                    }
                }
                else if (HaveOverView(item.community_message().x(), item.community_message().y()))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.community_message().x()),
                        AssistFunction::GridToCell(item.community_message().y())
                    );
                    if (bufferState->mapInfo->communityState.count(pos) == 0)
                    {
                        bufferState->mapInfo->communityState.emplace(pos, std::pair(item.community_message().team_id(), item.community_message().hp()));
                        logger->debug("Load Community!");
                    }
                    else
                    {
                        bufferState->mapInfo->communityState[pos].first = item.community_message().team_id();
                        bufferState->mapInfo->communityState[pos].second = item.community_message().hp();
                        logger->debug("Update Community!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::FortMessage:
                if (item.fort_message().team_id() == teamID)
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.fort_message().x()),
                        AssistFunction::GridToCell(item.fort_message().y())
                    );
                    if (bufferState->mapInfo->fortState.count(pos) == 0)
                    {
                        bufferState->mapInfo->fortState.emplace(pos, std::pair(item.fort_message().team_id(), item.fort_message().hp()));
                        logger->debug("Load Fort!");
                    }
                    else
                    {
                        bufferState->mapInfo->fortState[pos].first = item.fort_message().team_id();
                        bufferState->mapInfo->fortState[pos].second = item.fort_message().hp();
                        logger->debug("Update Fort!");
                    }
                }
                else if (HaveOverView(item.fort_message().x(), item.fort_message().y()))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.fort_message().x()),
                        AssistFunction::GridToCell(item.fort_message().y())
                    );
                    if (bufferState->mapInfo->fortState.count(pos) == 0)
                    {
                        bufferState->mapInfo->fortState.emplace(pos, std::pair(item.fort_message().team_id(), item.fort_message().hp()));
                        logger->debug("Load Fort!");
                    }
                    else
                    {
                        bufferState->mapInfo->fortState[pos].first = item.fort_message().team_id();
                        bufferState->mapInfo->fortState[pos].second = item.fort_message().hp();
                        logger->debug("Update Fort!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::WormholeMessage:
                if (HaveOverView(item.wormhole_message().x(), item.wormhole_message().y()))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.wormhole_message().x()),
                        AssistFunction::GridToCell(item.wormhole_message().y())
                    );
                    if (bufferState->mapInfo->wormholeState.count(pos) == 0)
                    {
                        bufferState->mapInfo->wormholeState.emplace(pos, item.wormhole_message().hp());
                        logger->debug("Load Wormhole!");
                    }
                    else
                    {
                        bufferState->mapInfo->wormholeState[pos] = item.wormhole_message().hp();
                        logger->debug("Update Wormhole!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::ResourceMessage:
                if (HaveOverView(item.resource_message().x(), item.resource_message().y()))
                {
                    auto pos = THUAI7::cellxy_t(
                        AssistFunction::GridToCell(item.resource_message().x()),
                        AssistFunction::GridToCell(item.resource_message().y())
                    );
                    if (bufferState->mapInfo->resourceState.count(pos) == 0)
                    {
                        bufferState->mapInfo->resourceState.emplace(pos, item.resource_message().progress());
                        logger->debug("Load Resource!");
                    }
                    else
                    {
                        bufferState->mapInfo->resourceState[pos] = item.resource_message().progress();
                        logger->debug("Update Resource!");
                    }
                }
                break;
            case THUAI7::MessageOfObj::NewsMessage:
                if (item.news_message().team_id() == teamID && item.news_message().to_id() == playerID)
                {
                    auto& news = item.news_message();
                    if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::TextMessage)
                    {
                        messageQueue.emplace(std::pair(news.from_id(), news.text_message()));
                        logger->debug("Load Text News!");
                    }
                    else if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::BinaryMessage)
                    {
                        messageQueue.emplace(std::pair(news.from_id(), news.binary_message()));
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
        bufferState->ships.clear();
        bufferState->enemyShips.clear();
        bufferState->bullets.clear();
        bufferState->guids.clear();
        bufferState->allGuids.clear();
        logger->info("Buffer cleared!");
        // 读取新的信息
        for (const auto& obj : message.obj_message())
            if (Proto2THUAI7::messageOfObjDict[obj.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage)
            {
                bufferState->allGuids.push_back(obj.ship_message().guid());
                if (obj.ship_message().team_id() == teamID)
                    bufferState->guids.push_back(obj.ship_message().guid());
            }
        bufferState->gameInfo = Proto2THUAI7::Protobuf2THUAI7GameInfo(message.all_message());
        LoadBufferSelf(message);
        // 确保这是一个活着的船，否则会使用空指针
        if (playerType == THUAI7::PlayerType::Ship && !bufferState->shipSelf)
        {
            logger->info("exit for nullSelf");
            return;
        }
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
    {
        bufferUpdated = true;
    }
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
    if (playerType == THUAI7::PlayerType::Ship)
        logger->info("Ship ID: {}", playerID);
    logger->info("player team: {}", THUAI7::playerTeamDict[playerTeam]);
    logger->info("****************************");

    // 建立与服务器之间通信的组件
    pComm = std::make_unique<Communication>(IP, port);

    // 构造timer
    if (playerType == THUAI7::PlayerType::Ship)
    {
        if (!file && !print)
            timer = std::make_unique<ShipAPI>(*this);
        else
            timer = std::make_unique<ShipDebugAPI>(*this, file, print, warnOnly, playerID);
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
