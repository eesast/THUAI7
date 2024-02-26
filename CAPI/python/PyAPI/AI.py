import PyAPI.structures as THUAI7
from PyAPI.Interface import IShipAPI, ITeamAPI, IAI
from typing import Union, Final, cast, List
from PyAPI.constants import Constants
import queue

import time

class Setting:
    # 为假则play()期间确保游戏状态不更新，为真则只保证游戏状态在调用相关方法时不更新，大致一帧更新一次
    @staticmethod
    def asynchronous() -> bool:
        return False
    
numOfGridPerCell: Final[int] = 1000

class AssistFunction:
    @staticmethod
    def CellToGrid(cell: int) -> int:
        return cell*numOfGridPerCell+numOfGridPerCell//2
    
    @staticmethod
    def GridToCell(grid: int) -> int:
        return grid//numOfGridPerCell
    
class AI(IAI):
    def __init__(self,pID:int):
        self.__playerID=pID

    def ShipPlay(self,api:IShipAPI)->None:
        #公共操作

        if self.__playerID==0:
            #player0的操作
            return
        elif self.__playerID==1:
            #player1的操作
            return
        elif self.__playerID==2:
            #player2的操作
            return
        return
    
    def TeamPlay(self,api:ITeamAPI)->None:
        #操作
        return