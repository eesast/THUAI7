from __future__ import annotations
from queue import Queue
from typing import Any, Generator, NoReturn

from easygui import choicebox, msgbox
import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.axes import Axes
from matplotlib.patches import Rectangle
from matplotlib.ticker import MultipleLocator
from matplotlib.backend_bases import MouseEvent, KeyEvent

from System import String
from GameClass.MapGenerator import MapStruct
from Preparation.Utility import PlaceType as PT
from Classes.RandomCore import RandomCore


class MapRenderer:
    class RenderUnit:
        r: int
        c: int
        tp: str

        def __init__(self, _r: int, _c: int, _tp: str) -> None:
            self.r = _r
            self.c = _c
            self.tp = _tp

    class CurrentRender:
        __cur_gen: Generator[PT, Any, NoReturn]
        __cur: PT

        def __init__(self, _areas: dict[PT, str]) -> None:
            _keys = list(_areas.keys())
            self.__cur = _keys[0]

            def g():
                while True:
                    for k in _keys:
                        yield k
            self.__cur_gen = g()

        def Get(self) -> PT:
            return self.__cur

        def Switch(self) -> None:
            self.__cur = next(self.__cur_gen)

    title: str
    map: MapStruct
    areas: dict[PT, str]
    mapf: String
    fig: Figure
    ax: Axes
    rects: list[list[Rectangle]]
    queue_render: Queue[RenderUnit]
    randomCores: list[RandomCore]

    isCursorLift: int  # 0: 未提起; 1: 提起未选定; 2: 提起并选定
    cursorLift: tuple[int]  # 提起坐标

    cur: CurrentRender

    @property
    def Queue_Render(self) -> RenderUnit:
        return self.queue_render.get(timeout=0.1)

    @Queue_Render.setter
    def Queue_Render(self, value: RenderUnit) -> None:
        self.queue_render.put(value, timeout=0.1)

    def __init__(self, _title, _mapStruct: MapStruct, _areas: dict[PT, str],
                 _mapf: str, _randoms: list[RandomCore]) -> None:
        self.title = _title
        self.map = _mapStruct
        self.areas = _areas
        self.mapf = _mapf
        self.randomCores = _randoms
        self.fig, self.ax = plt.subplots()
        self.rects = [[Rectangle((j, i), 1, 1, facecolor=self.areas[self.map[i, j]])
                       for j in range(self.map.width)]
                      for i in range(self.map.height)]
        self.queue_render = Queue()
        self.isCursorLift = 0
        self.cursorLift = None
        self.cur = MapRenderer.CurrentRender(_areas)

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
                match self.isCursorLift:
                    case 0:
                        self.map[r, c] = self.cur.Get()
                        self.Queue_Render = MapRenderer.RenderUnit(r, c, self.areas[self.cur.Get()])
                        self.Render()
                    case 1:
                        self.cursorLift = (r, c)
                        self.isCursorLift = 2
                    case 2:
                        liftr, liftc = self.cursorLift
                        dir_r, dir_c = (1 if liftr <= r else -1), (1 if liftc <= c else -1)
                        for i in range(liftr, r + dir_r, dir_r):
                            for j in range(liftc, c + dir_c, dir_c):
                                self.map[i, j] = self.cur.Get()
                                self.Queue_Render = MapRenderer.RenderUnit(i, j, self.areas[self.cur.Get()])
                        self.Render()
                        self.isCursorLift = 0
            case 3:
                self.cur.Switch()
            case _:
                return

    def on_press(self, event: KeyEvent) -> None:
        if not event.key:
            return
        match event.key:
            case 'z':
                self.isCursorLift = 1
            case 'c':
                MapStruct.ToFile(self.mapf, self.map)
                msgbox(msg='Your map has been saved.', title=self.title)
            case 'p':
                opt = choicebox(msg='Choose random', title=self.title, choices=[x.Name for x in self.randomCores])
                if opt is None:
                    return
                for x in self.randomCores:
                    if x.Name == opt:
                        if x.Menu():
                            x.Random(self.map)
                            for r in range(self.map.height):
                                for c in range(self.map.width):
                                    self.Queue_Render = MapRenderer.RenderUnit(r, c, self.areas[self.map[r, c]])
                            self.Render()
            case _:
                return

    def Render(self) -> None:
        while not self.queue_render.empty():
            cur = self.Queue_Render
            self._render(cur.r, cur.c, cur.tp)
        plt.show(block=False)

    def _render(self, r: int, c: int, tp: str) -> None:
        self.rects[r][c].set_color(tp)
        self.ax.draw_artist(self.rects[r][c])
