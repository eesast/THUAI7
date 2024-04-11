from typing import Final
import queue
import time

import PyAPI.structures as THUAI7
from PyAPI.Interface import IShipAPI, ITeamAPI, IAI
from PyAPI.utils import AssistFunction
from PyAPI.constants import Constants


class Setting:
    # 为假则play()期间确保游戏状态不更新，为真则只保证游戏状态在调用相关方法时不更新，大致一帧更新一次
    @staticmethod
    def Asynchronous() -> bool:
        return False


numOfGridPerCell: Final[int] = 1000


class AI(IAI):
    def __init__(self, pID: int):
        self.__playerID = pID

    def ShipPlay(self, api: IShipAPI) -> None:
        # 操作
        return

    def TeamPlay(self, api: ITeamAPI) -> None:
        # 操作
        return
