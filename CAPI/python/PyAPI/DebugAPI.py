import PyAPI.structures as THUAI7
from PyAPI.Interface import ILogic, IShipAPI, ITeamAPI, IGameTimer, IAI
from math import pi
from concurrent.futures import ThreadPoolExecutor, Future
from typing import List, cast, Tuple, Union
import logging
import os
import datetime


class ShipDebugAPI(IShipAPI, IGameTimer):
    def __init__(
        self,
        logic: ILogic,
        file: bool,
        screen: bool,
        warnOnly: bool,
        playerID: int,
        teamID: int,
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
            + f"/logs/api-{teamID}-{playerID}-log.txt",
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

    def Recover(self, recover: int) -> Future[bool]:
        self.__logger.info(f"Recover: called at {self.__GetTime()}ms")

        def logRecover() -> bool:
            result = self.__logic.Recover(recover)
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

    def RepairWormhole(self) -> Future[bool]:
        self.__logger.info(f"RepairWormhole: called at {self.__GetTime()}ms")

        def logRepairWormhole() -> bool:
            result = self.__logic.RepairWormhole()
            if not result:
                self.__logger.warning(f"RepairWormhole failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logRepairWormhole)

    def RepairHome(self) -> Future[bool]:
        self.__logger.info(f"RepairHome: called at {self.__GetTime()}ms")

        def logRepairHome() -> bool:
            result = self.__logic.RepairHome()
            if not result:
                self.__logger.warning(f"RepairHome failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logRepairHome)

    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        self.__logger.info(
            f"Rebuild: called at {self.__GetTime()}ms construction type {constructionType}"
        )

        def logRebuild() -> bool:
            result = self.__logic.Rebuild(constructionType)
            if not result:
                self.__logger.warning(
                    f"Rebuild failed at {self.__GetTime()}ms with construction type {constructionType}"
                )
            return result

        return self.__pool.submit(logRebuild)

    def Construct(self, constructionType: THUAI7.ConstructionType) -> Future[bool]:
        self.__logger.info(
            f"Construct: called at {self.__GetTime()}ms with construction type {constructionType}"
        )

        def logConstruct() -> bool:
            result = self.__logic.Construct(constructionType)
            if not result:
                self.__logger.warning(
                    f"Construct failed at {self.__GetTime()}ms with construction type {constructionType}"
                )
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

    def GetShips(self) -> List[THUAI7.Ship]:
        return self.__logic.GetShips()

    def GetEnemyShips(self) -> List[THUAI7.Ship]:
        return self.__logic.GetEnemyShips()

    def GetBullets(self) -> List[THUAI7.Bullet]:
        return self.__logic.GetBullets()

    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        return self.__logic.GetFullMap()

    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        return self.__logic.GetPlaceType(cellX, cellY)

    def GetConstructionState(self, cellX: int, cellY: int) -> THUAI7.ConstructionState | None:
        return self.__logic.GetConstructionState(cellX, cellY)

    def GetWormholeHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetWormholeHp(cellX, cellY)

    def GetResourceState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetResourceState(cellX, cellY)

    def GetHomeHp(self) -> int:
        return self.__logic.GetHomeHp()

    def GetGameInfo(self) -> THUAI7.GameInfo:
        return self.__logic.GetGameInfo()

    def GetPlayerGUIDs(self) -> List[int]:
        return self.__logic.GetPlayerGUIDs()

    def GetSelfInfo(self) -> THUAI7.Ship:
        return cast(THUAI7.Ship, self.__logic.GetSelfInfo())

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

    def Print(self, string: str) -> None:
        self.__logger.info(string)

    def PrintShip(self) -> None:
        for ship in self.__logic.GetShips():
            self.__logger.info("******ship Info******")
            self.__logger.info(
                f"teamID={ship.teamID} playerID={ship.playerID}, GUID={ship.guid} shipType:{ship.shipType}"
            )
            self.__logger.info(
                f"x={ship.x}, y={ship.y} hp={ship.hp} armor={ship.armor} shield={ship.shield} state:{ship.shipState}"
            )
            self.__logger.info(
                f"speed={ship.speed}, view range={ship.viewRange}, facingDirection={ship.facingDirection}"
            )
            self.__logger.info(
                f"producerType:{ship.producerType} constructorType:{ship.constructorType}"
            )
            self.__logger.info(
                f"armorType:{ship.armorType} shieldType:{ship.shieldType} weaponType:{ship.weaponType}"
            )
            self.__logger.info("************************\n")

    def PrintTeam(self) -> None:
        pass

    def PrintSelfInfo(self) -> None:
        ship = self.__logic.GetSelfInfo()
        self.__logger.info("******Self Info******")
        self.__logger.info(
            f"type={THUAI7.shipTypeDict[ship.shipType]}, playerID={ship.playerID}, GUID={ship.guid}, x={ship.x}, y={ship.y}"
        )
        self.__logger.info(
            f"state={THUAI7.shipStateDict[ship.shipState]}, speed={ship.speed}, view range={ship.viewRange}, facing direction={ship.facingDirection}"
        )
        self.__logger.info("*********************\n")

    def __GetTime(self) -> float:
        return (datetime.datetime.now() - self.__startPoint) / datetime.timedelta(
            milliseconds=1
        )

    def StartTimer(self) -> None:
        self.__startPoint = datetime.datetime.now()
        self.__logger.info("=== AI.play() ===")
        self.__logger.info(f"StartTimer: {self.__startPoint.time()}")

    def EndTimer(self) -> None:
        self.__logger.info(f"Time elapsed: {self.__GetTime()}ms")

    def Play(self, ai: IAI) -> None:
        ai.ShipPlay(self)


class TeamDebugAPI(ITeamAPI, IGameTimer):
    def __init__(
        self,
        logic: ILogic,
        file: bool,
        screen: bool,
        warnOnly: bool,
        playerID: int,
        teamID: int,
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
            + f"/logs/api-{teamID}-{playerID}-log.txt",
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

    def InstallModule(
        self, playerID: int, moduleType: THUAI7.ModuleType
    ) -> Future[bool]:
        self.__logger.info(
            f"InstallModule: playerID = {playerID}, type = {moduleType}, called at {self.__GetTime()}ms"
        )

        def logInstallModule() -> bool:
            result = self.__logic.InstallModule(playerID, moduleType)
            if not result:
                self.__logger.warning(f"InstallModule: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logInstallModule)

    def Recycle(self, playerID: int) -> Future[bool]:
        self.__logger.info(f"Recycle: ID = {playerID}, called at {self.__GetTime()}ms")

        def logRecycle() -> bool:
            result = self.__logic.Recycle(playerID)
            if not result:
                self.__logger.warning(f"Recycle: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logRecycle)

    def BuildShip(self, shipType: THUAI7.ShipType, birthIndex: int) -> Future[bool]:
        self.__logger.info(
            f"BuildShip: shipType = {shipType},birthIndex:{birthIndex} called at {self.__GetTime()}ms"
        )

        def logBuildShip() -> bool:
            result = self.__logic.BuildShip(shipType, birthIndex)
            if not result:
                self.__logger.warning(f"BuildShip: failed at {self.__GetTime()}ms")
            return result

        return self.__pool.submit(logBuildShip)

    def GetFrameCount(self) -> int:
        return self.__logic.GetFrameCount()

    def GetBullets(self) -> List[THUAI7.Bullet]:
        return self.__logic.GetBullets()

    def GetShips(self) -> List[THUAI7.Ship]:
        return self.__logic.GetShips()

    def GetEnemyShips(self) -> List[THUAI7.Ship]:
        return self.__logic.GetEnemyShips()

    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        return self.__logic.GetFullMap()

    def GetPlaceType(self, cellX: int, cellY: int) -> THUAI7.PlaceType:
        return self.__logic.GetPlaceType(cellX, cellY)

    def GetConstructionState(self, cellX: int, cellY: int) -> THUAI7.ConstructionState | None:
        return self.__logic.GetConstructionState(cellX, cellY)

    def GetWormholeHp(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetWormholeHp(cellX, cellY)

    def GetResourceState(self, cellX: int, cellY: int) -> int:
        return self.__logic.GetResourceState(cellX, cellY)

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

    def Print(self, string: str) -> None:
        self.__logger.info(string)

    def PrintShip(self) -> None:
        for ship in self.__logic.GetShips():
            self.__logger.info("******ship Info******")
            self.__logger.info(
                f"teamID={ship.teamID} playerID={ship.playerID}, GUID={ship.guid} shipType:{ship.shipType}"
            )
            self.__logger.info(
                f"x={ship.x}, y={ship.y} hp={ship.hp} armor={ship.armor} shield={ship.shield} state:{ship.shipState}"
            )
            self.__logger.info(
                f"speed={ship.speed}, view range={ship.viewRange}, facingDirection={ship.facingDirection}"
            )
            self.__logger.info(
                f"producerType:{ship.producerType} constructorType:{ship.constructorType}"
            )
            self.__logger.info(
                f"armorType:{ship.armorType} shieldType:{ship.shieldType} weaponType:{ship.weaponType}"
            )
            self.__logger.info("************************\n")

    def PrintTeam(self) -> None:
        self.PrintSelfInfo()

    def PrintSelfInfo(self) -> None:
        selfInfo = self.__logic.GetSelfInfo()
        self.__logger.info("******self team info******")
        self.__logger.info(
            f"teamID:{selfInfo.teamID} playerID:{selfInfo.playerID} score:{selfInfo.score} energy:{selfInfo.energy}"
        )
        self.__logger.info("************************\n")

    def __GetTime(self) -> float:
        return (datetime.datetime.now() - self.__startPoint) / datetime.timedelta(
            milliseconds=1
        )

    def StartTimer(self) -> None:
        self.__startPoint = datetime.datetime.now()
        self.__logger.info("=== AI.play() ===")
        self.__logger.info(f"StartTimer: {self.__startPoint.time()}")

    def EndTimer(self) -> None:
        self.__logger.info(f"Time elapsed: {self.__GetTime()}ms")

    def Play(self, ai: IAI) -> None:
        ai.TeamPlay(self)
