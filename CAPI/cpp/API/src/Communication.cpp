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

bool Communication::Move(int32_t playerID, int32_t teamID, int64_t time, double angle)
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
    auto request = THUAI72Proto::THUAI72ProtobufMoveMsg(playerID, teamID, time, angle);
    auto status = THUAI7Stub->Move(&context, request, &moveResult);
    if (status.ok())
    {
        return moveResult.act_success();
    }
    else
        return false;
}

bool Communication::Send(int32_t playerID, int32_t toPlayerID, int32_t teamID, std::string message, bool binary)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes sendMessageResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufSendMsg(playerID, toPlayerID, teamID, std::move(message), binary);
    auto status = THUAI7Stub->Send(&context, request, &sendMessageResult);
    if (status.ok())
        return sendMessageResult.act_success();
    else
        return false;
}

bool Communication::EndAllAction(int32_t playerID, int32_t teamID)
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
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->EndAllAction(&context, request, &endAllActionResult);
    if (status.ok())
        return endAllActionResult.act_success();
    else
        return false;
}

bool Communication::Recover(int32_t playerID, int64_t recover, int32_t teamID)
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
    auto request = THUAI72Proto::THUAI72ProtobufRecoverMsg(playerID, recover, teamID);
    auto status = THUAI7Stub->Recover(&context, request, &recoverResult);
    if (status.ok())
        return recoverResult.act_success();
    else
        return false;
}

bool Communication::Produce(int32_t playerID, int32_t teamID)
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
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->Produce(&context, request, &produceResult);
    if (status.ok())
        return produceResult.act_success();
    else
        return false;
}

bool Communication::RepairWormhole(int32_t playerID, int32_t teamID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes repairWormholeResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->RepairWormhole(&context, request, &repairWormholeResult);
    if (status.ok())
        return repairWormholeResult.act_success();
    else
        return false;
}

bool Communication::RepairHome(int32_t playerID, int32_t teamID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit || counterMove >= moveLimit)
            return false;
        counter++;
        counterMove++;
    }
    protobuf::BoolRes repairHomeResult;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->RepairHome(&context, request, &repairHomeResult);
    if (status.ok())
        return repairHomeResult.act_success();
    else
        return false;
}

bool Communication::Rebuild(int32_t playerID, int32_t teamID, THUAI7::ConstructionType constructionType)
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
    auto request = THUAI72Proto::THUAI72ProtobufConstructMsg(playerID, teamID, constructionType);
    auto status = THUAI7Stub->Rebuild(&context, request, &rebuildResult);
    if (status.ok())
        return rebuildResult.act_success();
    else
        return false;
}

bool Communication::Construct(int32_t playerID, int32_t teamID, THUAI7::ConstructionType constructionType)
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
    auto request = THUAI72Proto::THUAI72ProtobufConstructMsg(playerID, teamID, constructionType);
    auto status = THUAI7Stub->Construct(&context, request, &constructResult);
    if (status.ok())
        return constructResult.act_success();
    else
        return false;
}

bool Communication::InstallModule(int32_t playerID, int32_t teamID, THUAI7::ModuleType moduleType)
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
    auto request = THUAI72Proto::THUAI72ProtobufInstallMsg(playerID, teamID, moduleType);
    auto status = THUAI7Stub->InstallModule(&context, request, &installModuleResult);
    if (status.ok())
        return installModuleResult.act_success();
    else
        return false;
}

bool Communication::Attack(int32_t playerID, int32_t teamID, double angle)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes attackResult;
    ClientContext context;
    auto request = THUAI72Proto::THUI72ProtobufAttackMsg(playerID, teamID, angle);
    auto status = THUAI7Stub->Attack(&context, request, &attackResult);
    if (status.ok())
        return attackResult.act_success();
    else
        return false;
}

bool Communication::BuildShip(int32_t teamID, THUAI7::ShipType ShipType, int32_t birthIndex)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes reply;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufBuildShipMsg(teamID, ShipType, birthIndex);
    auto status = THUAI7Stub->BuildShip(&context, request, &reply);
    if (status.ok())
        return reply.act_success();
    else
        return false;
}

bool Communication::Recycle(int32_t playerID, int32_t teamID)
{
    {
        std::lock_guard<std::mutex> lock(mtxLimit);
        if (counter >= limit)
            return false;
        counter++;
    }
    protobuf::BoolRes reply;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->Recycle(&context, request, &reply);
    if (status.ok())
        return true;
    else
        return false;
}

bool Communication::TryConnection(int32_t playerID, int32_t teamID)
{
    protobuf::BoolRes reply;
    ClientContext context;
    auto request = THUAI72Proto::THUAI72ProtobufIDMsg(playerID, teamID);
    auto status = THUAI7Stub->TryConnection(&context, request, &reply);
    if (status.ok())
        return true;
    else
        return false;
}

void Communication::AddPlayer(int32_t playerID, int32_t teamID, THUAI7::ShipType ShipType)
{
    auto tMessage = [=]()
    {
        protobuf::PlayerMsg playerMsg = THUAI72Proto::THUAI72ProtobufPlayerMsg(playerID, teamID, ShipType);
        grpc::ClientContext context;
        auto MessageReader = THUAI7Stub->AddPlayer(&context, playerMsg);

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

protobuf::MessageToClient Communication::GetMessage2Client()
{
    std::unique_lock<std::mutex> lock(mtxMessage);
    cvMessage.wait(lock, [this]()
                   { return haveNewMessage; });
    haveNewMessage = false;
    return message2Client;
}
