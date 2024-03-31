from collections import deque, namedtuple
import random
import PyAPI.structures as THUAI7
from PyAPI.State import State

Transition = namedtuple("Transition", ["state", "action", "reward", "next_state"])


class ObservationSpace:
    def __init__(self, state: State):
        if isinstance(state.self, THUAI7.Sweeper):
            self.selfSweeperInfo = self.__ObsSweeper(state.self)
            self.otherSweeperInfo = [
                self.__ObsSweeper(sweeper)
                for sweeper in state.sweepers
                if sweeper.playerID != state.self.playerID
            ]
            self.bulletsInfo = [self.__ObsBullet(bullet) for bullet in state.bullets]
            self.homeInfo = [
                [*key, *value] for key, value in state.mapInfo.homeState.items()
            ]
        elif isinstance(state.self, THUAI7.Team):
            self.sweeperInfo = [
                self.__ObsSweeper(sweeper) for sweeper in state.sweepers
            ]
        self.enemySweeperInfo = [
            self.__ObsSweeper(sweeper) for sweeper in state.enemySweepers
        ]
        self.recycleBankInfo = [
            [*key, *value] for key, value in state.mapInfo.recycleBankState.items()
        ]
        self.chargeStationInfo = [
            [*key, *value] for key, value in state.mapInfo.chargeStationState.items()
        ]
        self.signalTowerInfo = [
            [*key, *value] for key, value in state.mapInfo.chargeStationState.items()
        ]
        self.bridgeInfo = [
            [*key, value] for key, value in state.mapInfo.bridgeState.items()
        ]
        self.garbageInfo = [
            [*key, value] for key, value in state.mapInfo.garbageState.items()
        ]
        self.gameInfo = self.__ObsGame(state.gameInfo)

    def __ObsSweeper(sweeper: THUAI7.Sweeper):
        return [
            sweeper.x,
            sweeper.y,
            sweeper.speed,
            sweeper.facingDirection,
            sweeper.viewRange,
            sweeper.hp,
            sweeper.armor,
            sweeper.shield,
            sweeper.sweeperState.value,
            sweeper.sweeperType.value,
            sweeper.producerType.value,
            sweeper.constructorType.value,
            sweeper.armorType.value,
            sweeper.shieldType.value,
            sweeper.weaponType.value,
        ]

    def __ObsBullet(bullet: THUAI7.Bullet):
        return [
            bullet.x,
            bullet.y,
            bullet.facingDirection,
            bullet.speed,
            bullet.damage,
            bullet.bombRange,
            bullet.explodeRange,
        ]

    def __ObsGame(gameInfo: THUAI7.GameInfo):
        return [
            [gameInfo.redHomeHp, gameInfo.redEnergy, gameInfo.redScore],
            [gameInfo.blueHomeHp, gameInfo.blueEnergy, gameInfo.blueScore],
        ]


class ActionSpace:
    def __init__(self):
        pass


class Memory:
    def __init__(self, max_len):
        self.memory = deque([], maxlen=max_len)

    def push(self, *args):
        self.memory.append(Transition(*args))

    def sample(self, batch_size):
        return random.sample(self.memory, batch_size)

    def __len__(self):
        return len(self.memory)
