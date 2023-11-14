from __future__ import annotations
from io import TextIOWrapper
import json
import os
import os.path

import easygui as eg

from Classes.AreaRenderDict import AreaRenderDict
from Classes.MapStruct import MapStruct
from Classes.MapRenderer import MapRenderer
from Classes.RandomCores.PerlinRandomCore import PerlinRandomCore
from Classes.RandomCores.СюйЧэнRandomCore import СюйЧэнRandomCore


# 查找设置
SETTINGS_PATH = ''
TARGET_SETTINGS_PATH = 'Settings.json'
for root, _, files in os.walk('.'):
    if TARGET_SETTINGS_PATH in files:
        SETTINGS_PATH = os.path.join(root, TARGET_SETTINGS_PATH)
if SETTINGS_PATH == '':
    raise FileNotFoundError('未找到设置文件')
# 加载设置
with open(SETTINGS_PATH, 'r', encoding='utf-8') as jsonfp:
    SETTINGS = json.load(jsonfp)
    TITLE: str = SETTINGS['title']
    FILE_SUFFIX: str = SETTINGS['file_suffix']
    DTYPE: str = SETTINGS['dtype']
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
    mapStruct = MapStruct(DTYPE, height, width)
    mapStruct.ToFile(path)
else:
    mapfile = open(path, 'r+b')
    mapStruct = MapStruct(DTYPE, mapfile)
    mapfile.close()
# 随机核加载
randomCores = [СюйЧэнRandomCore(TITLE), PerlinRandomCore(TITLE)]
# 地图渲染
mapRenderer = MapRenderer(TITLE, mapStruct, AREAS, path, randomCores)
mapRenderer.MainFrame()
