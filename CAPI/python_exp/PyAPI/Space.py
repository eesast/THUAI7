from typing import Optional, List, Sequence
from collections import namedtuple

import numpy as np

import PyAPI.structures as THUAI7
from PyAPI.State import State

Transition = namedtuple("Transition", ["state", "action", "reward", "next_state"])


class SingleObservation:
    def __init__(self, state: State):
        if isinstance(state.self, THUAI7.Ship):
            self.selfShipInfo = np.array(self.__ObsShip(state.self))
            self.otherShipInfo = np.array([self.__ObsShip(ship)
                                           for ship in state.ships
                                           if ship.playerID != state.self.playerID])
            self.bulletsInfo = np.array([self.__ObsBullet(bullet)
                                         for bullet in state.bullets])
            self.homeInfo = np.array([[*key, *value]
                                      for key, value in state.mapInfo.homeState.items()])
        elif isinstance(state.self, THUAI7.Team):
            self.shipInfo = np.array([self.__ObsShip(ship)
                                      for ship in state.ships])
        self.enemyShipInfo = np.array([self.__ObsShip(ship)
                                       for ship in state.enemyShips])
        self.factoryInfo = np.array([[*key, *value]
                                     for key, value in state.mapInfo.factoryState.items()])
        self.communityInfo = np.array([[*key, *value]
                                       for key, value in state.mapInfo.communityState.items()])
        self.fortInfo = np.array([[*key, *value]
                                  for key, value in state.mapInfo.communityState.items()])
        self.wormholeInfo = np.array([[*key, value]
                                      for key, value in state.mapInfo.wormholeState.items()])
        self.resourceInfo = np.array([[*key, value]
                                      for key, value in state.mapInfo.resourceState.items()])
        self.gameInfo = np.array(self.__ObsGame(state.gameInfo))

    def __ObsShip(ship: THUAI7.Ship):
        return [ship.x,
                ship.y,
                ship.speed,
                ship.facingDirection,
                ship.viewRange,
                ship.hp,
                ship.armor,
                ship.shield,
                ship.shipState.value,
                ship.shipType.value,
                ship.producerType.value,
                ship.constructorType.value,
                ship.armorType.value,
                ship.shieldType.value,
                ship.weaponType.value]

    def __ObsBullet(bullet: THUAI7.Bullet):
        return [bullet.x,
                bullet.y,
                bullet.facingDirection,
                bullet.speed,
                bullet.damage,
                bullet.bombRange,
                bullet.explodeRange]

    def __ObsGame(gameInfo: THUAI7.GameInfo):
        return [[gameInfo.redHomeHp, gameInfo.redEnergy, gameInfo.redScore],
                [gameInfo.blueHomeHp, gameInfo.blueEnergy, gameInfo.blueScore]]


class ObservatonSpace:
    def __init__(self, state: List[State]):
        assert len(state) == 5
        self.teamObs = SingleObservation(state[0])
        self.ship_1_Obs = SingleObservation(state[1])
        self.ship_2_Obs = SingleObservation(state[2])
        self.ship_3_Obs = SingleObservation(state[3])
        self.ship_4_Obs = SingleObservation(state[4])


class ShipAction:
    def __init__(self, action: int, angle: Optional[float] = None):
        assert action in range(0, 16), "ship action out of range"
        self.action = action
        self.attackAngle = angle


class ActionSpace:
    def __init__(self, teamAction: Sequence[int], shipsAction: Sequence[ShipAction]):
        assert (len(teamAction) == 0
                and (teamAction[0] in range(0, 14))
                and (teamAction[1] in range(0, 19)))
        assert len(shipsAction) == 4
        self.teamAction = teamAction
        self.shipsAction = shipsAction
        # move
        # attack  angle
        # recover
        # produce
        # rebuild constructiontype
        # constrct constructiontype
        # wait
        # endallaction
        # wait
        # endall
        # install
        # recycle
        # buildship
