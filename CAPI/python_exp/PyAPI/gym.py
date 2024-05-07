from PyAPI.logic import Logic
from PyAPI.Space import Transition


class gym:
    def __init__(self):
        self.memory = []
        self.logic1 = Logic(playerID=0, teamID=0)

    def push(self, *args):
        self.memory.append(Transition(*args))
