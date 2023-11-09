from __future__ import annotations
from io import TextIOWrapper
import json
import os.path

import easygui as eg

from AreaRenderDict import AreaRenderDict
from MapStruct import MapStruct
from MapRenderer import MapRenderer


# 加载设置
with open('./Settings.json', 'r', encoding='utf-8') as jsonfp:
    SETTINGS = json.load(jsonfp)
    TITLE: str = SETTINGS['title']
    FILE_SUFFIX: str = SETTINGS['file_suffix']
    AREAS: AreaRenderDict = AreaRenderDict(SETTINGS['areas'])
# 获取路径
path: str = eg.multenterbox(msg='', title=TITLE,
                            fields=[f'Path(*{FILE_SUFFIX})'])[0]
if path[-len(FILE_SUFFIX):] != FILE_SUFFIX:
    path += FILE_SUFFIX
# 地图加载
mapfile: TextIOWrapper
mapStruct: MapStruct
if not os.path.exists(path):
    height, width = [int(x) for x in eg.multenterbox(
        msg='Create new map', title=TITLE, fields=['Height', 'Width'])]
    mapStruct = MapStruct(height, width)
    mapStruct.ToFile(path)
else:
    mapfile = open(path, 'r+b')
    mapStruct = MapStruct(mapfile)
    mapfile.close()
# 地图渲染
mapRenderer = MapRenderer(mapStruct, AREAS, path)
mapRenderer.MainFrame()
