from multiprocessing.shared_memory import ShareableList
from typing import overload

ShareMemName: str = 'THUAI7ShareMEM'


class ProcessEnv:
    @property
    def tID(self) -> int:
        return self.__sharedList[0]

    @property
    def sIP(self) -> str:
        return self.__sharedList[1]

    @property
    def sPort(self) -> str:
        return self.__sharedList[2]

    @property
    def file(self) -> bool:
        return self.__sharedList[3]

    @property
    def screen(self) -> bool:
        return self.__sharedList[4]

    @property
    def warnOnly(self) -> bool:
        return self.__sharedList[5]

    @overload
    def __init__(self, tID: int,
                 sIP: str, sPort: str,
                 file: bool, screen: bool, warnOnly: bool) -> None: ...

    @overload
    def __init__(self) -> None: ...

    def __init__(self, *args) -> None:
        if args == ():
            self.__sharedList = ShareableList(name=ShareMemName)
        else:
            self.__sharedList = ShareableList([args[0], args[1], args[2], args[3], args[4], args[5]], name=ShareMemName)
