# Stub for C#

from __future__ import annotations
from typing import overload

from System import Array, UInt32, String
from Preparation.Utility import PlaceType as PT


class MapStruct:
    height: UInt32
    width: UInt32
    map: Array[PT]

    @overload
    def MapStruct(self, height: UInt32, width: UInt32) -> None: ...

    @overload
    def MapStruct(self, height: UInt32, width: UInt32, map: Array[PT]) -> None: ...

    @overload
    def MapStruct(self, height: UInt32, width: UInt32, map: Array[UInt32]) -> None: ...

    @staticmethod
    def FromFile(filename: String) -> MapStruct: ...

    @staticmethod
    def ToFile(filename: String, src: MapStruct) -> None: ...
