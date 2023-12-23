from __future__ import annotations
from functools import cached_property


class AreaRenderDict:
    class Unit:
        name: str
        value: int
        color: str

        def __init__(self, _name: str, _value: int, _color: str) -> None:
            self.name, self.value, self.color = _name, _value, _color

    areas: list[Unit]

    @cached_property
    def Name2Unit(self) -> dict[str, Unit]:
        return {x.name: x for x in self.areas}

    @cached_property
    def Value2Unit(self) -> dict[int, Unit]:
        return {x.value: x for x in self.areas}

    @cached_property
    def Color2Unit(self) -> dict[str, Unit]:
        return {x.color: x for x in self.areas}

    @cached_property
    def Value2Color(self) -> dict[int, str]:
        return {x: self.Value2Unit[x].color for x in self.Value2Unit}

    def __init__(self, _areas: list[list[str, int, str]]) -> None:
        self.areas = [AreaRenderDict.Unit(_name, _value, _color)
                      for _name, _value, _color in _areas]
