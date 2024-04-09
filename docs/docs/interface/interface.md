# 接口一览

import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

<Tabs>
<TabItem value="cpp" label="C++" default>

```cpp
    // 指挥本角色进行移动，`timeInMilliseconds` 为移动时间，单位为毫秒；`angleInRadian` 表示移动的方向，单位是弧度，使用极坐标——竖直向下方向为 x 轴，水平向右方向为 y 轴
    virtual std::future<bool> Move(int64_t timeInMilliseconds, double angleInRadian) = 0;

    // 向特定方向移动
    virtual std::future<bool> MoveRight(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveUp(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveLeft(int64_t timeInMilliseconds) = 0;
    virtual std::future<bool> MoveDown(int64_t timeInMilliseconds) = 0;

    // 基地
    virtual std::future<bool> InstallModule(std::shared_ptr<const THUAI7::Ship>,int32_t Module)`
    virtual std::vector<std::shared_ptr<const THUAI7::Ship>> GetShipInformation()`
    virtual std::future<bool> BuildShip(int32_t shipType)`
    virtual std::future<int> CurrentEconomic()`

    //舰船
    virtual std::future<bool> Recover()`
    virtual std::future<bool> Produce(int32_t cellX, int32_t cellY)`
    virtual std::future<bool> Rebuild(int32_t cellX, int32_t cellY)`
    virtual std::future<bool> Construct(int32_t buildingID，int32_t cellX, int32_t cellY)`

    //攻击
    virtual std::future<bool> Attack(double angleInRadian)`：

    // 信息查询、接收
    [[nodiscard]] virtual THUAI7::PlaceType GetPlaceType(int32_t cellX, int32_t cellY)`
    [[nodiscard]] virtual int32_t GetResourceProgress(int32_t cellX, int32_t cellY) const`
    [[nodiscard]] virtual int32_t GetBuildingHealthValue(int32_t cellX, int32_t cellY) const`
    [[nodiscard]] virtual std::vector<std::vector<THUAI7::PlaceType>> GetFullMap() const`

    // 其他
    virtual std::future<bool> Wait() = 0;
    [[nodiscard]] virtual std::shared_ptr<const THUAI7::GameInfo> GetGameInfo() const = 0;

    // 获取所有玩家的GUID
    [[nodiscard]] virtual std::vector<int64_t> GetPlayerGUIDs() const = 0;

    // 获取游戏目前所进行的帧数
    [[nodiscard]] virtual int GetFrameCount() const = 0;

    /*****选手可能用的辅助函数*****/

    // 获取指定格子中心的坐标
    [[nodiscard]] static inline int CellToGrid(int cell) noexcept
    {
        return cell * numOfGridPerCell + numOfGridPerCell / 2;
    }

    // 获取指定坐标点所位于的格子的 X 序号
    [[nodiscard]] static inline int GridToCell(int grid) noexcept
    {
        return grid / numOfGridPerCell;
    }

    [[nodiscard]] virtual bool HaveView(int gridX, int gridY) const = 0;

    // 用于DEBUG的输出函数，选手仅在开启Debug模式的情况下可以使用

    virtual void Print(std::string str) const = 0;
    virtual void PrintStudent() const = 0;
    virtual void PrintTricker() const = 0;
    virtual void PrintProp() const = 0;
    virtual void PrintSelfInfo() const = 0;
```

</TabItem>
<TabItem value="python" label="Python">

</TabItem>
</Tabs>