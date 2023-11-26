#include "Communication.h"
#include "utils.hpp"
#include "structures.h"
#include <thread>
#include <mutex>
#include <condition_variable>

#undef GetMessage
#undef SendMessage
#undef PeekMessage

using grpc::ClientContext;

Communication::Communication(std::string sIP, std::string sPort)
{
    std::string aim = sIP + ':' + sPort;
    auto channel = grpc::CreateChannel(aim, grpc::InsecureChannelCredentials());
    THUAI7Stub = protobuf::AvailableService::NewStub(channel);
}

bool Communication::Move(int64_t time, double angle, int64_t shipID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::MoveRes moveResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufMove(time, angle, shipID);
    auto status = THUAI6Stub->Move(&context, request, &moveResult);
    if (status.ok())
        return moveResult.act_success();
    else
        return false;
}

bool Communication::SendMessage(int64_t toID, std::string message, bool binary, int64_t shipID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes sendMessageResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufSend(std::move(message), toID, binary, playerID);
    auto status = THUAI7Stub->SendMessage(&context, request, &sendMessageResult);
    if (status.ok())
        return sendMessageResult.act_success();
    else
        return false;
}

bool Communication::EndAllAction(int64_t playerID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes endAllActionResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufID(playerID);
    auto status = THUAI7Stub->EndAllAction(&context, request, &endAllActionResult);
    if (status.ok())
        return endAllActionResult.act_success();
    else
        return false;
}

bool Communication::Recover(int64_t shipID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes recoverResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufID(shipID);
    auto status = THUAI7Stub->Recover(&context, request, &recoverResult);
    if (status.ok())
        return recoverResult.act_success();
    else
        return false;
}

bool Communication::Produce(int64_t shipID, int32_t x, int32_t y)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes produceResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufTarget(shipID, x, y);
    auto status = THUAI7Stub->Produce(&context, request, &produceResult);
    if (status.ok())
        return produceResult.act_success();
    else
        return false;
}

bool Communication::Rebuild(int64_t shipID, int32_t x, int32_t y)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes rebuildResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufTarget(shipID, x, y);
    auto status = THUAI7Stub->Rebuild(&context, request, &rebuildResult);
    if (status.ok())
        return rebuildResult.act_success();
    else
        return false;
}

bool Communication::Construct(int64_t buildingID, int32_t x, int32_t y)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes constructResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufTarget(buildingID, x, y);
    auto status = THUAI7Stub->Construct(&context, request, &constructResult);
    if (status.ok())
        return constructResult.act_success();
    else
        return false;
}

bool Communication::InstallModule(THUAI7::Module module, int64_t shipID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes installModuleResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufModule(module, shipID);
    auto status = THUAI7Stub->InstallModule(&context, request, &installModuleResult);
    if (status.ok())
        return installModuleResult.act_success();
    else
        return false;
}

bool Communication::Attack(double angle, int64_t shipID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes attackResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufAttack(angle, shipID);
    auto status = THUAI7Stub->Attack(&context, request, &attackResult);
    if (status.ok())
        return attackResult.act_success();
    else
        return false;
}

bool Communication::TryConnection(int64_t shipID)
{
    protobuf::BoolRes reply;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufID(shipID);
    auto status = THUAI7Stub->TryConnection(&context, request, &reply);
    if (status.ok())
        return true;
    else
        return false;
}

protobuf::MessageToClient Communication::GetMessage2Client()
{
    std::unique_lock<std::mutex> lock(mtxMessage);
    cvMessage.wait(lock, [this]()
                   { return haveNewMessage; });
    haveNewMessage = false;
    return message2Client;
}

void Communication::AddShip(int64_t shipID, THUAI7::ShipType shipType, THUAI7::PlayerTeam playerTeam)
{
    auto tMessage = [=]()
    {
        protobuf::shipMsg shipMsg = THUAI72Proto::THUAI72ProtobufShip(shipID, playerType, studentType, trickerType);
        grpc::ClientContext context;
        auto MessageReader = THUAI7Stub->AddShip(&context, playerMsg);

        protobuf::MessageToClient buffer2Client;
        counter = 0;
        counterMove = 0;

        while (MessageReader->Read(&buffer2Client))
        {
            {
                std::lock_guard<std::mutex> lock(mtxMessage);
                message2Client = std::move(buffer2Client);
                haveNewMessage = true;
                {
                    std::lock_guard<std::mutex> lock(mtxLimit);
                    counter = 0;
                    counterMove = 0;
                }
            }
            cvMessage.notify_one();
        }
    };
    std::thread(tMessage).detach();
}
