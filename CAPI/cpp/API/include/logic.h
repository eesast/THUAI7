#pragma once

#ifndef LOGIC_H
#define LOGIC_H

#ifdef _MSC_VER
#pragma warning(disable : 4996)
#endif

#include <iostream>
#include <vector>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <atomic>
#include <queue>

#include <spdlog/spdlog.h>
#include <spdlog/sinks/basic_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>

#include "Message2Server.pb.h"
#include "Message2Clients.pb.h"
#include "MessageType.pb.h"
#include "Services.grpc.pb.h"
#include "Services.pb.h"
#include "API.h"
#include "AI.h"
#include "structures.h"
#include "state.h"
#include "Communication.h"
#include "ConcurrentQueue.hpp"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

// 封装了通信组件和对AI对象进行操作
class Logic : public ILogic
{
private:
    // 日志组件
    std::unique_ptr<spdlog::logger> logger;

    // 通信组件
    std::unique_ptr<Communication> pComm;

    // ID
    THUAI7::PlayerType playerType;
    int64_t player_id;
    THUAI7::ShipType shipType;

    std::unique_ptr<IGameTimer> timer;
    std::thread tAI;  // 用于运行AI的线程

    mutable std::mutex mtxAI;
    mutable std::mutex mtxState;
    mutable std::mutex mtxBuffer;

    std::condition_variable cvBuffer;
    std::condition_variable cvAI;

    // 信息队列
    ConcurrentQueue<std::pair<int64_t, std::string>> messageQueue;

    // 存储状态，分别是现在的状态和缓冲区的状态。
    State state[2];
    State* currentState;
    State* bufferState;

    // 保存缓冲区数
    int32_t counterState = 0;
    int32_t counterBuffer = 0;

    THUAI7::GameState gameState = THUAI7::GameState::NullGameState;

    // 是否应该执行player()
    std::atomic_bool AILoop = true;

    // buffer是否更新完毕
    bool bufferUpdated = true;

    // 是否应当启动AI
    bool AIStart = false;

    // asynchronous = true 时控制内容更新的变量
    std::atomic_bool freshed = false;

    // 提供给API使用的函数

    [[nodiscard]]  std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const override;
    [[nodiscard]]  std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShip() const override;
    [[nodiscard]]  std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]]  std::shared_ptr<const THUAI7::Ship> GetSelfInfo() const override;
    [[nodiscard]]  std::shared_ptr<const THUAI7::Home> GetSelfInfo() const override;
    [[nodiscard]]  std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]]  THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]]  int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const override; 
    [[nodiscard]]  int32_t GetWormHp(int32_t cellX, int32_t cellY) const override; 
    [[nodiscard]]  int32_t GetResourceState(int32_t cellX, int32_t cellY) const override; 
    [[nodiscard]]  std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]]  int32_t GetEconomy() const override;

    // 供IAPI使用的操作相关的部分
    bool Move(int64_t time, double angle) override;
    bool SendMessage(int64_t toID, std::string message, bool binary) override;
    bool HaveMessage() override;
    std::pair<int64_t, std::string> GetMessage() override;


    int32_t GetCounter() const override;

    // IShipAPI使用的部分
    bool Recover() override;
    bool Recycle() override;
    bool Produce(int32_t x, int32_t y) override;
    bool Rebuild(int32_t cellX, int32_t cellY) override;
    bool InstallModule(THUAI7::Module module) override;
    bool EndAllAction() override;
    bool Attack(double angle) override;
    std::vector<int64_t> GetShipGUIDs() const override;
    [[nodiscard]]  bool HaveView(int32_t gridX, int32_t gridY, int32_t selfX, int32_t selfY, int32_t viewRange) const override;

    bool WaitThread() override;
    bool TryConnection();
    void ProcessMessage();

    // 将信息加载到buffer
    void LoadBufferSelf(const protobuf::MessageToClient& message);
    void LoadBufferCase(const protobuf::MessageOfObj& item);
    void LoadBuffer(const protobuf::MessageToClient& message);

    // 解锁AI线程
    void UnBlockAI();

    // 更新状态
    void Update() noexcept;

    // 等待
    void Wait() noexcept;

public:
    // 构造函数还需要传更多参数，有待补充
    Logic( int64_t playerID,THUAI7::PlayerType player_type, THUAI7::ShipType shipType);

    ~Logic()
    {
    }

    // Main函数同上
    void Main(CreateAIFunc createAI, std::string IP, std::string port, bool file, bool print, bool warnOnly);
};

#endif
