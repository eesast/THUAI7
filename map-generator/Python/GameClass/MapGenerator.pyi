#!/usr/bin/env python
# -*- coding: utf-8 -*-

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
    def __init__(self, height: UInt32, width: UInt32) -> None: ...

    @overload
    def __init__(self, height: UInt32, width: UInt32, map: Array[PT]) -> None: ...

    @overload
    def __init__(self, height: UInt32, width: UInt32, map: Array[UInt32]) -> None: ...

    @staticmethod
    def FromFile(filename: String) -> MapStruct: ...

    @staticmethod
    def ToFile(filename: String, src: MapStruct) -> None: ...

    def Clear(self) -> None: ...

    def __getitem__(self, i: UInt32, j: UInt32) -> PT: ...

    def __setitem__(self, i: UInt32, j: UInt32, value: PT) -> None: ...
