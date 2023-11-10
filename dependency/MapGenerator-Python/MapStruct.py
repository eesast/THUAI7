from __future__ import annotations
from functools import cached_property
from io import TextIOWrapper
from typing import Literal, overload
from array import array


class MapStruct:
    __arrAlloc: array[int]

    @cached_property
    def width(self) -> int:
        return self.__arrAlloc[1]

    @cached_property
    def height(self) -> int:
        return self.__arrAlloc[0]

    @property
    def ArrayView(self) -> list[list[int]]:
        return [[self[i, j] for j in range(self.width)]for i in range(self.height)]

    def __getitem__(self, rowcol: tuple[int, int]) -> int:
        return self.__arrAlloc[2+rowcol[1]+rowcol[0]*self.width]

    def __setitem__(self, rowcol: tuple[int, int], val: int) -> None:
        self.__arrAlloc[2+rowcol[1]+rowcol[0]*self.width] = val

    @overload
    def __init__(self, dtype: Literal['b', 'B', 'u', 'h', 'H', 'i', 'I', 'l', 'L', 'q', 'Q', 'f', 'd'],
                 height: int, width: int) -> None: ...

    @overload
    def __init__(self, dtype: Literal['b', 'B', 'u', 'h', 'H', 'i', 'I', 'l', 'L', 'q', 'Q', 'f', 'd'],
                 mapFile: TextIOWrapper) -> None: ...

    def __init__(self, dtype: Literal['b', 'B', 'u', 'h', 'H', 'i', 'I', 'l', 'L', 'q', 'Q', 'f', 'd'],
                 *args) -> None:
        self.__arrAlloc = array(dtype)
        if len(args) == 1:
            mapFile = args[0]
            self.__arrAlloc.fromfile(mapFile, 2)
            self.__arrAlloc.fromfile(mapFile, self.width*self.height)
        else:
            self.__arrAlloc.append(args[0])
            self.__arrAlloc.append(args[1])
            self.__arrAlloc.fromlist(
                [0 for _ in range(self.width*self.height)])

    def ToFile(self, path: str) -> None:
        with open(path, 'wb') as fp:
            self.__arrAlloc.tofile(fp)
