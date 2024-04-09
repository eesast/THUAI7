将 `RpcServices.cs` 中以下行修改，可以改为动态多进程:

```Git
@@ -155 +155 @@ namespace Server
- start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
+ start = Interlocked.Increment(ref playerCountNow) == 4;
@@ -164 +164 @@ namespace Server
- start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
+ start = Interlocked.Increment(ref playerCountNow) == 4;
```

另外，需要将 `Communication.py` 的 `BuildShip` 加上以下行：

```Git
@@ +201 @@ class Logic(ILogic):
+           if buildResult.act_success:
+               Process(target=self.__start,
+                       args=(buildResult.player_id, THUAI7.PlayerType.Ship, shipType, self.__processEnv.shmName)).start()
```

以及重新设置一下 `main.py` 的初始进程.