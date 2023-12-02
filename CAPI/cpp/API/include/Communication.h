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
    bool TryConnection(int64_t playerID);
    protobuf::MessageToClient GetMessage2Client();
    void AddPlayer(int64_t playerID, THUAI7::PlayerType playerType, THUAI7::ShipType shipType);
    bool EndAllAction(int64_t playerID);
    // 船
    bool Move(int64_t time, double angle, int64_t playerID);
    bool Recover(int64_t playerID);
    bool Produce(int64_t playerID, int32_t x, int32_t y);
    bool Rebuild(int64_t playerID, int32_t x, int32_t y);
    bool Construct(int64_t playerID, THUAI7::BuildingType buidingType, int32_t x, int32_t y);
    bool Attack(double angle, int64_t playerID);

    // 大本营
    bool InstallModule(THUAI7::Module module, int64_t playerID);
    bool BuildShip(THUAI7::ShipType shipType, int32_t x, int32_t y);
    bool Recycle(int64_t playerID);

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
