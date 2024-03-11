#ifndef COMMUNICATION_H
#define COMMUNICATION_H

#include "Message2Server.pb.h"
#include "Message2Clients.pb.h"
#include "MessageType.pb.h"
#include "Services.grpc.pb.h"
#include "Services.pb.h"
#include <grpcpp/grpcpp.h>
#include "structures.h"
#include <thread>
#include <mutex>
#include <condition_variable>
#include <queue>
#include <atomic>

#undef GetMessage
#undef SendMessage
#undef PeekMessage

class Logic;

class Communication
{
public:
    Communication(std::string sIP, std::string sPort);
    ~Communication()
    {
    }
    bool TryConnection(int64_t playerID, int64_t teamID);
    protobuf::MessageToClient GetMessage2Client();
    void AddPlayer(int64_t playerID, int64_t teamID, THUAI7::SweeperType SweeperType, int32_t x, int32_t y);
    bool EndAllAction(int64_t playerID, int64_t teamID);
    // Sweeper
    bool Move(int64_t playerID, int64_t teamID, int64_t time, double angle);
    bool Recover(int64_t playerID, int64_t recover, int64_t teamID);
    bool Produce(int64_t playerID, int64_t teamID);
    bool Rebuild(int64_t playerID, int64_t teamID, THUAI7::ConstructionType constructionType);
    bool Construct(int64_t playerID, int64_t teamID, THUAI7::ConstructionType constructionType);
    bool Attack(int64_t playerID, int64_t teamID, double angle);
    bool Send(int64_t playerID, int64_t toPlayerID, int64_t teamID, std::string message, bool binary);
    // Team
    bool InstallModule(int64_t playerID, int64_t teamID, THUAI7::ModuleType moduleType);
    bool BuildSweeper(int64_t teamID, THUAI7::SweeperType SweeperType, int32_t x, int32_t y);
    bool Recycle(int64_t playerID, int64_t teamID);

private:
    std::unique_ptr<protobuf::AvailableService::Stub> THUAI7Stub;
    bool haveNewMessage = false;
    protobuf::MessageToClient message2Client;
    std::mutex mtxMessage;
    std::mutex mtxLimit;
    int32_t counter;
    int32_t counterMove;
    static constexpr const int32_t limit = 50;
    static constexpr const int32_t moveLimit = 10;
    std::condition_variable cvMessage;
};

#endif
