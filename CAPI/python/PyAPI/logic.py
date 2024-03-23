import os
from typing import List, Union, Callable, Tuple
import threading
import logging
import copy
import platform
import proto.MessageType_pb2 as MessageType
import proto.Message2Server_pb2 as Message2Server
import proto.Message2Clients_pb2 as Message2Clients
from queue import Queue
import PyAPI.structures as THUAI7
from PyAPI.utils import Proto2THUAI7, AssistFunction
from PyAPI.API import SweeperAPI, TeamAPI
from PyAPI.AI import Setting
from PyAPI.Communication import Communication
from PyAPI.State import State
from PyAPI.Interface import ILogic, IGameTimer


class Logic(ILogic):
    def __init__(self, playerID: int, teamID: int, playerType: THUAI7.PlayerType, sweeperType: THUAI7.SweeperType) -> None:
        self.__playerID: int = playerID
        self.__teamID: int = teamID
        self.__playerType: THUAI7.PlayerType = playerType
        self.__sweeperType: THUAI7.SweeperType = sweeperType

        self.__comm: Communication

        self.__currentState: State = State()
        self.__bufferState: State = State()

        self.__timer: IGameTimer

        self.__threadAI: threading.Thread

        self.__mtxState: threading.Lock = threading.Lock()

        self.__cvBuffer: threading.Condition = threading.Condition()
        self.__cvAI: threading.Condition = threading.Condition()

        self.__counterState: int = 0
        self.__counterBuffer: int = 0

        self.__gameState: THUAI7.GameState = THUAI7.GameState.NullGameState

        self.__AILoop: bool = True

        self.__bufferUpdated: bool = False

        self.__AIStart: bool = False

        self.__freshed: bool = False

        self.__logger: logging.Logger = logging.getLogger("Logic")

        self.__messageQueue: Queue = Queue()

    def GetSweepers(self) -> List[THUAI7.Sweeper]:
        with self.__mtxState:
            self.__logger.debug("Called GetSweepers")
            return copy.deepcopy(self.__currentState.sweepers)

    def GetEnemySweepers(self) -> List[THUAI7.Sweeper]:
        with self.__mtxState:
            self.__logger.debug("Called GetEnemySweepers")
            return copy.deepcopy(self.__currentState.enemySweepers)

    def GetBullets(self) -> List[THUAI7.Bullet]:
        with self.__mtxState:
            self.__logger.debug("Called GetBullets")
            return copy.deepcopy(self.__currentState.bullets)

    def GetSelfInfo(self) -> Union[THUAI7.Sweeper, THUAI7.Team]:
        with self.__mtxState:
            self.__logger.debug("Called GetSelfInfo")
            return copy.deepcopy(self.__currentState.self)

    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        with self.__mtxState:
            self.__logger.debug("Called GetFullMap")
            return copy.deepcopy(self.__currentState.gameMap)

    def GetPlaceType(self, x: int, y: int) -> THUAI7.PlaceType:
        with self.__mtxState:
            if x < 0 or x >= len(self.__currentState.gameMap) or y < 0 or y >= len(self.__currentState.gameMap[0]):
                self.__logger.warning("GetPlaceType: Out of range")
                return THUAI7.PlaceType(0)
            self.__logger.debug("Called GetPlaceType")
            return copy.deepcopy(self.__currentState.gameMap[x][y])

    def GetGameInfo(self) -> THUAI7.GameInfo:
        with self.__mtxState:
            self.__logger.debug("Called GetGameInfo")
            return copy.deepcopy(self.__currentState.gameInfo)

    def Move(self, time: int, angle: float) -> bool:
        self.__logger.debug("Called Move")
        return self.__comm.Move(time, angle, self.__playerID)

    def SendMessage(self, toID: int, message: Union[str, bytes]) -> bool:
        self.__logger.debug("Called SendMessage")
        return self.__comm.SendMessage(toID, message, self.__playerID, self.__teamID)

    def HaveMessage(self) -> bool:
        self.__logger.debug("Called HaveMessage")
        return not self.__messageQueue.empty()

    def GetMessage(self) -> Tuple[int, Union[str, bytes]]:
        self.__logger.debug("Called GetMessage")
        if self.__messageQueue.empty():
            self.__logger.warning("GetMessage: No message")
            return -1, ""
        else:
            return self.__messageQueue.get()

    def WaitThread(self) -> bool:
        self.__Update()
        return True

    def GetCounter(self) -> int:
        with self.__mtxState:
            return copy.deepcopy(self.__counterState)

    def GetPlayerGUIDs(self) -> List[int]:
        with self.__mtxState:
            return copy.deepcopy(self.__currentState.guids)

    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetConstructionHp")
            if (cellX, cellY) in self.__currentState.mapInfo.factoryState:
                return copy.deepcopy(self.__currentState.mapInfo.factoryState[(cellX, cellY)])
            elif (cellX, cellY) in self.__currentState.mapInfo.communityState:
                return copy.deepcopy(self.__currentState.mapInfo.communityState[(cellX, cellY)])
            elif (cellX, cellY) in self.__currentState.mapInfo.fortState:
                return copy.deepcopy(self.__currentState.mapInfo.fortState[(cellX, cellY)])
            else:
                self.__logger.warning("GetConstructionHp: Out of range")
                return -1

    def GetWormHp(self, cellX: int, cellY: int) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetWormHp")
            if (cellX, cellY) not in self.__currentState.mapInfo.wormholeState:
                self.__logger.warning("GetWormHp: Out of range")
                return -1
            else:
                return copy.deepcopy(self.__currentState.mapInfo.wormholeState[(cellX, cellY)])

    def GetResourceState(self, cellX: int, cellY: int) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetResourceState")
            if (cellX, cellY) not in self.__currentState.mapInfo.resourceState:
                self.__logger.warning("GetResourceState: Out of range")
                return -1
            else:
                return copy.deepcopy(self.__currentState.mapInfo.resourceState[(cellX, cellY)])

    def GetHomeHp(self) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetHomeHp")
            return copy.deepcopy(self.__currentState.gameInfo.blueHomeHp
                                 if self.__teamID == 1
                                 else self.__currentState.gameInfo.redHomeHp)

    def GetMoney(self) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetMoney")
            return copy.deepcopy(self.__currentState.gameInfo.blueMoney
                                 if self.__teamID == 1
                                 else self.__currentState.gameInfo.redMoney)

    def GetScore(self) -> int:
        with self.__mtxState:
            self.__logger.debug("Called GetScore")
            return copy.deepcopy(self.__currentState.gameInfo.blueScore
                                 if self.__teamID == 1
                                 else self.__currentState.gameInfo.redScore)

    def Attack(self, angle: float) -> int:
        self.__logger.debug("Called Attack")
        return self.__comm.Attack(angle, self.__playerID, self.__teamID)

    def EndAllAction(self) -> bool:
        self.__logger.debug("Called EndAllAction")
        return self.__comm.EndAllAction(self.__playerID, self.__teamID)

    def HaveView(self, gridX: int, gridY: int, selfX: int, selfY: int, viewRange: int) -> bool:
        with self.__mtxState:
            self.__logger.debug("Called HaveView")
            return AssistFunction.HaveView(viewRange, selfX, selfY, gridX, gridY, self.__currentState.gameMap)

    def TryConnection(self) -> bool:
        self.__logger.info("Called TryConnection")
        return self.__comm.TryConnection(self.__playerID, self.__teamID)

    def Recover(self, recover: int) -> bool:
        self.__logger.debug("Called Recover")
        return self.__comm.Recover(self.__playerID, self.__teamID, recover)

    def Produce(self) -> bool:
        self.__logger.debug("Called Produce")
        return self.__comm.Produce(self.__playerID, self.__teamID)

    def Rebuild(self, constructionType: THUAI7.ConstructionType) -> bool:
        self.__logger.debug("Called Rebuild")
        return self.__comm.Rebuild(constructionType, self.__playerID, self.__teamID)

    def Construct(self, constructionType: THUAI7.ConstructionType) -> bool:
        self.__logger.debug("Called Construct")
        return self.__comm.Construct(constructionType, self.__playerID, self.__teamID)

    def InstallModule(self, moduleType: THUAI7.ModuleType) -> bool:
        self.__logger.debug("Called InstallModule")
        return self.__comm.InstallModule(moduleType, self.__playerID, self.__teamID)

    def Recycle(self) -> bool:
        self.__logger.debug("Called Recycle")
        return self.__comm.Recycle(self.__playerID, self.__playerID, self.__teamID)

    def BuildSweeper(self, sweeperType: THUAI7.SweeperType) -> bool:
        self.__logger.debug("Called BuildSweeper")
        return self.__comm.BuildSweeper(sweeperType, self.__teamID)

    def __TryConnection(self) -> bool:
        self.__logger.info("Try to connect to the server.")
        return self.__comm.TryConnection(self.__playerID, self.__teamID)

    def __ProcessMessage(self) -> None:
        def messageThread():
            self.__logger.info("Message thread started")
            self.__comm.AddPlayer(self.__playerID, self.__teamID, self.__sweeperType)
            self.__logger.info("Player added")

            while self.__gameState != THUAI7.GameState.GameEnd:
                clientMsg = self.__comm.GetMessage2Client()
                self.__logger.debug("Get message from server")
                self.__gameState = Proto2THUAI7.gameStateDict[clientMsg.game_state]

                if self.__gameState == THUAI7.GameState.GameStart:
                    self.__logger.info("Game start!")

                    for obj in clientMsg.obj_message:
                        if obj.WhichOneof("message_of_obj") == "map_message":
                            gameMap: List[List[THUAI7.PlaceType]] = []
                            for row in obj.map_message.rows:
                                cols: List[THUAI7.PlaceType] = []
                                for place in row.cols:
                                    cols.append(Proto2THUAI7.placeTypeDict[place])
                                gameMap.append(cols)
                            self.__currentState.gameMap = gameMap
                            self.__bufferState.gameMap = gameMap
                            self.__logger.info("Game map loaded!")
                            break
                    else:
                        self.__logger.error("No map message received")

                    self.__LoadBuffer(clientMsg)
                    self.__AILoop = True
                    self.__UnBlockAI()

                elif self.__gameState == THUAI7.GameState.GameRunning:
                    # 读取玩家的GUID
                    self.__LoadBuffer(clientMsg)
                else:
                    self.__logger.error("Unknown GameState!")
                    continue
            with self.__cvBuffer:
                self.__bufferUpdated = True
                self.__counterBuffer = -1
                self.__cvBuffer.notify()
                self.__logger.info("Game End!")
            self.__logger.info("Message thread end!")
            self.__AILoop = False

        threading.Thread(target=messageThread).start()

    def LoadBuffer(self, message: Message2Clients.MessageToClient) -> None:
        with self.__cvBuffer:
            self.__bufferState.sweepers.clear()
            self.__bufferState.enemySweepers.clear()
            self.__bufferState.teams.clear()
            self.__bufferState.bullets.clear()
            self.__bufferState.bombedBullets.clear()
            self.__bufferState.guids.clear()
            self.__logger.debug("Buffer cleared")

            if self.__playerID != 0:
                for obj in message.obj_message:
                    if obj.WhichOneof("message_of_obj") == "sweeper_message":
                        self.__bufferState.guids.append(obj.sweeper_message.guid)
            else:
                for obj in message.obj_message:
                    if obj.WhichOneof("message_of_obj") == "team_message":
                        self.__bufferState.guids.append(obj.team_message.guid)

            self.__bufferState.gameInfo = Proto2THUAI7.Protobuf2THUAI7GameInfo(message.all_message)

            self.__LoadBufferSelf(message)
            for item in message.obj_message:
                self.__LoadBufferCase(item)
            if Setting.Asynchronous():
                with self.__mtxState:
                    self.__currentState, self.__bufferState = self.__bufferState, self.__currentState
                    self.__counterState = self.__counterBuffer
                    self.__logger.info("Update state!")
                self.__freshed = True
            else:
                self.__bufferUpdated = True
            self.__counterBuffer += 1
            self.__cvBuffer.notify()

    def __LoadBufferSelf(self, message: Message2Clients.MessageToClient) -> None:
        if self.__playerID != 0:
            for item in message.obj_message:
                if item.WhichOneof("message_of_obj") == "sweeper_message":
                    if item.sweeper_message.player_id == self.__playerID:
                        self.__bufferState.self = Proto2THUAI7.Protobuf2THUAI7Sweeper(item.sweeper_message)
                        self.__bufferState.sweepers.append(self.__bufferState.self)
                    else:
                        self.__bufferState.sweepers.append(Proto2THUAI7.Protobuf2THUAI7Sweeper(item.sweeper_message))
                    self.__logger.debug("Load sweeper")
        else:
            for item in message.obj_message:
                if item.WhichOneof("message_of_obj") == "team_message":
                    self.__bufferState.self = Proto2THUAI7.Protobuf2THUAI7Team(item.team_message)
                    self.__bufferState.teams.append(self.__bufferState.self)
                    self.__logger.debug("Load team")

    def __LoadBufferCase(self, item: Message2Clients.MessageOfObj) -> None:
        if item.WhichOneof("message_of_obj") == "sweeper_message":
            if item.sweeper_message.team_id != self.__teamID:
                if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                           self.__bufferState.self.x, self.__bufferState.self.y,
                                           item.sweeper_message.x, item.sweeper_message.y,
                                           self.__bufferState.gameMap):
                    self.__bufferState.enemySweepers.append(Proto2THUAI7.Protobuf2THUAI7Sweeper(item.sweeper_message))
                    self.__logger.debug("Load enemy sweeper")

        elif item.WhichOneof("message_of_obj") == "bullet_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                       self.__bufferState.self.x, self.__bufferState.self.y,
                                       item.bullet_message.x, item.bullet_message.y,
                                       self.__bufferState.gameMap):
                self.__bufferState.bullets.append(Proto2THUAI7.Protobuf2THUAI7Bullet(item.bullet_message))
                self.__logger.debug("Add Bullet!")

        elif item.WhichOneof("message_of_obj") == "factory_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange, self.__bufferState.self.x,
                                       self.__bufferState.self.y, item.factory_message.x, item.factory_message.y,
                                       self.__bufferState.gameMap):
                pos = (AssistFunction.GridToCell(item.factory_message.x),
                       AssistFunction.GridToCell(item.factory_message.y))
                if pos not in self.__bufferState.mapInfo.factoryState:
                    self.__bufferState.mapInfo.factoryState[pos] = item.factory_message.hp
                    self.__logger.debug("New Factory")
                else:
                    self.__bufferState.mapInfo.factoryState[pos] = item.factory_message.hp
                    self.__logger.debug("Update Factory")

        elif item.WhichOneof("message_of_obj") == "community_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                       self.__bufferState.self.x, self.__bufferState.self.y,
                                       item.community_message.x, item.community_message.y,
                                       self.__bufferState.gameMap):
                pos = (AssistFunction.GridToCell(item.community_message.x),
                       AssistFunction.GridToCell(item.community_message.y))
                if pos not in self.__bufferState.mapInfo.communityState:
                    self.__bufferState.mapInfo.communityState[pos] = item.community_message.hp
                    self.__logger.debug("New Community")
                else:
                    self.__bufferState.mapInfo.communityState[pos] = item.community_message.hp
                    self.__logger.debug("Update Community")

        elif item.WhichOneof("message_of_obj") == "fort_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                       self.__bufferState.self.x, self.__bufferState.self.y,
                                       item.fort_message.x, item.fort_message.y,
                                       self.__bufferState.gameMap):
                pos = (AssistFunction.GridToCell(item.fort_message.x),
                       AssistFunction.GridToCell(item.fort_message.y))
                if pos not in self.__bufferState.mapInfo.fortState:
                    self.__bufferState.mapInfo.fortState[pos] = item.fort_message.hp
                    self.__logger.debug("New Fort")
                else:
                    self.__bufferState.mapInfo.fortState[pos] = item.fort_message.hp
                    self.__logger.debug("Update Fort")

        elif item.WhichOneof("message_of_obj") == "wormhole_message":
            pos = (AssistFunction.GridToCell(item.wormhole_message.x),
                   AssistFunction.GridToCell(item.wormhole_message.y))
            self.__bufferState.mapInfo.wormholeState[pos] = item.wormhole_message.hp
            self.__logger.debug("Update Wormhole")

        elif item.WhichOneof("message_of_obj") == "home_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                       self.__bufferState.self.x, self.__bufferState.self.y,
                                       item.home_message.x, item.home_message.y,
                                       self.__bufferState.gameMap):
                pos = (AssistFunction.GridToCell(item.home_message.x),
                       AssistFunction.GridToCell(item.home_message.y))
                self.__bufferState.mapInfo.homeState[pos] = item.home_message.hp
                self.__logger.debug("Update Home")

        elif item.WhichOneof("message_of_obj") == "resource_message":
            if AssistFunction.HaveView(self.__bufferState.self.viewRange,
                                       self.__bufferState.self.x, self.__bufferState.self.y,
                                       item.resource_message.x, item.resource_message.y,
                                       self.__bufferState.gameMap):
                pos = (AssistFunction.GridToCell(item.resource_message.x),
                       AssistFunction.GridToCell(item.resource_message.y))
                self.__bufferState.mapInfo.resourceState[pos] = item.resource_message.progress
                self.__logger.debug("Update Resource")

        elif item.WhichOneof("message_of_obj") == "news_message":
            if item.news_message.to_id == self.__playerID:
                if item.news_message.WhichOneof("news") == "text_message":
                    self.__messageQueue.put((item.news_message.from_id, item.news_message.text_message))
                    self.__logger.debug("Add News!")
                elif item.news_message.WhichOneof("news") == "binary_message":
                    self.__messageQueue.put((item.news_message.from_id, item.news_message.binary_message))
                    self.__logger.debug("Add News!")
                else:
                    self.__logger.error("Unknown News!")

        # elif item.WhichOneof("message_of_obj")=="bombed_bullet_message":
        #     if AssistFunction.HaveView(
        #         self.__bufferState.self.viewRange,
        #         self.__bufferState.self.x,
        #         self.__bufferState.self.y,
        #         item.bombed_bullet_message.x,
        #         item.bombed_bullet_message.y,
        #         self.__bufferState.gameMap,
        #     ):
        #         self.__bufferState.bombedBullets.append(Proto2THUAI7.Protobuf2THUAI7BombedBullet(item.bombed_bullet_message))
        #         self.__logger.debug("Add Bombed Bullet!")

        elif item.WhichOneof("message_of_obj") == "team_message":
            if item.team_message.team_id != self.__teamID:
                self.__bufferState.teams.append(Proto2THUAI7.Protobuf2THUAI7Team(item.team_message))
                self.__logger.debug("Add Team!")

        else:
            self.__logger.error("Unknown message!")

    def __UnBlockAI(self) -> None:
        with self.__cvAI:
            self.__AIStart = True
            self.__cvAI.notify()

    def __Update(self) -> None:
        if not Setting.Asynchronous():
            with self.__cvBuffer:
                self.__cvBuffer.wait_for(lambda: self.__bufferUpdated)
                with self.__mtxState:
                    self.__bufferState, self.__currentState = self.__currentState, self.__bufferState
                    self.__counterState = self.__counterBuffer
                self.__bufferUpdated = False
                self.__logger.info("Update state!")

    def __Wait(self) -> None:
        self.__freshed = False
        with self.__cvBuffer:
            self.__cvBuffer.wait_for(lambda: self.__freshed)

    def Main(
        self,
        createAI: Callable,
        IP: str,
        port: str,
        file: bool,
        screen: bool,
        warnOnly: bool,
    ) -> None:
        # 建立日志组件
        self.__logger.setLevel(logging.DEBUG)
        formatter = logging.Formatter(
            "[%(name)s] [%(asctime)s.%(msecs)03d] [%(levelname)s] %(message)s",
            "%H:%M:%S",
        )
        # 确保文件存在
        # if not os.path.exists(os.path.dirname(os.path.dirname(os.path.realpath(__file__))) + "/logs"):
        #     os.makedirs(os.path.dirname(os.path.dirname(
        #         os.path.realpath(__file__))) + "/logs")

        if platform.system().lower() == "windows":
            os.system(f'mkdir "{os.path.dirname(os.path.dirname(os.path.realpath(__file__)))}\\logs"')
        else:
            os.system(f'mkdir -p "{os.path.dirname(os.path.dirname(os.path.realpath(__file__)))}/logs"')

        fileHandler = logging.FileHandler(
            os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
            + "/logs/logic"
            + str(self.__playerID)
            + "-log.txt",
            "w+",
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

        self.__logger.info("*********Basic Info*********")
        self.__logger.info("asynchronous: %s", Setting.Asynchronous())
        self.__logger.info("server: %s:%s", IP, port)
        self.__logger.info("playerID: %s", self.__playerID)
        self.__logger.info("player type: %s", self.__playerType.name)
        self.__logger.info("****************************")

        # 建立通信组件
        self.__comm = Communication(IP, port)

        # 构造timer
        if not file and not screen:
            self.__timer = SweeperAPI(self)
        else:
            self.__timer = SweeperDebugAPI(
                self, file, screen, warnOnly, self.__playerID
            )

        # 构建AI线程
        def AIThread():
            with self.__cvAI:
                self.__cvAI.wait_for(lambda: self.__AIStart)

            ai = createAI(self.__playerID)
            while self.__AILoop:
                if Setting.Asynchronous():
                    self.__Wait()
                    self.__timer.StartTimer()
                    self.__timer.Play(ai)
                    self.__timer.EndTimer()
                else:
                    self.__Update()
                    self.__timer.StartTimer()
                    self.__timer.Play(ai)
                    self.__timer.EndTimer()

        if self.__TryConnection():
            self.__logger.info("Connect to the server successfully, AI thread will be started.")
            self.__threadAI = threading.Thread(target=AIThread)
            self.__threadAI.start()
            self.__logger.info("Start to Process Message")
            self.__ProcessMessage()
            self.__logger.info("Join the AI thread.")
            self.__threadAI.join()
        else:
            self.__AILoop = False
            self.__logger.error("Failed to connect to the server.")
            return
