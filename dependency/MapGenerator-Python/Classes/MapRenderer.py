from __future__ import annotations

import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.axes import Axes
from matplotlib.patches import Rectangle
from matplotlib.ticker import MultipleLocator
from matplotlib.backend_bases import MouseEvent, KeyEvent

from Classes.AreaRenderDict import AreaRenderDict
from MapStruct import MapStruct


class MapRenderer:
    map: MapStruct
    areaRender: AreaRenderDict
    mapf: str
    fig: Figure
    ax: Axes
    rects: list[list[Rectangle]]

    __curMax: int
    __cur: int

    @property
    def cur(self) -> str: ...

    @cur.getter
    def cur(self) -> str:
        return self.areaRender.areas[self.__cur].color

    @cur.setter
    def cur(self, _) -> None:
        self.__cur += 1
        self.__cur %= self.__curMax

    def __init__(self, _mapStruct: MapStruct, _areas: AreaRenderDict, _mapf: str) -> None:
        self.map = _mapStruct
        self.areaRender = _areas
        self.mapf = _mapf
        self.fig, self.ax = plt.subplots()
        self.rects = [[Rectangle((j, i), 1, 1, facecolor=self.areaRender.areas[self.map[i, j]].color)
                       for j in range(self.map.width)]
                      for i in range(self.map.height)]
        self.__curMax = len(self.areaRender.areas)
        self.__cur = 0

    def MainFrame(self) -> None:
        self.fig.set_size_inches(self.map.width, self.map.height)
        self.ax.set_xlim(0, self.map.width)
        self.ax.set_ylim(self.map.height, 0)
        self.ax.xaxis.set_major_locator(MultipleLocator(1))
        self.ax.yaxis.set_major_locator(MultipleLocator(1))
        self.ax.set_aspect(1)
        for i in range(self.map.height):
            for j in range(self.map.width):
                self.ax.add_patch(self.rects[i][j])
        plt.hlines(range(self.map.height + 1), 0, self.map.width)
        plt.vlines(range(self.map.width + 1), 0, self.map.height)
        self.fig.canvas.mpl_connect('button_press_event', self.on_click)
        self.fig.canvas.mpl_connect('key_press_event', self.on_press)
        plt.show()

    def on_click(self, event: MouseEvent) -> None:
        if not event.button:
            return
        match event.button:
            case 1:
                r, c = int(event.ydata), int(event.xdata)
                self.map[r, c] = self.areaRender.areas[self.__cur].value
                self._render(r, c)
            case 3:
                self.cur = 0
            case _:
                return

    def on_press(self, event: KeyEvent) -> None:
        if not event.key:
            return
        match event.key:
            case 'c':
                self.map.ToFile(self.mapf)

    def _render(self, r: int, c: int) -> None:
        self.rects[r][c].set_color(self.cur)
        self.ax.draw_artist(self.rects[r][c])
        plt.show(block=False)
