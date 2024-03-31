import PyAPI.structures as THUAI7
from PyAPI.Interface import ISweeperAPI, ITeamAPI, IAI
from PyAPI.utils import AssistFunction
from typing import Union, Final, cast, List
from PyAPI.constants import Constants
import queue
import time


class Setting:
    # 为假则play()期间确保游戏状态不更新，为真则只保证游戏状态在调用相关方法时不更新，大致一帧更新一次
    @staticmethod
    def Asynchronous() -> bool:
        return False

    @staticmethod
    def SweeperTypes() -> List[THUAI7.SweeperType]:
        return [
            THUAI7.SweeperType.CivilianSweeper,
            THUAI7.SweeperType.MilitarySweeper,
            THUAI7.SweeperType.MilitarySweeper,
            THUAI7.SweeperType.FlagSweeper,
        ]


numOfGridPerCell: Final[int] = 1000


class AI(IAI):
    def __init__(self, pID: int):
        self.__playerID = pID

    def SweeperPlay(self, api: ISweeperAPI) -> None:
        # 公共操作
        if self.__playerID == 1:
            api.PrintSelfInfo()
            time.sleep(1)
            # player1的操作
            return
        elif self.__playerID == 2:
            # player2的操作
            return
        elif self.__playerID == 3:
            # player3的操作
            return
        elif self.__playerID == 4:
            # player4的操作
            return
        returnF

    def TeamPlay(self, api: ITeamAPI) -> None:
        assert self.__playerID == 0, "Team's playerID must be 0"
        # 操作
        return
