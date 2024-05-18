import PyAPI.structures as THUAI7
from PyAPI.AI import Setting
from PyAPI.utils import THUAI72Proto
from PyAPI.Interface import IErrorHandler
import proto.Services_pb2_grpc as Services
import proto.Message2Clients_pb2 as Message2Clients
import threading
import grpc
import time
from typing import Union


class BoolErrorHandler(IErrorHandler):
    @staticmethod
    def result():
        return False


class Communication:
    def __init__(self, sIP: str, sPort: str):
        aim = sIP + ":" + sPort
        channel = grpc.insecure_channel(aim)
        self.__THUAI7Stub = Services.AvailableServiceStub(channel)
        self.__haveNewMessage = False
        self.__cvMessage = threading.Condition()
        self.__message2Client: Message2Clients.MessageToClient
        self.__mtxLimit = threading.Lock()
        self.__counter = 0
        self.__counterMove = 0
        self.__limit = 50
        self.__moveLimit = 10

    def Move(self, time: int, angle: float, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if (
                    self.__counter >= self.__limit
                    or self.__counterMove >= self.__moveLimit
                ):
                    return False
                self.__counter += 1
                self.__counterMove += 1
            moveResult: Message2Clients.MoveRes = self.__THUAI7Stub.Move(
                THUAI72Proto.THUAI72ProtobufMoveMsg(playerID, teamID, time, angle)
            )
        except grpc.RpcError:
            return False
        else:
            return moveResult.act_success

    def SendMessage(
        self, toID: int, message: Union[str, bytes], playerID: int, teamID: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            sendResult: Message2Clients.BoolRes = self.__THUAI7Stub.Send(
                THUAI72Proto.THUAI72ProtobufSendMsg(
                    playerID,
                    toID,
                    teamID,
                    message,
                    True if isinstance(message, bytes) else False,
                )
            )
        except grpc.RpcError:
            return False
        else:
            return sendResult.act_success

    def Attack(self, angle: float, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            attackResult: Message2Clients.BoolRes = self.__THUAI7Stub.Attack(
                THUAI72Proto.THUAI72ProtobufAttackMsg(playerID, teamID, angle)
            )
        except grpc.RpcError:
            return False
        else:
            return attackResult.act_success

    def Recover(self, playerID: int, teamID: int, recover: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            recoverResult: Message2Clients.BoolRes = self.__THUAI7Stub.Recover(
                THUAI72Proto.THUAI72ProtobufRecoverMsg(playerID, teamID, recover)
            )
        except grpc.RpcError:
            return False
        else:
            return recoverResult.act_success

    def Produce(self, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            produceResult: Message2Clients.BoolRes = self.__THUAI7Stub.Produce(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return produceResult.act_success

    def RepairWormhole(self, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            produceResult: Message2Clients.BoolRes = self.__THUAI7Stub.RepairWormhole(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return produceResult.act_success

    def RepairHome(self, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            produceResult: Message2Clients.BoolRes = self.__THUAI7Stub.RepairHome(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return produceResult.act_success

    def Rebuild(
        self, constructionType: THUAI7.ConstructionType, playerID: int, teamID: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            rebuildResult: Message2Clients.BoolRes = self.__THUAI7Stub.Rebuild(
                THUAI72Proto.THUAI72ProtobufConstructMsg(
                    playerID, teamID, constructionType
                )
            )
        except grpc.RpcError:
            return False
        else:
            return rebuildResult.act_success

    def Construct(
        self, constructionType: THUAI7.ConstructionType, playerID: int, teamID: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            constructResult: Message2Clients.BoolRes = self.__THUAI7Stub.Construct(
                THUAI72Proto.THUAI72ProtobufConstructMsg(
                    playerID, teamID, constructionType
                )
            )
        except grpc.RpcError:
            return False
        else:
            return constructResult.act_success

    def EndAllAction(self, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if (
                    self.__counter >= self.__limit
                    or self.__counterMove >= self.__moveLimit
                ):
                    return False
                self.__counter += 1
                self.__counterMove += 1
            endResult: Message2Clients.BoolRes = self.__THUAI7Stub.EndAllAction(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return endResult.act_success

    def SendMessage(
        self, toID: int, message: Union[str, bytes], playerID: int, teamID: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            sendResult: Message2Clients.BoolRes = self.__THUAI7Stub.Send(
                THUAI72Proto.THUAI72ProtobufSendMsg(
                    playerID,
                    toID,
                    teamID,
                    message,
                    True if isinstance(message, bytes) else False,
                )
            )
        except grpc.RpcError:
            return False
        else:
            return sendResult.act_success

    def InstallModule(
        self, moduleType: THUAI7.ModuleType, playerID: int, teamID: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            installResult: Message2Clients.BoolRes = self.__THUAI7Stub.InstallModule(
                THUAI72Proto.THUAI72ProtobufInstallMsg(playerID, teamID, moduleType)
            )
        except grpc.RpcError:
            return False
        else:
            return installResult.act_success

    def Recycle(self, playerID: int, teamID: int) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            recycleResult: Message2Clients.BoolRes = self.__THUAI7Stub.Recycle(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return recycleResult.act_success

    def BuildShip(
        self, teamID: int, shipType: THUAI7.ShipType, birthIndex: int
    ) -> bool:
        try:
            with self.__mtxLimit:
                if self.__counter >= self.__limit:
                    return False
                self.__counter += 1
            buildResult: Message2Clients.BoolRes = self.__THUAI7Stub.BuildShip(
                THUAI72Proto.THUAI72ProtobufBuildShipMsg(teamID, shipType, birthIndex)
            )
        except grpc.RpcError:
            return False
        else:
            return buildResult.act_success

    def TryConnection(self, playerID: int, teamID: int) -> bool:
        try:
            tryResult: Message2Clients.BoolRes = self.__THUAI7Stub.TryConnection(
                THUAI72Proto.THUAI72ProtobufIDMsg(playerID, teamID)
            )
        except grpc.RpcError:
            return False
        else:
            return tryResult.act_success

    def GetMessage2Client(self) -> Message2Clients.MessageToClient:
        with self.__cvMessage:
            self.__cvMessage.wait_for(lambda: self.__haveNewMessage)
            self.__haveNewMessage = False
            return self.__message2Client

    def AddPlayer(self, playerID: int, teamID: int, shipType: THUAI7.ShipType) -> None:
        def tMessage():
            try:
                if playerID == 0:
                    playerMsg = THUAI72Proto.THUAI72ProtobufPlayerMsg(
                        playerID, teamID, shipType
                    )
                    for msg in self.__THUAI7Stub.AddPlayer(playerMsg):
                        with self.__cvMessage:
                            self.__haveNewMessage = True
                            self.__message2Client = msg
                            self.__cvMessage.notify()
                            with self.__mtxLimit:
                                self.__counter = 0
                                self.__counterMove = 0
                elif playerID >= 1 and playerID <= 8:
                    playerMsg = THUAI72Proto.THUAI72ProtobufPlayerMsg(
                        playerID, teamID, shipType
                    )
                    for msg in self.__THUAI7Stub.AddPlayer(playerMsg):
                        with self.__cvMessage:
                            self.__haveNewMessage = True
                            self.__message2Client = msg
                            self.__cvMessage.notify()
                            with self.__mtxLimit:
                                self.__counter = 0
                                self.__counterMove = 0
            except grpc.RpcError:
                return

        threading.Thread(target=tMessage).start()
