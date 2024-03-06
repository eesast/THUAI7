import CLR_START
import clr
import sys
clr.AddReference('System')
PREPARATION_IMPORT = [r'../../logic/Preparation/bin/Debug/net8.0/',
                      r'../../logic/Preparation/bin/Release/net8.0/',
                      r'../../../logic/Preparation/bin/Debug/net8.0/',
                      r'../../../logic/Preparation/bin/Release/net8.0/',
                      r'./',
                      r'./_internal/']
PREPARATION = 'Preparation'
GAMECLASS_IMPORT = [r'../../logic/GameClass/bin/Debug/net8.0/',
                    r'../../logic/GameClass/bin/Release/net8.0/',
                    r'../../../logic/GameClass/bin/Debug/net8.0/',
                    r'../../../logic/GameClass/bin/Release/net8.0/',
                    r'./',
                    r'./_internal/']
GAMECLASS = 'GameClass'


def Find(paths: list[str], target: str) -> None:
    try:
        sys.path.append(paths[0])
        clr.AddReference(target)
    except BaseException:
        paths.pop(0)
        if paths == []:
            raise FileNotFoundError(f'未找到{target}')
        Find(paths, target)


Find(PREPARATION_IMPORT, PREPARATION)
Find(GAMECLASS_IMPORT, GAMECLASS)
