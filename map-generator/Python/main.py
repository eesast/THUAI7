from __future__ import annotations
import os
import os.path
import sys

from easygui import multenterbox

import CLR_IMPORT  # NOQA: E402
import SETTINGS
from GameClass.MapGenerator import MapStruct
from Classes.MapRenderer import MapRenderer
from Classes.RandomCores.PerlinRandomCore import PerlinRandomCore
from Classes.RandomCores.XuchengRandomCore import XuchengRandomCore

# 命令行接口
if (argc := len(sys.argv)) != 1:
    match sys.argv[1]:
        # 生成器最小化
        case "--min":
            randomCore = XuchengRandomCore(SETTINGS.title)
            mapStruct = MapStruct(50, 50)
            randomCore.Random(mapStruct)
            if argc >= 3:
                MapStruct.ToFile(sys.argv[2], mapStruct)
            else:
                MapStruct.ToFile(f"demo{SETTINGS.file_suffix}", mapStruct)
            exit(0)

# 获取路径
path: str = multenterbox(msg='', title=SETTINGS.title, fields=[f'Path(*{SETTINGS.file_suffix})'])[0]
if path[-len(SETTINGS.file_suffix):] != SETTINGS.file_suffix:
    path += SETTINGS.file_suffix
# 地图加载
if not os.path.exists(path):
    height, width = [int(x)
                     for x in multenterbox(msg='Create new map', title=SETTINGS.title, fields=['Height', 'Width'])]
    mapStruct = MapStruct(height, width)
    MapStruct.ToFile(path, mapStruct)
else:
    mapStruct = MapStruct.FromFile(path)
# 随机核加载
randomCores = [XuchengRandomCore(SETTINGS.title), PerlinRandomCore(SETTINGS.title)]
# 地图渲染
mapRenderer = MapRenderer(SETTINGS.title, mapStruct, SETTINGS.areas, path, randomCores)
mapRenderer.MainFrame()
