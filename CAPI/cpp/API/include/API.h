#pragma once
#ifndef API_H
#define API_H

#ifdef _MSC_VER
#pragma warning(disable : 4996)
#endif

#include "Message2Server.pb.h"
#include "Message2Clients.pb.h"
#include "MessageType.pb.h"
#include "Services.grpc.pb.h"
#include "Services.pb.h"
#include <future>
#include <iostream>
#include <vector>
#include <optional>

#include <spdlog/spdlog.h>
#include <spdlog/sinks/basic_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>

#include "structures.h"

#undef GetMessage
#undef SendMessage
#undef PeekMessage

const constexpr int32_t numOfGridPerCell = 1000;

class IAI;

class ILogic
{
    // API中依赖Logic的部分

public:
    // 获取服务器发来的消息
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Sweeper> SweeperGetSelfInfo() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Team> TeamGetSelfInfo() const = 0;
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const = 0;
    [[nodiscard]] virtual std::vector<int64_t> GetPlayerGUIDs() const = 0;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetBridgeHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetGarbageState(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetHomeHp() const = 0;
    [[nodiscard]] virtual int32_t GetEnergy() const = 0;
    [[nodiscard]] virtual int32_t GetScore() const = 0;

    // 供IAPI使用的操作相关的公共部分
    virtual bool Send(int64_t toPlayerID, std::string message, bool binary) = 0;
    virtual bool HaveMessage() = 0;
    virtual std::pair<int64_t, std::string> GetMessage() = 0;
    virtual bool WaitThread() = 0;
    virtual int32_t GetCounter() const = 0;
    virtual bool EndAllAction() = 0;

    // ISweeperAPI使用的部分
    virtual bool Move(int64_t time, double angle) = 0;
    virtual bool Recover() = 0;
    virtual bool Produce() = 0;
    virtual bool Rebuild(THUAI7::ConstructionType constructionType) = 0;
    virtual bool Construct(THUAI7::ConstructionType constructionType) = 0;
    virtual bool Attack(double angle) = 0;
    [[nodiscard]] virtual bool HaveView(int32_t gridX, int32_t gridY, int32_t selfX, int32_t selfY, int32_t viewRange) const = 0;

    // Team使用的部分
    virtual bool Recycle(int64_t playerID) = 0;
    virtual bool InstallModule(int64_t playerID, THUAI7::ModuleType moduleType) = 0;
    virtual bool BuildSweeper(THUAI7::SweeperType SweeperType) = 0;
};

class IAPI
{
public:
    // 选手可执行的操作，应当保证所有函数的返回值都应当为std::future，例如下面的移动函数：
    // 指挥本角色进行移动，`timeInMilliseconds` 为移动时间，单位为毫秒；`angleInRadian` 表示移动的方向，单位是弧度，使用极坐标——竖直向下方向为 x 轴，水平向右方向为 y 轴
    // 发送信息、接受信息，注意收消息时无消息则返回nullopt
    virtual std::future<bool> SendTextMessage(int64_t toPlayerID, std::string) = 0;
    virtual std::future<bool> SendBinaryMessage(int64_t toPlayerID, std::string) = 0;
    [[nodiscard]] virtual bool HaveMessage() = 0;
    [[nodiscard]] virtual std::pair<int64_t, std::string> GetMessage() = 0;

    // 获取游戏目前所进行的帧数
    [[nodiscard]] virtual int32_t GetFrameCount() const = 0;
    // 等待下一帧
    virtual bool Wait() = 0;
    virtual std::future<bool> EndAllAction() = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const = 0;
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const = 0;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetBridgeHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetGarbageState(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetHomeHp() const = 0;
    [[nodiscard]] virtual int32_t GetEnergy() const = 0;
    [[nodiscard]] virtual int32_t GetScore() const = 0;
    [[nodiscard]] virtual std::vector<int64_t> GetPlayerGUIDs() const = 0;

    /*****选手可能用的辅助函数*****/

    // 获取指定格子中心的坐标
    [[nodiscard]] static inline int32_t CellToGrid(int32_t cell) noexcept
    {
        return cell * numOfGridPerCell + numOfGridPerCell / 2;
    }

    // 获取指定坐标点所位于的格子的 X 序号
    [[nodiscard]] static inline int32_t GridToCell(int32_t grid) noexcept
    {
        return grid / numOfGridPerCell;
    }

    // 用于DEBUG的输出函数，选手仅在开启Debug模式的情况下可以使用

    virtual void Print(std::string str) const = 0;
    virtual void PrintSweeper() const = 0;
    virtual void PrintTeam() const = 0;
    virtual void PrintSelfInfo() const = 0;
};

class ISweeperAPI : public IAPI
{
public:
    virtual std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) = 0;
    // 向特定方向移动
    virtual std::future<bool> MoveRight(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveUp(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveLeft(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveDown(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> Attack(double angleInRadian) = 0;
    virtual std::future<bool> Recover() = 0;
    virtual std::future<bool> Produce() = 0;
    virtual std::future<bool> Rebuild(THUAI7::ConstructionType constructionType) = 0;
    virtual std::future<bool> Construct(THUAI7::ConstructionType constructionType) = 0;
    virtual std::shared_ptr<const THUAI7::Sweeper> GetSelfInfo() const = 0;
    virtual bool HaveView(int32_t gridX, int32_t gridY) const = 0;
};

class ITeamAPI : public IAPI
{
public:
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Team> GetSelfInfo() const = 0;
    virtual std::future<bool> InstallModule(int64_t playerID, THUAI7::ModuleType moduletype) = 0;
    virtual std::future<bool> Recycle(int64_t playerID) = 0;
    virtual std::future<bool> BuildSweeper(THUAI7::SweeperType SweeperType) = 0;
};

class IGameTimer
{
public:
    virtual ~IGameTimer() = default;
    virtual void StartTimer() = 0;
    virtual void EndTimer() = 0;
    virtual void Play(IAI& ai) = 0;
};

class SweeperAPI : public ISweeperAPI, public IGameTimer
{
public:
    SweeperAPI(ILogic& logic) :
        logic(logic)
    {
    }
    void StartTimer() override
    {
    }
    void EndTimer() override
    {
    }
    void Play(IAI& ai) override;

    std::future<bool> SendTextMessage(int64_t, std::string) override;
    std::future<bool> SendBinaryMessage(int64_t, std::string) override;
    [[nodiscard]] bool HaveMessage() override;
    [[nodiscard]] std::pair<int64_t, std::string> GetMessage() override;

    [[nodiscard]] int32_t GetFrameCount() const override;
    bool Wait() override;
    std::future<bool> EndAllAction() override;

    std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) override;
    std::future<bool> MoveRight(int64_t timeInMilliseconds) override;
    std::future<bool> MoveUp(int64_t timeInMilliseconds) override;
    std::future<bool> MoveLeft(int64_t timeInMilliseconds) override;
    std::future<bool> MoveDown(int64_t timeInMilliseconds) override;
    std::future<bool> Attack(double angleInRadian) override;
    std::future<bool> Recover() override;
    std::future<bool> Produce() override;
    std::future<bool> Rebuild(THUAI7::ConstructionType constructionType) override;
    std::future<bool> Construct(THUAI7::ConstructionType constructionType) override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBridgeHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetGarbageState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetPlayerGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Sweeper> GetSelfInfo() const override;
    [[nodiscard]] bool HaveView(int32_t gridX, int32_t gridY) const override;
    [[nodiscard]] int32_t GetEnergy() const override;
    [[nodiscard]] int32_t GetScore() const override;
    void Print(std::string str) const
    {
    }
    void PrintSweeper() const
    {
    }
    void PrintTeam() const
    {
    }
    void PrintSelfInfo() const
    {
    }

private:
    ILogic& logic;
};

class TeamAPI : public ITeamAPI, public IGameTimer
{
public:
    TeamAPI(ILogic& logic) :
        logic(logic)
    {
    }
    void StartTimer() override
    {
    }
    void EndTimer() override
    {
    }
    void Play(IAI& ai) override;

    std::future<bool> SendTextMessage(int64_t, std::string) override;
    std::future<bool> SendBinaryMessage(int64_t, std::string) override;
    [[nodiscard]] bool HaveMessage() override;
    [[nodiscard]] std::pair<int64_t, std::string> GetMessage() override;

    [[nodiscard]] int32_t GetFrameCount() const override;
    bool Wait() override;
    std::future<bool> EndAllAction() override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBridgeHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetGarbageState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetPlayerGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Team> GetSelfInfo() const override;

    [[nodiscard]] int32_t GetScore() const override;
    [[nodiscard]] int32_t GetEnergy() const override;
    std::future<bool> InstallModule(int64_t playerID, THUAI7::ModuleType moduleType) override;
    std::future<bool> Recycle(int64_t playerID) override;
    std::future<bool> BuildSweeper(THUAI7::SweeperType SweeperType) override;
    void Print(std::string str) const
    {
    }
    void PrintSweeper() const
    {
    }
    void PrintTeam() const
    {
    }
    void PrintSelfInfo() const
    {
    }

private:
    ILogic& logic;
};

class SweeperDebugAPI : public ISweeperAPI, public IGameTimer
{
public:
    SweeperDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t SweeperID);
    void StartTimer() override;
    void EndTimer() override;
    void Play(IAI& ai) override;
    std::future<bool> SendTextMessage(int64_t, std::string) override;
    std::future<bool> SendBinaryMessage(int64_t, std::string) override;
    [[nodiscard]] bool HaveMessage() override;
    [[nodiscard]] std::pair<int64_t, std::string> GetMessage() override;
    bool Wait() override;
    [[nodiscard]] int32_t GetFrameCount() const override;
    std::future<bool> EndAllAction() override;

    std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) override;
    std::future<bool> MoveRight(int64_t timeInMilliseconds) override;
    std::future<bool> MoveUp(int64_t timeInMilliseconds) override;
    std::future<bool> MoveLeft(int64_t timeInMilliseconds) override;
    std::future<bool> MoveDown(int64_t timeInMilliseconds) override;
    std::future<bool> Attack(double angleInRadian) override;
    std::future<bool> Recover() override;
    std::future<bool> Produce() override;
    std::future<bool> Rebuild(THUAI7::ConstructionType constructionType) override;
    std::future<bool> Construct(THUAI7::ConstructionType constructionType) override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBridgeHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetGarbageState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetPlayerGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Sweeper> GetSelfInfo() const override;
    [[nodiscard]] bool HaveView(int32_t gridX, int32_t gridY) const override;
    [[nodiscard]] int32_t GetEnergy() const override;
    [[nodiscard]] int32_t GetScore() const override;

    void Print(std::string str) const override;
    void PrintSweeper() const override;
    void PrintSelfInfo() const override;
    void PrintTeam() const
    {
    }

private:
    std::chrono::system_clock::time_point startPoint;
    std::unique_ptr<spdlog::logger> logger;
    ILogic& logic;
};

class TeamDebugAPI : public ITeamAPI, public IGameTimer
{
public:
    TeamDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t TeamID);
    void StartTimer() override;
    void EndTimer() override;
    void Play(IAI& ai) override;

    std::future<bool> SendTextMessage(int64_t, std::string) override;
    std::future<bool> SendBinaryMessage(int64_t, std::string) override;
    [[nodiscard]] bool HaveMessage() override;
    [[nodiscard]] std::pair<int64_t, std::string> GetMessage() override;

    [[nodiscard]] int32_t GetFrameCount() const override;
    bool Wait() override;
    std::future<bool> EndAllAction() override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetSweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Sweeper>> GetEnemySweepers() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetConstructionHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBridgeHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetGarbageState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetPlayerGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Team> GetSelfInfo() const override;

    [[nodiscard]] int32_t GetScore() const override;
    [[nodiscard]] int32_t GetEnergy() const override;
    std::future<bool> InstallModule(int64_t playerID, THUAI7::ModuleType moduleType) override;
    std::future<bool> Recycle(int64_t playerID) override;
    std::future<bool> BuildSweeper(THUAI7::SweeperType SweeperType) override;
    void Print(std::string str) const override;
    void PrintSelfInfo() const override;
    // TODO
    void PrintTeam() const
    {
    }
    void PrintSweeper() const
    {
    }

private:
    std::chrono::system_clock::time_point startPoint;
    std::unique_ptr<spdlog::logger> logger;
    ILogic& logic;
};

#endif