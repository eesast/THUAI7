from __future__ import annotations
import os
import os.path

from easygui import multenterbox

import CLR_IMPORT
import SETTINGS
from GameClass.MapGenerator import MapStruct
from Classes.MapRenderer import MapRenderer
from Classes.RandomCores.PerlinRandomCore import PerlinRandomCore
from Classes.RandomCores.XuchengRandomCore import XuchengRandomCore

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
