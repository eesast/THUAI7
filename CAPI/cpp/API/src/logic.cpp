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

Logic::Logic(int64_t ID, THUAI7::PlayerTeam team, bool ISship) :
    APIid(ID),
    playerTeam(team),
    isShip(ISship),

{
    currentState = &state[0];
    bufferState = &state[1];
    currentState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    currentState->mapInfo = std::make_shared<THUAI7::GameMap>();
    bufferState->gameInfo = std::make_shared<THUAI7::GameInfo>();
    bufferState->mapInfo = std::make_shared<THUAI7::GameMap>();
}

std::vector<std::shared_ptr<const THUAI7::Ship>> Logic::GetShips() const
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Ship>> temp(currentState->ships.begin(), currentState->ships.end());
    logger->debug("Called GetShips");
    return temp;
}

std::vector<std::shared_ptr<const THUAI7::Ship>> Logic::GetEnemyShip()
{
    std::unique_lock<std::mutex> lock(mtxState);
    std::vector<std::shared_ptr<const THUAI7::Ship>> temp(currentState->enemyships.begin(), currentState->enemyships.end());
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

std::shared_ptr<const THUAI7::Ship> Logic::GetSelfInfo()
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called ShipGetSelfInfo");
    return currentState->shipSelf;
}

std::shared_ptr<const THUAI7::Home> Logic::GetSelfInfo()
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called HomeGetSelfInfo");
    return currentState->homeSelf;
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

int32_t Logic::GetBuildingHp(int32_t cellX, int32_t cellY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetBuildingHp");
    auto pos = std::make_pair(cellX, cellY);
    auto it=currentState->mapInfo->factoryState.find(pos);
    auto it2=currentState->mapInfo->communityState.find(pos);
    auto it3=currentState->mapInfo->fortressState.find(pos);
    if(it!=currentState->mapInfo->factoryState.end())
    {
        return currentState->mapInfo->factoryState[pos];
    }
    else if(it2!=currentState->mapInfo->communityState.end())
        return currentState->mapInfo->communityState[pos];
    else if(it3!=currentState->mapInfo->fortressState.end())
        return currentState->mapInfo->fortressState[pos];
    else
    {
        logger->warn("Building not found");
        return -1;
    }
}

int32_t Logic::GetWormHp(int32_t cellX, int32_t cellY) 
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetWormHp");
    auto pos = std::make_pair(cellX, cellY);
    auto it=currentState->mapInfo->wormState.find(pos);
    if(it!=currentState->mapInfo->wormState.end())
    {
        return currentState->mapInfo->wormState[pos];
    }
    else
    {
        logger->warn("Worm not found");
        return -1;
    }
}

int32_t GetResourceState(int32_t cellX, int32_t cellY)
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetResourceState");
    auto pos = std::make_pair(cellX, cellY);
    auto it=currentState->mapInfo->resourceState.find(pos);
    if(it!=currentState->mapInfo->resourceState.end())
    {
        return currentState->mapInfo->resourceState[pos];
    }
    else
    {
        logger->warn("Resource not found");
        return -1;
    }
}

int32_t Logic::GetEconomy()
{
    std::unique_lock<std::mutex> lock(mtxState);
    logger->debug("Called GetEconomy");
    if(playerTeam == THUAI7::PlayerTeam::Up)
        return currentState->gameInfo->upEconomy;
    else if(playerTeam ==THUAI7::PlayerTeam::Down)
        return currentState->gameInfo->downEconomy;
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
    return pComm->Move(time, angle, APIid);
}

bool Logic::SendMessage(int64_t toID, std::string message, bool binary)
{
    logger->debug("Called SendMessage");
    return pComm->SendMessage(toID, std::move(message), binary, playerID);
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
    return pComm->Attack(angle, playerID);
}


bool Logic::Recover()
{
    logger->debug("Called Recover");
    return pComm->Recover(APIid);
}

//等待完成
bool Logic::Recycle()
{
    // logger->debug("Called Recycle");
    // return pComm->Recycle(APIid);
}

bool Logic::Produce(int32_t x, int32_t y);
{
     logger->debug("Called Produce");
     return pComm->Produce(APIid,x,y);
}

bool Logic::Rebuild(int32_t cellX, int32_t cellY)
{
    logger->debug("Called Rebuild");
    return pComm->Rebuild(APIid, cellX, cellY);
}

bool Logic::InstallModule(THUAI7::Module module)
{
    logger->debug("Called InstallModule");
    return pComm->InstallModule(module, APIid);
}

bool Logic::EndAllAction()
{
    logger->debug("Called EndAllAction");
    return pComm->EndAllAction(APIid);
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
            logger->info("Message thread start!");
            pComm->AddShip(APIid, shipType, playerTeam);
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
                                for (int32_t i = 0; i < item.map_message().row_size(); i++)
                                {
                                    std::vector<THUAI7::PlaceType> row;
                                    for (int32_t j = 0; j < mapResult.row(i).col_size(); j++)
                                    {
                                        if (Proto2THUAI7::placeTypeDict.count(mapResult.row(i).col(j)) == 0)
                                            logger->error("Unknown place type!");
                                        row.push_back(Proto2THUAI7::placeTypeDict[mapResult.row(i).col(j)]);
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
    if(isShip)//本身是船
    {
        for (const auto& item : message.obj_message())
        {
            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage)
            {
                if (item.ship_message().ship_id() == APIid)
                {
                    bufferState->shipSelf = Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                    bufferState->ships.push_back(bufferState->shipSelf);
                }
                else
                {
                    std::shared_ptr<THUAI7::Ship> ship=Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message());
                    if(ship->team==playerTeam)
                        bufferState->ships.push_back(ship);
                }
                logger->debug("Add Ship!");
            }
        }
    }
    else//本身是大本营
    {
        for (const auto& item : message.obj_message())
        {
            if (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::HomeMessage)
            {
                if (item.home_message().home_id() == APIid)
                {
                    bufferState->homeSelf = Proto2THUAI7::Protobuf2THUAI7Home(item.home_message());
                    bufferState->homes.push_back(bufferState->shipSelf);
                }
                logger->debug("Add Home!");
            }
    }
}

void Logic::LoadBufferCase(const protobuf::MessageOfObj& item)
{
    int32_t x, y, viewRange;
    if (isShip)
        x = bufferState->shipSelf->x, y = bufferState->shipSelf->y;
    else
        x = bufferState->homeSelf->x, y = bufferState->homeSelf->y;

    if (isShip && Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage)
    {
        if(playerTeam != item.ship_message().team())
        {
            if (AssistFunction::HaveView( x, y, item.ship_message().x(), item.ship_message().y(), bufferState->gameMap))
            {
                bufferState->enemyships.push_back(Proto2THUAI7::Protobuf2THUAI7Ship(item.ship_message()));
                logger->debug("Add Enemyship!");
            }
            return;
        }
    }

    switch (Proto2THUAI7::messageOfObjDict[item.message_of_obj_case()])
    {
        case THUAI7::MessageOfObj::BulletMessage:
            if (AssistFunction::HaveView(x, y, item.bullet_message().x(), item.bullet_message().y(), bufferState->gameMap))
            {
                bufferState->bullets.push_back(Proto2THUAI7::Protobuf2THUAI7Bullet(item.bullet_message()));
                logger->debug("Add Bullet!");
            }
            break;
        case THUAI7::MessageOfObj::FactoryMessage:
            if (AssistFunction::HaveView(x, y, item.factory_message().x(), item.factory_message().y(), bufferState->gameMap))
            {
                auto pos=std::make_pair(AssistFunction::GridToCell(item.factory_message().x()),AssistFunction::GridToCell(item.factory_message().y()));
                if (bufferState->mapInfo->factoryState.count(pos) == 0)
                {
                    bufferState->mapInfo->factoryState.emplace(pos, {item.factory_message().hp(),item.factory_message().team()});
                    logger->debug("Add Factory!");
                }
                else
                {
                    bufferState->mapInfo->factoryState[pos].first = item.factory_message().hp();
                    logger->debug("Update Factory!");
                }
            }
            break;
        case THUAI7::MessageOfObj::CommunityMessage:
            if (AssistFunction::HaveView(x, y, item.community_message().x(), item.community_message().y(), bufferState->gameMap))
            {
                auto pos=std::make_pair(AssistFunction::GridToCell(item.community_message().x()),AssistFunction::GridToCell(item.community_message().y()));
                if (bufferState->mapInfo->communityState.count(pos) == 0)
                {
                    bufferState->mapInfo->communityState.emplace(pos, {item.community_message().hp(),item.community_message().team()});
                    logger->debug("Add Community!");
                }
                else
                {
                    bufferState->mapInfo->communityState[pos].first = item.community_message().hp();
                    logger->debug("Update Community!");
                }
            }
            break;
        case THUAI7::MessageOfObj::FortressMessage:
            if (AssistFunction::HaveView(x, y, item.fortress_message().x(), item.fortress_message().y(), bufferState->gameMap))
            {
                auto pos=std::make_pair(AssistFunction::GridToCell(item.fortress_message().x()),AssistFunction::GridToCell(item.fortress_message().y()));
                if (bufferState->mapInfo->fortressState.count(pos) == 0)
                {
                    bufferState->mapInfo->fortressState.emplace(pos, {item.fortress_message().hp(),item.fortress_message().team()});
                    logger->debug("Add Fortress!");
                }
                else
                {
                    bufferState->mapInfo->fortressState[pos].first = item.fortress_message().hp();
                    logger->debug("Update Fortress!");
                }
            }
            break;
        case THUAI7::MessageOfObj::WormholeMessage:
            if (AssistFunction::HaveView(x, y, item.wormhole_message().x(), item.wormhole_message().y(), bufferState->gameMap))
            {
                auto pos=std::make_pair(AssistFunction::GridToCell(item.wormhole_message().x()),AssistFunction::GridToCell(item.wormhole_message().y()));
                if (bufferState->mapInfo->wormholeState.count(pos) == 0)
                {
                    bufferState->mapInfo->wormholeState.emplace(pos, item.wormhole_message().hp());
                    logger->debug("Add Wormhole!");
                }
                else
                {
                    bufferState->mapInfo->wormholeState[pos] = item.wormhole_message().hp();
                    logger->debug("Update Wormhole!");
                }
            }
            break;
        case THUAI7::MessageOfObj::ResourceMessage:
            if (AssistFunction::HaveView(x, y, item.resource_message().x(), item.resource_message().y(), bufferState->gameMap))
            {
                auto pos=std::make_pair(AssistFunction::GridToCell(item.resource_message().x()),AssistFunction::GridToCell(item.resource_message().y()));
                if (bufferState->mapInfo->resourceState.count(pos) == 0)
                {
                    bufferState->mapInfo->resourceState.emplace(pos, item.resource_message().progress());
                    logger->debug("Add Resouce!");
                }
                else
                {
                    bufferState->mapInfo->resourceState[pos].first = item.resouce_message().progress();
                    logger->debug("Update Resource!");
                }
            }
            break;
        case THUAI7::MessageOfObj::NewsMessage:
            {
                auto news = item.news_message();
                if (news.to_id() == APIid)
                {
                    if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::TextMessage)
                    {
                        messageQueue.emplace(std::make_pair(news.from_id(), news.text_message()));
                        logger->debug("Add News!");
                    }
                    else if (Proto2THUAI7::newsTypeDict[news.news_case()] == THUAI7::NewsType::BinaryMessage)
                    {
                        messageQueue.emplace(std::make_pair(news.from_id(), news.binary_message()));
                        logger->debug("Add Binary News!");
                    }
                    else
                        logger->error("Unknown NewsType!");
                }
                break;
            }
        case THUAI6::MessageOfObj::NullMessageOfObj:
        default:
            break;
    }
}

void Logic::LoadBuffer(const protobuf::MessageToClient& message)
{
    // 将消息读入到buffer中
    {
        std::lock_guard<std::mutex> lock(mtxBuffer);

        // 清空原有信息
        bufferState->ships.clear();
        bufferState->enemyships.clear();
        bufferState->homes.clear();
        bufferState->bullets.clear();
        bufferState->guids.clear();

        logger->debug("Buffer cleared!");
        // 读取新的信息
        for (const auto& obj : message.obj_message())
            if (Proto2THUAI7::messageOfObjDict[obj.message_of_obj_case()] == THUAI7::MessageOfObj::ShipMessage)
                bufferState->guids.push_back(obj.ship_message().guid());
            else if (Proto2THUAI7::messageOfObjDict[obj.message_of_obj_case()] == THUAI7::MessageOfObj::HomeMessage)
                bufferState->guids.push_back(obj.home_message().guid());
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
    return pComm->TryConnection(APIid);
}

bool Logic::HaveView(int32_t gridX, int32_t gridY, int32_t selfX, int32_t selfY) const
{
    std::unique_lock<std::mutex> lock(mtxState);
    return AssistFunction::HaveView(selfX, selfY, gridX, gridY, currentState->gameMap);
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
    if(isShip)
        logger->info("ship ID: {}", APIid);
    else
        logger->info("home ID: {}", APIid);
    logger->info("player team: {}", THUAI7::playerTeamDict[playerTeam]);
    logger->info("****************************");

    // 建立与服务器之间通信的组件
    pComm = std::make_unique<Communication>(IP, port);

    // 构造timer
    if (playerTeam==THUAI7::PlayerTeam::Up)
    {
        if(isShip)
        {
            if (!file && !print)
                timer = std::make_unique<ShipAPI>(*this);
            else
                timer = std::make_unique<ShipDebugAPI>(*this, file, print, warnOnly, APIid);
        }
        else
        {
            if (!file && !print)
                timer = std::make_unique<HomeAPI>(*this);
            else
                timer = std::make_unique<HomeDebugAPI>(*this, file, print, warnOnly, APIid);
        }
    }
    else if (playerTeam==THUAI7::PlayerTeam::Down)
    {
        if(isShip)
        {
            if (!file && !print)
                timer = std::make_unique<ShipAPI>(*this);
            else
                timer = std::make_unique<ShipDebugAPI>(*this, file, print, warnOnly, APIid);
        }
        else
        {
            if (!file && !print)
                timer = std::make_unique<HomeAPI>(*this);
            else
                timer = std::make_unique<HomeDebugAPI>(*this, file, print, warnOnly, APIid);
        }
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
            auto ai = createAI(APIid);

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
