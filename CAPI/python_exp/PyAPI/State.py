from typing import List, Union

import PyAPI.structures as THUAI7


class State:
    def __init__(self) -> None:
        self.teamScore = 0
        self.self = THUAI7.Ship()
        self.ships = []
        self.enemyShips = []
        self.gameMap = []
        self.bullets = []
        self.bombedBullets = []
        self.mapInfo = THUAI7.GameMap()
        self.gameInfo = THUAI7.GameInfo()
        self.guids = []

    teamScore: int
    self: Union[THUAI7.Ship, THUAI7.Team]

    ships: List[THUAI7.Ship]
    enemyShips: List[THUAI7.Ship]

    gameMap: List[List[THUAI7.PlaceType]]

    bullets: List[THUAI7.Bullet]
    # bombedBullets: List[THUAI7.BombedBullet]

    mapInfo: THUAI7.GameMap

    gameInfo: THUAI7.GameInfo

    guids: List[int]
