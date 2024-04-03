import PyAPI.structures as THUAI7
from PyAPI.utils import AssistFunction
from typing import Union, Final, cast, List
from PyAPI.constants import Constants
from logic import Logic
from PyAPI.Space import ActionSpace, ObservatonSpace, Transition
import time


class gym:
    def __init__(self):
        self.memory = []
        self.logic1 = Logic(playerID=0, teamID=0)

    def push(self, *args):
        self.memory.append(Transition(*args))
