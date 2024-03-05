from __future__ import annotations
from abc import abstractmethod, abstractproperty

from GameClass.MapGenerator import MapStruct


class RandomCore:
    @abstractproperty
    def Name(self) -> str: ...

    @abstractmethod
    def Menu(self) -> bool: ...

    @abstractmethod
    def Random(self, mp: MapStruct) -> None: ...
