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
    dict_Name2Unit: dict[str, Unit]
    dict_Value2Unit: dict[int, Unit]
    dict_Color2Unit: dict[str, Unit]

    @cached_property
    def dict_Value2Color(self) -> dict[int, str]:
        return {x: self.Value2Unit(x).color for x in self.dict_Value2Unit}

    def __init__(self, _areas: list[list[str, int, str]]) -> None:
        self.areas = []
        self.dict_Name2Unit = {}
        self.dict_Value2Unit = {}
        self.dict_Color2Unit = {}
        for _name, _value, _color in _areas:
            cur = AreaRenderDict.Unit(_name, _value, _color)
            self.areas.append(cur)
            self.dict_Name2Unit[_name] = cur
            self.dict_Value2Unit[_value] = cur
            self.dict_Color2Unit[_color] = cur

    def Name2Unit(self, _name: str) -> AreaRenderDict.Unit:
        return self.dict_Name2Unit[_name]

    def Value2Unit(self, _value: int) -> AreaRenderDict.Unit:
        return self.dict_Value2Unit[_value]

    def Color2Unit(self, _color: str) -> AreaRenderDict.Unit:
        return self.dict_Color2Unit[_color]
