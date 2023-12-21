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
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShips() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Ship> ShipGetSelfInfo() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Home> HomeGetSelfInfo() const = 0;

    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const = 0;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const = 0;

    [[nodiscard]] virtual int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetWormHp(int32_t cellX, int32_t cellY) = 0;
    [[nodiscard]] virtual int32_t GetResourceState(int32_t cellX, int32_t cellY) = 0;
    [[nodiscard]] virtual int32_t GetHomeHp() = 0;
    [[nodiscard]] virtual int32_t GetEconomy() const = 0;

    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const = 0;
    // 供IAPI使用的操作相关的部分
    virtual bool SendMessage(int64_t toID, std::string message, bool binary) = 0;
    virtual bool HaveMessage() = 0;
    virtual std::pair<int64_t, std::string> GetMessage() = 0;
    virtual bool WaitThread() = 0;
    virtual int32_t GetCounter() const = 0;

    // IShipAPI使用的部分
    virtual bool Move(int64_t time, double angle) = 0;
    virtual bool Recover() = 0;
    virtual bool Produce(int32_t cellX, int32_t cellY) = 0;
    virtual bool ReBuild(int32_t cellX, int32_t cellY) = 0;
    virtual bool Construct(int32_t building_id, int32_t cellX, int32_t cellY) = 0;
    virtual bool Attack(double angle) = 0;
    virtual bool EndAllAction() = 0;
    [[nodiscard]] virtual std::shared_ptr<THUAI7::Ship> GetSelfInfo() const = 0;

    // 大本营使用的部分
    virtual bool Recycle(std::shared_ptr<THUAI7::Ship> ship) = 0;
    virtual bool InstallModule(const std::shared_ptr<THUAI7::Ship> ship, const THUAI7::ModuleType type, const THUAI7::ModuleLevel level) = 0;
    virtual bool BuildShip(THUAI7::ShipType shipType, int32_t player_id, int32_t cellX, int32_t cellY) = 0;
};

class IAPI
{
public:
    // 发送信息、接受信息，注意收消息时无消息则返回nullopt
    virtual std::future<bool> SendTextMessage(int64_t, std::string) = 0;
    virtual std::future<bool> SendBinaryMessage(int64_t, std::string) = 0;
    [[nodiscard]] virtual bool HaveMessage() = 0;
    [[nodiscard]] virtual std::pair<int64_t, std::string> GetMessage() = 0;

    // 获取游戏目前所进行的帧数
    [[nodiscard]] virtual int32_t GetFrameCount() const = 0;
    // 等待下一帧
    virtual bool Wait() = 0;
    virtual std::future<bool> EndAllAction() = 0;

    // 获取视野内可见的信息
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const = 0;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShips() const = 0;

    // 获取视野内可见的子弹信息
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const = 0;
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const = 0;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const = 0;
    [[nodiscard]] virtual int32_t GetWormHp(int32_t cellX, int32_t cellY) = 0;
    [[nodiscard]] virtual int32_t GetResourceState(int32_t cellX, int32_t cellY) = 0;
    [[nodiscard]] virtual int32_t GetHomeHp() = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const = 0;

    // 获取所有船的GUID
    [[nodiscard]] virtual std::vector<int64_t> GetShipGUIDs() const = 0;

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

    [[nodiscard]] virtual bool HaveView(int32_t gridX, int32_t gridY) const = 0;

    // 用于DEBUG的输出函数，选手仅在开启Debug模式的情况下可以使用

    virtual void Print(std::string str) const = 0;
    virtual void PrintShip() const = 0;
    virtual void PrintHome() const = 0;
    virtual void PrintSelfInfo() const = 0;
};

class IShipAPI : public IAPI
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
    virtual std::future<bool> Produce(int32_t cellX, int32_t cellY) = 0;
    virtual std::future<bool> ReBuild(int32_t cellX, int32_t cellY) = 0;
    virtual std::future<bool> Construct(THUAI7::BuildingType buildingType, int32_t building_id, int32_t cellX, int32_t cellY) = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Ship> GetSelfInfo() const = 0;
};

class IHomeAPI : public IAPI
{
public:
    [[nodiscard]] virtual int64_t CurrentEconomic() const = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Home> GetSelfInfo() const = 0;
    virtual std::future<bool> InstallModule(std::shared_ptr<THUAI7::Ship> ship, const THUAI7::ModuleType type, const THUAI7::ModuleLevel level) = 0;
    virtual std::future<bool> Recycle(std::shared_ptr<THUAI7::Ship> ship) = 0;
    virtual std::future<bool> BuildShip(THUAI7::ShipType shipType, int64_t player_id, int32_t cellX, int32_t cellY) = 0;
};

class IGameTimer
{
public:
    virtual ~IGameTimer() = default;
    virtual void StartTimer() = 0;
    virtual void EndTimer() = 0;
    virtual void Play(IAI& ai) = 0;
};

class ShipAPI : public IShipAPI, public IGameTimer
{
    ShipAPI(ILogic& logic) :
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
    std::future<bool> Wait() override;
    std::future<bool> EndAllAction() override;

    std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) override;
    std::future<bool> MoveRight(int64_t timeInMilliseconds) override;
    std::future<bool> MoveUp(int64_t timeInMilliseconds) override;
    std::future<bool> MoveLeft(int64_t timeInMilliseconds) override;
    std::future<bool> MoveDown(int64_t timeInMilliseconds) override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShips() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetWormHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetResourceState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetShipGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Ship> GetSelfInfo() const override;
    [[nodiscard]] std::future<bool> HaveView(int32_t gridX, int32_t gridY) const override;

    void Print(std::string str) const override
    {
    }
    void PrintShip() const override
    {
    }
    void PrintSelfInfo() const override
    {
    }

private:
    ILogic& logic;
};

class HomeAPI : public IHomeAPI, public IGameTimer
{
    HomeAPI(ILogic& logic) :
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

    [[nodiscard]] int32_t GetFrameCount() const override;
    std::future<bool> Wait() override;
    std::future<bool> EndAllAction() override;

    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShips() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetWormHp(int32_t x, int32_t y) const override;
    [[nodiscard]] int32_t GetResourceState(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] int32_t GetHomeHp() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] std::vector<int64_t> GetShipGUIDs() const override;
    [[nodiscard]] std::shared_ptr<const THUAI7::Homep> GetSelfInfo() const override;
    [[nodiscard]] std::future<bool> HaveView(int32_t gridX, int32_t gridY) const override;

    [[nodiscard]] int32_t CurrentEconomic() const override;
    std::future<bool> InstallModule(std::shared_ptr<THUAI7::Ship> ship, const THUAI7::ModuleType type, const THUAI7::ModuleLevel level) override;
    std::future<bool> Recycle(std::shared_ptr<THUAI7::Ship> ship) override;
    std::future<bool> BuildShip(THUAI7::ShipType shipType, int64_t player_id, int32_t cellX, int32_t cellY) override;

    void Print(std::string str) const override
    {
    }
    void PrintHome() const override
    {
    }
    void PrintSelfInfo() const override
    {
    }
};

class ShipDebugAPI : public IShipAPI, public IGameTimer
{
    ShipDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t shipID);
    void StartTimer() override
    {
    }
    void EndTimer() override
    {
    }
    void Play(IAI& ai) override;
    [[nodiscard]] int32_t GetFrameCount() const override;

    std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) override;
    std::future<bool> MoveRight(int64_t timeInMilliseconds) override;
    std::future<bool> MoveUp(int64_t timeInMilliseconds) override;
    std::future<bool> MoveLeft(int64_t timeInMilliseconds) override;
    std::future<bool> MoveDown(int64_t timeInMilliseconds) override;
    virtual bool InstallModule(const THUAI7::ModuleType type, const THUAI7::ModuleLevel level) override;
    std::future<bool> Attack(double angleInRadian) override;
    std::future<bool> EndAllAction() override;

    std::future<bool> SendTextMessage(int64_t, std::string) override;
    std::future<bool> SendBinaryMessage(int64_t, std::string) override;
    [[nodiscard]] bool HaveMessage() override;
    [[nodiscard]] std::pair<int64_t, std::string> GetMessage() override;

    bool Wait() override;

    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const override;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShip() const override;
    [[nodiscard]] std::vector<std::shared_ptr<const THUAI7::Bullet>> GetBullets() const override;
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] virtual int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const override;

    [[nodiscard]] virtual std::shared_ptr<const THUAI7 ::GameInfo> GetGameInfo() const override;

    [[nodiscard]] virtual std::vector<int64_t> GetShipGUIDs() const override;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Ship> GetSelfInfo() const override;

    [[nodiscard]] bool HaveView(int32_t gridX, int32_t gridY) const override;

    void Print(std::string str) const override
    {
    }
    void PrintShip() const override
    {
    }
    void PrintSelfInfo() const override
    {
    }

private:
    std::chrono::system_clock::time_point startPoint;
    std::unique_ptr<spdlog::logger> logger;
    ILogic& logic;
};

class HomeDebugAPI : public IHomeAPI, public IGameTimer
{
    HomeDebugAPI(ILogic& logic, bool file, bool print, bool warnOnly, int64_t homeID);
    void StartTimer() override
    {
    }
    void EndTimer() override
    {
    }
    void Play(IAI& ai) override;
    [[nodiscard]] int32_t GetFrameCount() const override;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetShips() const override;
    [[nodiscard]] virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetEnemyShip() const override;
    [[nodiscard]] virtual int32_t GetEconomy() const override;
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const override;
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] virtual int32_t GetBuildingHp(int32_t cellX, int32_t cellY) const override;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const override;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::Home> GetSelfInfo() const override;
    [[nodiscard]] bool HaveView(int32_t gridX, int32_t gridY) const override;
    virtual bool SendMessage(int64_t toID, std::string message, bool binary) override;
    virtual bool HaveMessage() override;
    virtual std::pair<int64_t, std::string> GetMessage() override;
    bool Wait() override;
    void Print(std::string str) const override
    {
    }
    void PrintSelfInfo() const override
    {
    }

private:
    std::chrono::system_clock::time_point startPoint;
    std::unique_ptr<spdlog::logger> logger;
    ILogic& logic;
};

#endif
