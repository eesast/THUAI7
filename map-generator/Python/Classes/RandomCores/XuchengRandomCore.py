from __future__ import annotations
from math import floor
from random import random

from easygui import multenterbox

from GameClass.MapGenerator import MapStruct
from Preparation.Utility import PlaceType as PT
from Classes.RandomCore import RandomCore


class DefaultXuchengRandomSettings:
    asteroidWidth = 2
    resourceNum = 7
    constructionNum = 5
    shadowProb = 0.015
    shadowCrossBonus = 23
    ruinProb = 0.01
    ruinCrossBonus = 40


class XuchengRandomCore(RandomCore):
    title: str
    asteroidWidth: int
    resourceNum: int
    constructionNum: int
    shadowProb: float
    shadowCrossBonus: int
    ruinProb: float
    ruinCrossBonus: int

    @property
    def AsteroidWidth(self) -> int:
        return self.asteroidWidth

    @AsteroidWidth.setter
    def AsteroidWidth(self, value: int) -> None:
        if value < 1 or value > 4:
            self.asteroidWidth = DefaultXuchengRandomSettings.asteroidWidth
        else:
            self.asteroidWidth = value

    @property
    def ResourceNum(self) -> int:
        return self.resourceNum

    @ResourceNum.setter
    def ResourceNum(self, value: int) -> None:
        if value < 1 or value > 10:
            self.resourceNum = DefaultXuchengRandomSettings.resourceNum
        else:
            self.resourceNum = value

    @property
    def ConstructionNum(self) -> int:
        return self.constructionNum

    @ConstructionNum.setter
    def ConstructionNum(self, value: int) -> None:
        if value < 1 or value > 10:
            self.constructionNum = DefaultXuchengRandomSettings.constructionNum
        else:
            self.constructionNum = value

    @property
    def ShadowProb(self) -> float:
        return self.shadowProb

    @ShadowProb.setter
    def ShadowProb(self, value: float) -> None:
        if value < 0 or value > 0.1:
            self.shadowProb = DefaultXuchengRandomSettings.shadowProb
        else:
            self.shadowProb = value

    @property
    def ShadowCrossBonus(self) -> int:
        return self.shadowCrossBonus

    @ShadowCrossBonus.setter
    def ShadowCrossBonus(self, value: int) -> None:
        if value < 1 or value > 50:
            self.shadowCrossBonus = DefaultXuchengRandomSettings.shadowCrossBonus
        else:
            self.shadowCrossBonus = value

    @property
    def RuinProb(self) -> float:
        return self.ruinProb

    @RuinProb.setter
    def RuinProb(self, value: float) -> None:
        if value < 0 or value > 0.1:
            self.ruinProb = DefaultXuchengRandomSettings.ruinProb
        else:
            self.ruinProb = value

    @property
    def RuinCrossBonus(self) -> int:
        return self.ruinCrossBonus

    @RuinCrossBonus.setter
    def RuinCrossBonus(self, value: int) -> None:
        if value < 1 or value > 50:
            self.ruinCrossBonus = DefaultXuchengRandomSettings.ruinCrossBonus
        else:
            self.ruinCrossBonus = value

    def __init__(self,
                 title,
                 asteroidWidth: int = DefaultXuchengRandomSettings.asteroidWidth,
                 resourceNum: int = DefaultXuchengRandomSettings.resourceNum,
                 constructionNum: int = DefaultXuchengRandomSettings.constructionNum,
                 shadowProb: float = DefaultXuchengRandomSettings.shadowProb,
                 shadowCrossBonus: int = DefaultXuchengRandomSettings.shadowCrossBonus,
                 ruinProb: float = DefaultXuchengRandomSettings.ruinProb,
                 ruinCrossBonus: int = DefaultXuchengRandomSettings.ruinCrossBonus) -> None:
        self.title = title
        self.AsteroidWidth = asteroidWidth
        self.ResourceNum = resourceNum
        self.ConstructionNum = constructionNum
        self.ShadowProb = shadowProb
        self.ShadowCrossBonus = shadowCrossBonus
        self.RuinProb = ruinProb
        self.RuinCrossBonus = ruinCrossBonus

    @property
    def Name(self) -> str:
        return 'Xucheng'

    def Menu(self) -> bool:
        try:
            (self.AsteroidWidth,
             self.ResourceNum,
             self.ConstructionNum,
             self.ShadowProb,
             self.ShadowCrossBonus,
             self.RuinProb,
             self.RuinCrossBonus) = (lambda i1, i2, i3, f4, i5, f6, i7:
                                     (int(i1), int(i2), int(i3), float(f4), int(i5), float(f6), int(i7)))(*multenterbox(
                                         msg='Random settings',
                                         title=self.title,
                                         fields=[
                                             'Asteroid 宽度',
                                             'Resource 数量',
                                             'Construction 数量',
                                             'Shadow 生成概率',
                                             'Shadow 蔓延加成',
                                             'Ruin 生成概率',
                                             'Ruin 蔓延加成'
                                         ],
                                         values=[self.AsteroidWidth,
                                                 self.ResourceNum,
                                                 self.ConstructionNum,
                                                 self.ShadowProb,
                                                 self.ShadowCrossBonus,
                                                 self.RuinProb,
                                                 self.RuinCrossBonus]
                                     ))
        except TypeError:
            return False
        return True

    def Random(self, mp: MapStruct) -> None:
        mp.Clear()
        XuchengRandomCore.generateBorderRuin(mp)
        XuchengRandomCore.generateHome(mp)
        XuchengRandomCore.generateAsteroid(mp, self.asteroidWidth)
        XuchengRandomCore.generateResource(mp, self.resourceNum)
        XuchengRandomCore.generateConstruction(mp, self.constructionNum)
        XuchengRandomCore.generateShadow(mp, self.shadowProb, self.shadowCrossBonus)
        XuchengRandomCore.generateRuin(mp, self.ruinProb, self.ruinCrossBonus)
        XuchengRandomCore.generateWormhole(mp)

    @staticmethod
    def isEmptyNearby(mp: MapStruct, x: int, y: int, r: int) -> bool:
        for i in range(x - r if x - r >= 0 else 0, (x + r if x + r <= 49 else 49) + 1):
            for j in range(y - r if y - r >= 0 else 0, (y - r if y + r <= 9 else 49) + 1):
                if mp[i, j] != PT.Null:
                    return False
        return True

    @staticmethod
    def haveSthNearby(mp: MapStruct, x: int, y: int, r: int, tp: PT) -> int:
        ret = 0
        for i in range(x - r if x - r >= 0 else 0, (x + r if x + r <= 49 else 49) + 1):
            for j in range(y - r if y - r >= 0 else 0, (y - r if y + r <= 9 else 49) + 1):
                if mp[i, j] == tp:
                    ret += 1
        return ret

    @staticmethod
    def haveSthCross(mp: MapStruct, x: int, y: int, r: int, tp: PT) -> int:
        ret = 0
        for i in range(x - r if x - r >= 0 else 0, (x + r if x + r <= 49 else 49) + 1):
            if mp[i, y] == tp:
                ret += 1
        for j in range(y - r if y - r >= 0 else 0, (y + r if y + r <= 49 else 49) + 1):
            if mp[x, j] == tp:
                ret += 1
        return ret

    @staticmethod
    def generateBorderRuin(mp: MapStruct) -> None:
        for i in range(50):
            mp[i, 0] = PT.Ruin
            mp[i, 49] = PT.Ruin
            mp[0, i] = PT.Ruin
            mp[49, i] = PT.Ruin

    @staticmethod
    def generateHome(mp: MapStruct) -> None:
        mp[3, 46] = PT.Home
        mp[46, 3] = PT.Home

    @staticmethod
    def generateAsteroid(mp: MapStruct, width: int = DefaultXuchengRandomSettings.asteroidWidth) -> None:
        for i in range(1, 49):
            for j in range(24, 24 - width, -1):
                mp[i, j] = PT.Asteroid
                mp[49 - i, 49 - j] = PT.Asteroid
        for i in range(1, 23):
            if random() < 0.5 and i != 9 and i != 10 and i != 11 and i != 12:
                mp[i, 24 - width] = PT.Asteroid
                mp[i, 24 + width] = PT.Null
                mp[49 - i, 25 + width] = PT.Asteroid
                mp[49 - i, 25 - width] = PT.Null

    @staticmethod
    def generateResource(mp: MapStruct, num: int = DefaultXuchengRandomSettings.resourceNum) -> None:
        i = 0
        while i < num:
            x = floor(random() * 48) + 1
            y = floor(random() * 23) + 1
            if XuchengRandomCore.isEmptyNearby(mp, x, y, 2):
                mp[x, y] = PT.Resource
                mp[49 - x, 49 - y] = PT.Resource
            else:
                i -= 1
            i += 1

    @staticmethod
    def generateConstruction(mp: MapStruct, num: int = DefaultXuchengRandomSettings.constructionNum) -> None:
        i = 0
        while i < num:
            x = floor(random() * 48) + 1
            y = floor(random() * 23) + 1
            if XuchengRandomCore.isEmptyNearby(mp, x, y, 1):
                mp[x, y] = PT.Construction
                mp[49 - x, 49 - y] = PT.Construction
            else:
                i -= 1
            i += 1

    @staticmethod
    def generateShadow(mp: MapStruct, prob: float = DefaultXuchengRandomSettings.shadowProb,
                       crossBonus: int = DefaultXuchengRandomSettings.shadowCrossBonus) -> None:
        for i in range(50):
            for j in range(50):
                if (mp[i, j] == PT.Null and
                        random() < prob * (XuchengRandomCore.haveSthCross(mp, i, j, 1, PT.Shadow) * crossBonus + 1)):
                    mp[i, j] = PT.Shadow
                    mp[49 - i, 49 - j] = PT.Shadow

    @staticmethod
    def generateRuin(mp: MapStruct, prob: float = DefaultXuchengRandomSettings.ruinProb,
                     crossBonus: int = DefaultXuchengRandomSettings.ruinCrossBonus) -> None:
        for i in range(2, 48):
            for j in range(2, 48):
                if ((mp[i, j] == PT.Null or mp[i, j] == PT.Shadow) and
                    not XuchengRandomCore.haveSthNearby(mp, i, j, 1, PT.Asteroid) and
                    not XuchengRandomCore.haveSthNearby(mp, i, j, 1, PT.Home) and
                        random() < prob
                        * (XuchengRandomCore.haveSthCross(mp, i, j, 1, PT.Ruin)
                           * (0 if XuchengRandomCore.haveSthCross(mp, i, j, 1, PT.Ruin) > 1
                              else crossBonus) + 1)):
                    mp[i, j] = PT.Ruin
                    mp[49 - i, 49 - j] = PT.Ruin

    @staticmethod
    def generateWormhole(mp: MapStruct) -> None:
        for i in range(1, 49):
            if mp[10, i] == PT.Asteroid:
                mp[10, i] = PT.Wormhole
                mp[39, 49 - i] = PT.Wormhole
            if mp[11, i] == PT.Asteroid:
                mp[11, i] = PT.Wormhole
                mp[38, 49 - i] = PT.Wormhole
            if mp[24, i] == PT.Asteroid:
                mp[24, i] = PT.Wormhole
                mp[25, 49 - i] = PT.Wormhole
