import PyAPI.structures as THUAI7
from PyAPI.Interface import ILogic, ISweeperAPI, ITeamAPI, IGameTimer, IAI
from math import pi
from concurrent.futures import ThreadPoolExecutor, Future
from typing import List, cast, Tuple, Union
import logging
import os
import datetime

class SweeperDebugAPI(ISweeperAPI, IGameTimer):
    def __init__(
        self, logic: ILogic, file: bool, screen: bool, warnOnly: bool, playerID: int
    ) -> None:
        self.__logic = logic
        self.__pool = ThreadPoolExecutor(20)
        self.__startPoint = datetime.datetime.now()
        self.__logger = logging.getLogger("api " + str(playerID))
        self.__logger.setLevel(logging.DEBUG)
        formatter = logging.Formatter(
            "[%(name)s] [%(asctime)s.%(msecs)03d] [%(levelname)s] %(message)s",
            "%H:%M:%S",
        )
        # 确保文件存在
        if not os.path.exists(
            os.path.dirname(os.path.dirname(os.path.realpath(__file__))) + "/logs"
        ):
            os.makedirs(
                os.path.dirname(os.path.dirname(os.path.realpath(__file__))) + "/logs"
            )

        fileHandler = logging.FileHandler(
            os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
            + "/logs/api-"
            + str(playerID)
            + "-log.txt",
            mode="w+",
            encoding="utf-8",
        )
        screenHandler = logging.StreamHandler()
        if file:
            fileHandler.setLevel(logging.DEBUG)
            fileHandler.setFormatter(formatter)
            self.__logger.addHandler(fileHandler)
        if screen:
            if warnOnly:
                screenHandler.setLevel(logging.WARNING)
            else:
                screenHandler.setLevel(logging.INFO)
            screenHandler.setFormatter(formatter)
            self.__logger.addHandler(screenHandler)


    def Move(self, timeInMilliseconds: int, angle: float) -> Future[bool]:
        self.__logger.info(
            f"Move: timeInMilliseconds = {timeInMilliseconds}, angle = {angle}, called at {self.__GetTime()}ms"
        )

        def logMove() -> bool:
            result = self.__logic.Move(timeInMilliseconds, angle)
            if not result:
                self.__logger.warning(f"Move: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logMove)

    def MoveRight(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi * 0.5)

    def MoveLeft(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi * 1.5)

    def MoveUp(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, pi)

    def MoveDown(self, timeInMilliseconds: int) -> Future[bool]:
        return self.Move(timeInMilliseconds, 0)

    def Attack(self, angle: float) -> Future[bool]:
        self.__logger.info(f"Attack: angle = {angle}, called at {self.__GetTime()}ms")

        def logAttack() -> bool:
            result = self.__logic.Attack(angle)
            if not result:
                self.__logger.warning(f"Attack: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logAttack)

    def Recover(self) -> Future[bool]:
        self.__logger.info(f"Recover: called at {self.__GetTime()}ms")
        def logRecover()->bool:
            result=self.__logic.Recover()
            if not result:
                self.__logger.warning(f"Recover failed at {self.__GetTime()}ms")
            return result
        return self.__pool.submit(logRecover)

    def Produce(self) -> Future[bool]:
        self.__logger.info(f"Produce: called at {self.__GetTime()}ms")
        
        def logProduce() -> bool:
            result = self.__logic.Produce()
            if not result:
                self.__logger.warning(f"Produce failed at {self.__GetTime()}ms")
            return result
        
        return self.__pool.submit(logProduce)


    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        self.__logger.info(f"Rebuild: called at {self.__GetTime()}ms construction type {constructionType}")
        
        def logRebuild() -> bool:
            result = self.__logic.Rebuild(constructionType)
            if not result:
                self.__logger.warning(f"Rebuild failed at {self.__GetTime()}ms with construction type {constructionType}")
            return result
        
        return self.__pool.submit(logRebuild)

    def Construct(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        self.__logger.info(f"Construct: called at {self.__GetTime()}ms with construction type {constructionType}")
        
        def logConstruct() -> bool:
            result = self.__logic.Construct(constructionType)
            if not result:
                self.__logger.warning(f"Construct failed at {self.__GetTime()}ms with construction type {constructionType}")
            return result
        
        return self.__pool.submit(logConstruct)

    def Wait(self) -> bool:
        self.__logger.info(f"Wait: called at {self.__GetTime()}ms")
        if self.__logic.GetCounter() == -1:
            return False
        else:
            return self.__logic.WaitThread()

    def EndAllAction(self) -> Future[bool]:
        self.__logger.info(f"EndAllAction: called at {self.__GetTime()}ms")

        def logEnd() -> bool:
            result = self.__logic.EndAllAction()
            if not result:
                self.__logger.warning(f"EndAllAction: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logEnd)
    
    def SendMessage(self, toID: int, message: Union[str, bytes]) -> Future[bool]:
        self.__logger.info(
            f"SendMessage: toID = {toID}, message = {message}, called at {self.__GetTime()}ms"
        )

        def logSend() -> bool:
            result = self.__logic.SendMessage(toID, message)
            if not result:
                self.__logger.warning(f"SendMessage: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logSend)

    def HaveMessage(self) -> bool:
        self.__logger.info(f"HaveMessage: called at {self.__GetTime()}ms")
        result = self.__logic.HaveMessage()
        if not result:
            self.__logger.warning(f"HaveMessage: failed at {self.__GetTime()}ms")
        return result

    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        self.__logger.info(f"GetMessage: called at {self.__GetTime()}ms")
        result = self.__logic.GetMessage()
        if result[0] == -1:
            self.__logger.warning(f"GetMessage: failed at {self.__GetTime()}ms")
        return result

    def GetFrameCount(self) -> int:
        return self.__logic.GetFrameCount()
    
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

    def GetWormHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetWormHp(cellX, cellY)

    def GetResourceState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetResourceState(cellX, cellY)

    def GetHomeHp(self) -> int:
        return self.__logic.GetHomeHp()

    def GetGameInfo(self) -> THUAI7.GameInfo:
        return self.__logic.GetGameInfo()

    def GetPlayerGUIDs(self) -> List[int]:
        return self.__logic.GetPlayerGUIDs()

    def GetSelfInfo(self) -> THUAI7.Sweeper:
        return cast(THUAI7.Sweepers, self.__logic.GetSelfInfo())

    def GetMoney(self) -> int:
        return self.__logic.GetMoney()

    def GetScore(self) -> int:
        return self.__logic.GetScore()

    def HaveView(self, gridX: int, gridY: int) -> bool:
        return self.__logic.HaveView(gridX, gridY,
                                     self.GetSelfInfo().x, self.GetSelfInfo().y,
                                     self.GetSelfInfo().viewRange)

    def Print(self, cont: str) -> None:
        self.__logger.info(cont)

    def PrintSweeper(self) -> None:
        for sweeper in self.__logic.GetSweepers():
            self.__logger.info("******sweeper Info******")
            self.__logger.info(
                f"teamID={sweeper.teamID} playerID={sweeper.playerID}, GUID={sweeper.guid} sweeperType:{sweeper.sweeperType}"
            )
            self.__logger.info(
                f"x={sweeper.x}, y={sweeper.y} hp={sweeper.hp} armor={sweeper.armor} shield={sweeper.shield} state:{sweeper.sweeperState}"
            )
            self.__logger.info(
                f"speed={sweeper.speed}, view range={sweeper.viewRange}, facingDirection={sweeper.facingDirection}"
            )
            self.__logger.info(
                f"producerType:{sweeper.producerType} constructorType:{sweeper.constructorType}"
            )
            self.__logger.info(
                f"armorType:{sweeper.armorType} shieldType:{sweeper.shieldType} weaponType:{sweeper.weaponType}"
            )
            self.__logger.info("************************\n")

    def PrintTeam(self) -> None:
        for team in self.__logic.Get

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

    def GetWormHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetWormHp(cellX, cellY)

    def GetResouceState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetResouceState(cellX, cellY)

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

    def GetMoney(self) -> int:
        return self.__logic.GetMoney()

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

    def PrintSelfInfo(self) -> None:
        pass
