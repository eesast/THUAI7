from __future__ import annotations
from queue import Queue

from easygui import choicebox
import matplotlib.pyplot as plt
from matplotlib.figure import Figure
from matplotlib.axes import Axes
from matplotlib.patches import Rectangle
from matplotlib.ticker import MultipleLocator
from matplotlib.backend_bases import MouseEvent, KeyEvent

from Classes.AreaRenderDict import AreaRenderDict
from Classes.MapStruct import MapStruct
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

    title: str
    map: MapStruct
    areaRender: AreaRenderDict
    mapf: str
    fig: Figure
    ax: Axes
    rects: list[list[Rectangle]]
    queue_render: Queue[RenderUnit]
    randomCores: list[RandomCore]

    isCursorLift: int  # 0: 未提起; 1: 提起未选定; 2: 提起并选定
    cursorLift: tuple[int]  # 提起坐标

    __curMax: int
    __cur: int

    @property
    def Cur(self) -> str:
        return self.areaRender.areas[self.__cur].color

    @Cur.setter
    def Cur(self, _) -> None:
        self.__cur += 1
        self.__cur %= self.__curMax

    @property
    def Queue_Render(self) -> RenderUnit:
        return self.queue_render.get(timeout=0.1)

    @Queue_Render.setter
    def Queue_Render(self, value: RenderUnit) -> None:
        self.queue_render.put(value, timeout=0.1)

    def __init__(self, _title, _mapStruct: MapStruct, _areas: AreaRenderDict,
                 _mapf: str, _randoms: list[RandomCore]) -> None:
        self.title = _title
        self.map = _mapStruct
        self.areaRender = _areas
        self.mapf = _mapf
        self.randomCores = _randoms
        self.fig, self.ax = plt.subplots()
        self.rects = [[Rectangle((j, i), 1, 1, facecolor=self.areaRender.areas[self.map[i, j]].color)
                       for j in range(self.map.width)]
                      for i in range(self.map.height)]
        self.queue_render = Queue()
        self.isCursorLift = 0
        self.cursorLift = None
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
                match self.isCursorLift:
                    case 0:
                        self.map[r, c] = self.areaRender.Color2Unit[self.Cur].value
                        self.Queue_Render = MapRenderer.RenderUnit(r, c, self.Cur)
                        self.Render()
                    case 1:
                        self.cursorLift = (r, c)
                        self.isCursorLift = 2
                    case 2:
                        liftr, liftc = self.cursorLift
                        dir_r, dir_c = (1 if liftr <= r else -1), (1 if liftc <= c else -1)
                        for i in range(liftr, r + dir_r, dir_r):
                            for j in range(liftc, c + dir_c, dir_c):
                                self.map[i, j] = self.areaRender.Color2Unit[self.Cur].value
                                self.Queue_Render = MapRenderer.RenderUnit(i, j, self.Cur)
                        self.Render()
                        self.isCursorLift = 0
            case 3:
                self.Cur = 0
            case _:
                return

    def on_press(self, event: KeyEvent) -> None:
        if not event.key:
            return
        match event.key:
            case 'z':
                self.isCursorLift = 1
            case 'c':
                self.map.ToFile(self.mapf)
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
                                    self.Queue_Render = MapRenderer.RenderUnit(
                                        r, c, self.areaRender.Value2Color[self.map[r, c]])
                            self.Render()
                            plt.show(block=False)
            case _:
                return

    def Render(self) -> None:
        while not self.queue_render.empty():
            cur = self.Queue_Render
            self._render(cur.r, cur.c, cur.tp)

    def _render(self, r: int, c: int, tp: str) -> None:
        self.rects[r][c].set_color(tp)
        self.ax.draw_artist(self.rects[r][c])
        plt.show(block=False)
