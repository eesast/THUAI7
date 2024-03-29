from typing import List, Union, Tuple
from concurrent.futures import Future
from abc import abstractmethod, ABCMeta
import PyAPI.structures as THUAI7

from abc import ABCMeta, abstractmethod
from typing import List, Tuple, Union


class ILogic(metaclass=ABCMeta):
    '''`IAPI` 统一可用的接口'''

    @abstractmethod
    def GetSweepers(self) -> List[THUAI7.Sweeper]:
        pass

    @abstractmethod
    def GetEnemySweepers(self) -> List[THUAI7.Sweeper]:
        pass

    @abstractmethod
    def GetBullets(self) -> List[THUAI7.Bullet]:
        pass

    @abstractmethod
    def GetSelfInfo(self) -> Union[THUAI7.Sweeper, THUAI7.Team]:
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
    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetBridgeHp(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetGarbageState(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetHomeHp(self) -> int:
        pass

    @abstractmethod
    def GetMoney(self) -> int:
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
    def GetMessage(self) -> Tuple[int, str]:
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
    def Recover(self) -> bool:
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
    def BuildSweeper(self, sweeperType: THUAI7.SweeperType) -> bool:
        pass


class IAPI(metaclass=ABCMeta):
    '''
    选手可执行的操作，应当保证所有函数的返回值都应当为 `asyncio.Future`，例如下面的移动函数：\n
    指挥本角色进行移动：
    - `timeInMilliseconds` 为移动时间，单位为毫秒
    - `angleInRadian` 表示移动的方向，单位是弧度，使用极坐标——竖直向下方向为 x 轴，水平向右方向为 y 轴\n
    发送信息、接受信息，注意收消息时无消息则返回 `nullopt`
    '''

    @abstractmethod
    def SendMessage(self, toPlayerID: int, message: Union[str, bytes]) -> Future[bool]:
        pass

    @abstractmethod
    def HaveMessage(self) -> bool:
        pass

    @abstractmethod
    def GetMessage(self) -> Tuple[int, str]:
        pass

    @abstractmethod
    def Move(self, timeInMilliseconds: int, angle: float) -> Future[bool]:
        pass

    # 获取游戏目前所进行的帧数
    @abstractmethod
    def GetFrameCount(self) -> int:
        '获取游戏目前所进行的帧数'
        pass

    @abstractmethod
    def Wait(self) -> Future[bool]:
        '等待下一帧'
        pass

    @abstractmethod
    def EndAllAction(self) -> Future[bool]:
        pass

    @abstractmethod
    def GetSweepers(self) -> List[THUAI7.Sweeper]:
        pass

    @abstractmethod
    def GetEnemySweepers(self) -> List[THUAI7.Sweeper]:
        pass

    @abstractmethod
    def GetBullets(self) -> List[THUAI7.Bullet]:
        pass

    @abstractmethod
    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        pass

    @abstractmethod
    def GetGameInfo(self) -> THUAI7.GameInfo:
        pass

    @abstractmethod
    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        pass

    @abstractmethod
    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetBridgeHp(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetGarbageState(self, cellX: int, cellY: int) -> int:
        pass

    @abstractmethod
    def GetHomeHp(self) -> int:
        pass

    @abstractmethod
    def GetMoney(self) -> int:
        pass

    @abstractmethod
    def GetScore(self) -> int:
        pass

    @abstractmethod
    def GetPlayerGUIDs(self) -> List[int]:
        pass

    @abstractmethod
    def Print(self, string: str) -> None:
        pass

    @abstractmethod
    def PrintSweeper(self) -> None:
        pass

    @abstractmethod
    def PrintTeam(self) -> None:
        pass

    @abstractmethod
    def PrintSelfInfo(self) -> None:
        pass

    @abstractmethod
    def GetSelfInfo(self) -> Union[THUAI7.Sweeper, THUAI7.Team]:
        pass


class ISweeperAPI(IAPI, metaclass=ABCMeta):
    @abstractmethod
    def Move(self, timeInMilliseconds: int, angleInRadian: float) -> Future[bool]:
        pass

    @abstractmethod
    def MoveRight(self, timeInMilliseconds: int) -> Future[bool]:
        pass

    @abstractmethod
    def MoveUp(self, timeInMilliseconds: int) -> Future[bool]:
        pass

    @abstractmethod
    def MoveLeft(self, timeInMilliseconds: int) -> Future[bool]:
        pass

    @abstractmethod
    def MoveDown(self, timeInMilliseconds: int) -> Future[bool]:
        pass

    @abstractmethod
    def Attack(self, angleInRadian: float) -> Future[bool]:
        pass

    @abstractmethod
    def Recover(self) -> Future[bool]:
        pass

    @abstractmethod
    def Produce(self) -> Future[bool]:
        pass

    @abstractmethod
    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        pass

    @abstractmethod
    def Construct(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        pass

    @abstractmethod
    def GetSelfInfo(self) -> THUAI7.Sweeper:
        pass

    @abstractmethod
    def HaveView(self, gridX: int, gridY: int) -> bool:
        pass


class ITeamAPI(IAPI, metaclass=ABCMeta):
    @abstractmethod
    def GetSelfInfo(self) -> THUAI7.Team:
        pass

    @abstractmethod
    def InstallModule(
        self, playerID: int, moduleType: THUAI7.ModuleType
    ) -> Future[bool]:
        pass

    @abstractmethod
    def Recycle(self, playerID: int) -> Future[bool]:
        pass

    @abstractmethod
    def BuildSweeper(self, sweeperType: THUAI7.SweeperType) -> Future[bool]:
        pass


class IAI(metaclass=ABCMeta):
    @abstractmethod
    def SweeperPlay(self, api: ISweeperAPI) -> None:
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
