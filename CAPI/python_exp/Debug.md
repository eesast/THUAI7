将 `RpcServices.cs` 中以下行修改:

```Git
@@ -155 +155 @@ namespace Server
- start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
+ start = Interlocked.Increment(ref playerCountNow) == 4;
@@ -164 +164 @@ namespace Server
- start = Interlocked.Increment(ref playerCountNow) == (playerNum * TeamCount);
+ start = Interlocked.Increment(ref playerCountNow) == 4;
```