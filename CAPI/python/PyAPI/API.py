import PyAPI.structures as THUAI7
from PyAPI.Interface import ILogic, ISweeperAPI, ITeamAPI, IGameTimer, IAI
from math import pi
from concurrent.futures import ThreadPoolExecutor, Future
from typing import List, cast, Tuple, Union


class SweeperAPI(ISweeperAPI, IGameTimer):
    def __init__(self, logic: ILogic) -> None:
        self.__logic = logic
        self.__pool = ThreadPoolExecutor(20)

    def Move(self, timeInMilliseconds: int, angle: float) -> Future[bool]:
        return self.__pool.submit(self.__logic.Move, timeInMilliseconds, angle)

    def MoveRight(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi * 0.5)

    def MoveLeft(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi * 1.5)

    def MoveUp(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi)

    def MoveDown(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, 0)

    def Attack(self, angle: float) -> Future[bool]:
        return self.__pool.submit(self.__logic.Attack, angle)

    def Recover(self) -> Future[bool]:
        return self.__pool.submit(self.__logic.Recover)

    def Produce(self) -> Future[bool]:
        return self.__pool.submit(self.__logic.Produce)

    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        return self.__pool.submit(self.__logic.Rebuild, constructionType)

    def Construct(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        return self.__pool.submit(self.__logic.Construct, constructionType)

    def GetFrameCount(self) -> int:
        return self.__logic.GetFrameCount()

    def Wait(self) -> bool:
        if self.__logic.GetCounter() == -1:
            return False
        else:
            return self.__logic.WaitThread()

    def EndAllAction(self) -> Future[bool]:
        return self.__pool.submit(self.__logic.EndAllAction)

    def SendMessage(self, toID: int, message: Union[str, bytes]) -> Future[bool]:
        return self.__pool.submit(self.__logic.SendMessage, toID, message)

    def HaveMessage(self) -> bool:
        return self.__logic.HaveMessage()

    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        return self.__logic.GetMessage()

    def GetSweepers(self) -> List[THUAI7.Sweeper]:
        return self.__logic.GetSweepers()

    def GetEnemySweepers(self) -> List[THUAI7.Sweeper]:
        return self.__logic.GetEnemySweepers()

    def GetBullets(self) -> List[THUAI7.Bullet]:
        return self.__logic.GetBullets()

    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        return self.__logic.GetFullMap()

    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        return self.__logic.GetPlaceType(cellX, cellY)

    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetConstructionHp(cellX, cellY)

    def GetBridgeHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetBridgeHp(cellX, cellY)

    def GetGarbageState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetGarbageState(cellX, cellY)

    def GetHomeHp(self) -> int:
        return self.__logic.GetHomeHp()

    def GetGameInfo(self) -> THUAI7.GameInfo:
        return self.__logic.GetGameInfo()

    def GetPlayerGUIDs(self) -> List[int]:
        return self.__logic.GetPlayerGUIDs()

    def GetSelfInfo(self) -> THUAI7.Sweeper:
        return cast(THUAI7.Sweepers, self.__logic.GetSelfInfo())

    def GetEnergy(self) -> int:
        return self.__logic.GetEnergy()

    def GetScore(self) -> int:
        return self.__logic.GetScore()

    def HaveView(self, gridX: int, gridY: int) -> bool:
        return self.__logic.HaveView(
            gridX,
            gridY,
            self.GetSelfInfo().x,
            self.GetSelfInfo().y,
            self.GetSelfInfo().viewRange,
        )

    def Print(self, cont: str) -> None:
        pass

    def PrintSweeper(self) -> None:
        pass

    def PrintTeam(self) -> None:
        pass

    def PrintSelfInfo(self) -> None:
        pass

    def StartTimer(self) -> None:
        pass

    def EndTimer(self) -> None:
        pass

    def Play(self, ai: IAI) -> None:
        ai.SweeperPlay(self)


class TeamAPI(ITeamAPI, IGameTimer):
    def __init__(self, logic: ILogic) -> None:
        self.__logic = logic
        self.__pool = ThreadPoolExecutor(20)

    def StartTimer(self) -> None:
        pass

    def EndTimer(self) -> None:
        pass

    def Play(self, ai: IAI) -> None:
        ai.TeamPlay(self)

    def SendMessage(self, toID: int, message: Union[str, bytes]) -> Future[bool]:
        return self.__pool.submit(self.__logic.SendMessage, toID, message)

    def HaveMessage(self) -> bool:
        return self.__logic.HaveMessage()

    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        return self.__logic.GetMessage()

    def GetFrameCount(self) -> int:
        return self.__logic.GetFrameCount()

    def Wait(self) -> bool:
        if self.__logic.GetCounter() == -1:
            return False
        else:
            return self.__logic.WaitThread()

    def EndAllAction(self) -> Future[bool]:
        return self.__pool.submit(self.__logic.EndAllAction)

    def GetBullets(self) -> List[THUAI7.Bullet]:
        return self.__logic.GetBullets()

    def GetSweepers(self) -> List[THUAI7.Sweeper]:
        return self.__logic.GetSweepers()

    def GetEnemySweepers(self) -> List[THUAI7.Sweeper]:
        return self.__logic.GetEnemySweepers()

    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        return self.__logic.GetFullMap()

    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        return self.__logic.GetPlaceType(cellX, cellY)

    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetConstructionHp(cellX, cellY)

    def GetBridgeHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetBridgeHp(cellX, cellY)

    def GetGarbageState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetGarbageState(cellX, cellY)

    def GetHomeHp(self) -> int:
        return self.__logic.GetHomeHp()

    def GetGameInfo(self) -> THUAI7.GameInfo:
        return self.__logic.GetGameInfo()

    def GetPlayerGUIDs(self) -> List[int]:
        return self.__logic.GetPlayerGUIDs()

    def GetSelfInfo(self) -> THUAI7.Home:
        return cast(THUAI7.Home, self.__logic.GetSelfInfo())

    def GetScore(self) -> int:
        return self.__logic.GetScore()

    def GetEnergy(self) -> int:
        return self.__logic.GetEnergy()

    def InstallModule(self, ID: int, type: THUAI7.ModuleType) -> Future[bool]:
        return self.__pool.submit(self.__logic.InstallModule, ID, type)

    def Recycle(self, ID: int) -> Future[bool]:
        return self.__pool.submit(self.__logic.Recycle, ID)

    def BuildSweeper(self, sweeperType: THUAI7.SweeperType) -> Future[bool]:
        return self.__pool.submit(self.__logic.BuildSweeper, sweeperType)

    def Print(self, string: str) -> None:
        pass

    def PrintTeam(self) -> None:
        pass

    def PrintSweeper(self) -> None:
        pass

    def PrintSelfInfo(self) -> None:
        pass
