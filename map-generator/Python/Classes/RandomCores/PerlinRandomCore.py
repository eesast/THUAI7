from __future__ import annotations
from typing import Any

from easygui import multenterbox
from perlin_noise import PerlinNoise

from GameClass.MapGenerator import MapStruct
from Preparation.Utility import PlaceType as PT
from Classes.RandomCore import RandomCore


class PerlinRandomCore(RandomCore):
    title: str
    octaves: float
    seed: int | None

    @property
    def Octaves(self) -> float:
        return self.octaves

    @Octaves.setter
    def Octaves(self, value: float) -> None:
        if value > 0:
            self.octaves = value
        else:
            self.octaves = 10

    @property
    def Seed(self) -> int | None:
        return self.seed

    @Seed.setter
    def Seed(self, value: int | Any) -> None:
        if isinstance(value, int) and value > 0:
            self.seed = value
        else:
            self.seed = None

    def __init__(self, title: str, octaves: float = 10) -> None:
        self.title = title
        self.Octaves = octaves
        self.Seed = None

    @property
    def Name(self) -> str:
        return 'Perlin'

    def Menu(self) -> bool:
        try:
            (self.Octaves,
             self.Seed) = (lambda f1, i2: (float(f1), int(i2) if i2 else i2))(*multenterbox(
                 msg='Random settings',
                 title=self.title,
                 fields=[
                     'Octaves',
                     'Seed'
                 ],
                 values=[
                     self.Octaves,
                     self.Seed
                 ]
             ))
        except TypeError:
            return False
        return True

    def Random(self, mp: MapStruct) -> None:
        noise = PerlinNoise(self.Octaves, self.Seed)
        arr: list[list[float]] = []
        arrMax: float = float('-inf')
        arrMin: float = float('inf')
        for i in range(mp.height):
            arr.append([])
            for j in range(mp.width):
                cur = noise([j / mp.width, i / mp.height])
                arr[i].append(cur)
                if cur > arrMax:
                    arrMax = cur
                elif cur < arrMin:
                    arrMin = cur
        arr0, arr1, arr2 = arrMin, (2 * arrMin + arrMax) / 3, (arrMin + 2 * arrMax) / 3
        for i in range(mp.height):
            for j in range(mp.width):
                if i == 0 or i == mp.height - 1:
                    mp[i, j] = PT.Ruin
                    continue
                elif j == 0 or j == mp.width - 1:
                    mp[i, j] = PT.Ruin
                    continue
                cur = arr[i][j]
                if arr0 < cur < arr1:
                    mp[i, j] = PT.Shadow
                elif arr1 < cur < arr2:
                    mp[i, j] = PT.Null
                else:
                    mp[i, j] = PT.Ruin
