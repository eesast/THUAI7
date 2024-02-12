import CLR_START
import clr
import sys
try:
    sys.path.append(r'../../logic/Preparation/bin/Release/net8.0/')
    clr.AddReference('Preparation')
except BaseException:
    try:
        sys.path.append(r'../../logic/Preparation/bin/Debug/net8.0/')
        clr.AddReference('Preparation')
    except BaseException:
        raise FileNotFoundError('Preparation项目未正确编译')
