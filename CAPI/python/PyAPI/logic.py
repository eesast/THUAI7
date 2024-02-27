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
from PyAPI.API import ShipAPI, TeamAPI
from PyAPI.AI import Setting
from PyAPI.Communication import Communication
from PyAPI.State import State
from PyAPI.Interface import ILogic, IGameTimer

class Logic(ILogic):
    def __init__(self,playerID:int,shipType:THUAI7.ShipType,teamID:int)->None:
        self.__playerID:int=playerID
        self.__teamID:int=teamID
        self.__shipType:THUAI7.ShipType=shipType

        self.__comm:Communication

        self.__currentState:State=State()
        self.__bufferState:State=State()

        self.__timer:IGameTimer

        self.__threadAI:threading.Thread

        self.__mtxState:threading.Lock=threading.Lock()

        self.__cvBuffer:threading.Condition=threading.Condition()
        self.__cvAI:threading.Condition=threading.Condition()

        self.__counterState:int=0
        self.__counterBuffer:int=0

        self.__gameState:THUAI7.GameState=THUAI7.GameState(0)

        self.__AILoop:bool=True

        self.__bufferUpdated:bool=False

        self.__AIStart:bool=False

        self.__freshed:bool=False

        self.__logger:logging.Logger=logging.getLogger("Logic")

        self.__messageQueue:Queue=Queue()

    def GetShips(self) -> List[THUAI7.Ship]:
        with self.__mtxState:
            self.__logger.debug("Called GetShips")
            return copy.deepcopy(self.__currentState.ships)
        
    def GetEnemyShips(self) -> List[THUAI7.Ship]:
        with self.__mtxState:
            self.__logger.debug("Called GetEnemyShips")
            return copy.deepcopy(self.__currentState.enemyShips)
        
    def GetBullets(self) -> List[THUAI7.Bullet]:
        with self.__mtxState:
            self.__logger.debug("Called GetBullets")
            return copy.deepcopy(self.__currentState.bullets)
        
    def GetSelfInfo(self)->Union[THUAI7.Ship,THUAI7.Team]:
        with self.__mtxState:
            self.__logger.debug("Called GetSelfInfo")
            return copy.deepcopy(self.__currentState.self)
        
    def GetFullMap(self) -> List[List[THUAI7.PlaceType]]:
        with self.__mtxState:
            self.__logger.debug("Called GetFullMap")
            return copy.deepcopy(self.__currentState.gameMap)
        
    def GetPlaceType(self,x:int,y:int)->THUAI7.PlaceType:
        with self.__mtxState:
            if x<0 or x>=len(self.__currentState.gameMap) or y<0 or y>=len(self.__currentState.gameMap[0]):
                self.__logger.warning("GetPlaceType: Out of range")
                return THUAI7.PlaceType(0)
            self.__logger.debug("Called GetPlaceType")
            return copy.deepcopy(self.__currentState.gameMap[x][y])
        
    def GetGameInfo(self)->THUAI7.GameInfo:
        with self.__mtxState:
            self.__logger.debug("Called GetGameInfo")
            return copy.deepcopy(self.__currentState.gameInfo)
        
    def Move(self,time:int,angle:float)->bool:
        self.__logger.debug("Called Move")
        return self.__comm.Move(time,angle,self.__playerID)
    
    def SendMessage(self,toID:int,message:Union[str,bytes])->bool:
        self.__logger.debug("Called SendMessage")
        return self.__comm.SendMessage(toID,message,self.__playerID,self.__teamID)
    
    def HaveMessage(self)->bool:
        self.__logger.debug("Called HaveMessage")
        return not self.__messageQueue.empty()
    
    def GetMessage(self)->Tuple[int,Union[str,bytes]]:
        self.__logger.debug("Called GetMessage")
        if self.__messageQueue.empty():
            self.__logger.warning("GetMessage: No message")
            return -1,""
        else:
            return self.__messageQueue.get()
        
    def WaitThread(self) -> bool:
        self.__Update()
        return True
    
    def GetCounter(self)->int:
        with self.__mtxState:
            return copy.deepcopy(self.__counterState)
        
    def GetPlayerGUIDs(self)->List[int]:
        with self.__mtxState:
            return copy.deepcopy(self.__currentState.guids)
        
    def GetConstructionHp(self, cellX: int, cellY: int) -> int:
        with self.__mtxState:
            if cellX<0 or cellX>=len(self.__currentState.gameMap) or cellY<0 or cellY>=len(self.__currentState.gameMap[0]):
                self.__logger.warning("GetConstructionHp: Out of range")
                return 0
            return copy.deepcopy(self.__currentState.constructionHp[cellX][cellY])