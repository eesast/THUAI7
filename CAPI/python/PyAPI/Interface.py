from typing import List, Union, Tuple
from concurrent.futures import Future
from abc import abstractmethod, ABCMeta
import PyAPI.structures as THUAI7

from abc import ABCMeta, abstractmethod
from typing import List, Tuple, Union


class ILogic(metaclass=ABCMeta):
    """`IAPI` 统一可用的接口"""

    @abstractmethod
    def GetShips(self) -> List[THUAI7.Ship]:
        pass

    @abstractmethod
    def GetEnemyShips(self) -> List[THUAI7.Ship]:
        pass

    @abstractmethod
    def GetBullets(self) -> List[THUAI7.Bullet]:
        pass

    @abstractmethod
    def GetSelfInfo(self) -> Union[THUAI7.Ship, THUAI7.Team]:
        pass

    @abstractmethod
    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        pass

    @abstractmethod
    def GetGameInfo(self) -> THUAI7.GameInfo:
        pass

    @abstractmethod
    def GetPlayerGUIDs(self) -> List[int]:
        pass

    @abstractmethod
    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        pass

    @abstractmethod
    def GetConstructionState(self, cellX: int, cellY: int) -> THUAI7.ConstructionState | None:
        pass

    @abstractmethod
    def GetWormholeHp(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetResourceState(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetHomeHp(self) -> int:
        pass

    @abstractmethod
    def GetEnergy(self) -> int:
        pass

    @abstractmethod
    def GetScore(self) -> int:
        pass

    @abstractmethod
    def SendMessage(self, toID: int, message: Union[str, bytes]) -> bool:
        pass

    @abstractmethod
    def HaveMessage(self) -> bool:
        pass

    @abstractmethod
    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        pass

    @abstractmethod
    def WaitThread(self) -> bool:
        pass

    @abstractmethod
    def GetCounter(self) -> int:
        pass

    @abstractmethod
    def EndAllAction(self) -> bool:
        pass

    @abstractmethod
    def Move(self, time: int, angle: float) -> bool:
        pass

    @abstractmethod
    def Recover(self, recover: int) -> bool:
        pass

    @abstractmethod
    def Produce(self) -> bool:
        pass

    @abstractmethod
    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> bool:
        pass

    @abstractmethod
    def Construct(self, constructionType: THUAI7.ConstructionType) -> bool:
        pass

    @abstractmethod
    def RepairHome(self) -> bool:
        pass

    @abstractmethod
    def RepairWormhole(self) -> bool:
        pass

    @abstractmethod
    def Attack(self, angle: float) -> bool:
        pass

    @abstractmethod
    def HaveView(
        self, gridX: int, gridY: int, selfX: int, selfY: int, viewRange: int
    ) -> bool:
        pass

    @abstractmethod
    def Recycle(self, playerID: int) -> bool:
        pass

    @abstractmethod
    def InstallModule(self, playerID: int, moduleType: THUAI7.ModuleType) -> bool:
        pass

    @abstractmethod
    def BuildShip(self, shipType: THUAI7.ShipType, birthIndex: int) -> bool:
        pass


class IAPI(metaclass=ABCMeta):
    @abstractmethod
    def SendMessage(self, toPlayerID: int, message: Union[str, bytes]) -> Future[bool]:
        """发送消息

        :param toPlayerID: 接收方队内编号, 舰船为1~4, 基地为0
        :param message: 待发送消息, 分为 `str` 型和 `bytes` 型
        :return: 发送是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def HaveMessage(self) -> bool:
        """检查是否拥有待接收消息

        :return: 是否拥有待接收消息
        """
        pass

    @abstractmethod
    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        """接收消息队列的第一个消息

        :return: 消息发送方的队内编号与消息, 如无消息发送方编号为-1
        """
        pass

    @abstractmethod
    def GetFrameCount(self) -> int:
        """获取当前帧率

        :raise: `NotImplementedError`
        """
        pass

    @abstractmethod
    def Wait(self) -> Future[bool]:
        """等待一帧
        - 在 `Setting.Asynchronous() == True` 下

        :return: 等待是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def EndAllAction(self) -> Future[bool]:
        """发出停止一切行动指令

        :return: 是否进入无行动状态, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def GetShips(self) -> List[THUAI7.Ship]:
        """获取本队所有舰船

        :return: 本队所有舰船, 详见 `THUAI7.Ship` 定义
        """
        pass

    @abstractmethod
    def GetEnemyShips(self) -> List[THUAI7.Ship]:
        """获取敌方舰船
        - 需要敌方舰船处于视野内, 对于 `TeamAPI` 指所有舰船视野

        :return: 视野内敌方所有舰船, 详见 `THUAI7.Ship` 定义
        """
        pass

    @abstractmethod
    def GetBullets(self) -> List[THUAI7.Bullet]:
        """获取敌方子弹
        - 需要子弹处于视野内

        :return: 视野内敌方所有子弹, 详见 `THUAI7.Bullet` 定义
        :raise: `NotImplementedError` 对于 `TeamAPI`
        """
        pass

    @abstractmethod
    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        """获取地图

        :return: `THUAI7.PlaceType` 的二维数组
        """
        pass

    @abstractmethod
    def GetGameInfo(self) -> THUAI7.GameInfo:
        """获取当前游戏信息

        :return: 当前游戏信息, 详见 `THUAI7.GameInfo` 定义
        """
        pass

    @abstractmethod
    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        """获取区域类型

        :param cellX: X坐标, 单位Cell
        :param cellY: Y坐标, 单位Cell
        :return: 该坐标的区域类型
        """
        pass

    @abstractmethod
    def GetConstructionState(self, cellX: int, cellY: int) -> THUAI7.ConstructionState | None:
        """获取当前建筑状态

        :param cellX: X坐标, 单位Cell
        :param cellY: Y坐标, 单位Cell
        :return: 该建筑信息
        """
        pass

    @abstractmethod
    def GetWormholeHp(self, cellX: int, cellY: int) -> int:
        """获取当前虫洞状态

        :param cellX: X坐标, 单位Cell
        :param cellY: Y坐标, 单位Cell
        :return: 当前虫洞血量
        """
        pass

    @abstractmethod
    def GetResourceState(self, cellX: int, cellY: int) -> int:
        """获取当前资源状态

        :param cellX: X坐标, 单位Cell
        :param cellY: Y坐标, 单位Cell
        :return: 当前资源剩余可开采量
        """
        pass

    @abstractmethod
    def GetHomeHp(self) -> int:
        """获取当前大本营血量

        :return: 当前大本营血量
        """
        pass

    @abstractmethod
    def GetEnergy(self) -> int:
        """获取当前经济

        :return: 当前经济
        """
        pass

    @abstractmethod
    def GetScore(self) -> int:
        """获取当前得分

        :return: 当前得分
        """
        pass

    @abstractmethod
    def GetPlayerGUIDs(self) -> List[int]:
        """获取本队所有舰船GUID

        :return: 本队所有舰船GUID
        """
        pass

    @abstractmethod
    def Print(self, string: str) -> None:
        """
        (DEBUG)打印字符串

        :param string: 待打印字符串
        """
        pass

    @abstractmethod
    def PrintShip(self) -> None:
        """
        (DEBUG)打印所有舰船
        """
        pass

    @abstractmethod
    def PrintTeam(self) -> None:
        """
        (DEBUG)打印队伍信息
        """
        pass

    @abstractmethod
    def PrintSelfInfo(self) -> None:
        """
        (DEBUG)打印自身信息, `ShipDebugAPI` 打印舰船信息, `TeamDebugAPI` 打印队伍信息
        """
        pass


class IShipAPI(IAPI, metaclass=ABCMeta):
    @abstractmethod
    def Move(self, timeInMilliseconds: int, angleInRadian: float) -> Future[bool]:
        """发出移动指令

        :param timeInMilliseconds: 期望移动的毫秒数
        :param angleInRadian: 期望移动的弧度数, 向下为x轴正方向, 向右为y轴正方向
        :return: 移动是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def MoveRight(self, timeInMilliseconds: int) -> Future[bool]:
        """发出向右移动指令

        :param timeInMilliseconds: 期望移动的毫秒数
        :return: 移动是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def MoveUp(self, timeInMilliseconds: int) -> Future[bool]:
        """发出向上移动指令

        :param timeInMilliseconds: 期望移动的毫秒数
        :return: 移动是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def MoveLeft(self, timeInMilliseconds: int) -> Future[bool]:
        """发出向左移动指令

        :param timeInMilliseconds: 期望移动的毫秒数
        :return: 移动是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def MoveDown(self, timeInMilliseconds: int) -> Future[bool]:
        """发出向下移动指令

        :param timeInMilliseconds: 期望移动的毫秒数
        :return: 移动是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def Attack(self, angleInRadian: float) -> Future[bool]:
        """发出攻击指令

        :param angleInRadian: 期望攻击的弧度值, 向下为x轴正方向, 向右为y轴正方向
        :return: 攻击是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def Recover(self, recover: int) -> Future[bool]:
        """发出回复指令
        - 需要接近可用的 `Home` 或 `Community`(`Construction`)

        :param recover: 期望回复生命值
        :return: 回复是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def Produce(self) -> Future[bool]:
        """发出生产指令
        - 需要接近未采集完的 `Resource`

        :return: 进入生产状态是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def Rebuild(self, constructionType: THUAI7.ConstructionState) -> Future[bool]:
        """发出重建指令
        - 需要接近待重建 `Construction`

        :param constructionType: 建筑类型
        :return: 进入建造状态是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def RepairWormhole(self) -> Future[bool]:
        """发出修复虫洞指令
        - 需要接近待建造的 `Wormhole`

        :return: 是否成功进入修复虫洞状态，通过`.result()`方法等待获取`bool`
        """
        pass

    @abstractmethod
    def RepairHome(self) -> Future[bool]:
        """发出修复大本营指令
        - 需要接近待建造的 `Home`

        :return: 是否成功进入修复大本营状态，通过`.result()`方法等待获取`bool`
        """
        pass

    @abstractmethod
    def Construct(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        """发出建造指令
        - 需要接近待建 `Construction`

        :param constructionType: 建筑类型
        :return: 进入建造状态是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def GetSelfInfo(self) -> THUAI7.Ship:
        """获取本舰船信息

        :return: 舰船信息, 详见 `THUAI7.Ship` 定义
        """
        pass

    @abstractmethod
    def HaveView(self, gridX: int, gridY: int) -> bool:
        """检测是否拥有视野

        :param gridX: 待检测X坐标, 单位Grid
        :param gridY: 待检测Y坐标, 单位Grid
        :return: 是否拥有视野
        """
        pass


class ITeamAPI(IAPI, metaclass=ABCMeta):
    @abstractmethod
    def GetSelfInfo(self) -> THUAI7.Team:
        """获取本队伍信息

        :return: 队伍信息, 详见 `THUAI7.Team` 定义
        """
        pass

    @abstractmethod
    def InstallModule(
        self, playerID: int, moduleType: THUAI7.ModuleType
    ) -> Future[bool]:
        """安装模块

        :param playerID: 待安装模块的舰船编号
        :param moduleType: 模块类型
        :return: 安装是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def Recycle(self, playerID: int) -> Future[bool]:
        """回收舰船

        :param playerID: 待回收舰船编号
        :return: 回收是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass

    @abstractmethod
    def BuildShip(self, shipType: THUAI7.ShipType, birthIndex: int) -> Future[bool]:
        """建造舰船

        :param shipType: 舰船类型
        :param birthIndex: 出生点 (可用的 `Home` 与 `Community`(`Construction`)) 编号
        :return: 建造是否成功, 通过 `.result()` 方法等待获取 `bool`
        """
        pass


class IAI(metaclass=ABCMeta):
    @abstractmethod
    def ShipPlay(self, api: IShipAPI) -> None:
        pass

    @abstractmethod
    def TeamPlay(self, api: ITeamAPI) -> None:
        pass


class IGameTimer(metaclass=ABCMeta):
    @abstractmethod
    def StartTimer(self) -> None:
        pass

    @abstractmethod
    def EndTimer(self) -> None:
        pass

    @abstractmethod
    def Play(self, ai: IAI) -> None:
        pass


class IErrorHandler(metaclass=ABCMeta):
    @staticmethod
    @abstractmethod
    def result():
        pass
