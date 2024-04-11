# Client 调试客户端使用指南

## 1. 启动 Server
可使用命令行或 Visual Studio 启动 Server，启动后会监听本地 8888 端口。
相关命令行参数如下所示，可在 `\THUAI7\logic\Server\ArgumentOptions.cs` 中查看。

```Csharp
public class ArgumentOptions
{
    [Option("ip", Required = false, HelpText = "Server listening ip")]
    public string ServerIP { get; set; } = "localhost";

    [Option('p', "port", Required = true, HelpText = "Server listening port")]
    public ushort ServerPort { get; set; } = 8888;

    [Option("teamCount", Required = false, HelpText = "The number of teams, 2 by defualt")]
    public ushort TeamCount { get; set; } = 2;

    [Option("shipNum", Required = false, HelpText = "The max number of Ship, 4 by default")]
    public ushort ShipCount { get; set; } = 4;

    [Option("homeNum", Required = false, HelpText = "The number of Home , 1 by default")]
    public ushort HomeCount { get; set; } = 1;

    [Option('g', "gameTimeInSecond", Required = false, HelpText = "The time of the game in second, 10 minutes by default")]
    public uint GameTimeInSecond { get; set; } = 10 * 60;
    [Option('f', "fileName", Required = false, HelpText = "The file to store playback file or to read file.")]
    public string FileName { get; set; } = "114514";
    [Option("notAllowSpectator", Required = false, HelpText = "Whether to allow a spectator to watch the game.")]
    public bool NotAllowSpectator { get; set; } = false;
    [Option('b', "playback", Required = false, HelpText = "Whether open the server in a playback mode.")]
    public bool Playback { get; set; } = false;
    [Option("playbackSpeed", Required = false, HelpText = "The speed of the playback, between 0.25 and 4.0")]
    public double PlaybackSpeed { get; set; } = 1.0;
    [Option("resultOnly", Required = false, HelpText = "In playback mode to get the result directly")]
    public bool ResultOnly { get; set; } = false;
    [Option('k', "token", Required = false, HelpText = "Web API Token")]
    public string Token { get; set; } = "114514";
    [Option('u', "url", Required = false, HelpText = "Web Url")]
    public string Url { get; set; } = "114514";
    [Option('m', "mapResource", Required = false, HelpText = "Map Resource Path")]
    public string MapResource { get; set; } = DefaultArgumentOptions.MapResource;
    [Option("requestOnly", Required = false, HelpText = "Only send web requests")]
    public bool RequestOnly { get; set; } = false;
    [Option("finalGame", Required = false, HelpText = "Whether it is the final game")]
    public bool FinalGame { get; set; } = false;
    [Option("cheatMode", Required = false, HelpText = "Whether to open the cheat code")]
    public bool CheatMode { get; set; } = false;
    [Option("resultFileName", Required = false, HelpText = "Result file name, saved as .json")]
    public string ResultFileName { get; set; } = "114514";
    [Option("startLockFile", Required = false, HelpText = "Whether to create a file that identifies whether the game has started")]
    public string StartLockFile { get; set; } = "114514";
    [Option("mode", Required = false, HelpText = "Whether to run final competition")]
    public int Mode { get; set; } = 0;
}
```

其中，主要需要改动的是 `TeamCount` ， `ShipCount` 和 `HomeCount` ，分别代表队伍总数量，每队船只总数量和基地总数量。只有加入的 Client 对应数据满足这些条件，Server 才能开始游戏并发送数据。
进行游戏的最小条件是有一支队伍（默认红队）与一个基地，即设置 `TeamCount` 为 1， `ShipCount` 为 0， `HomeCount` 为 1 。只有加入了基地才能加入船只。如果需要测试船只，则最小条件为 `TeamCount` 为 1， `ShipCount` 为 1， `HomeCount` 为 1 。

## 2. 启动 Client
目前 Client 只能通过 Visual Studio 启动，其与 Server 的连接函数位于 `\THUAI7\logic\Client\ViewModel\GeneralViewModel.cs` 中的 `ConnectToServer` 函数中，如下所示。

```Csharp
// 连接Server,comInfo[]的格式：0-ip 1- port 2-playerID 3-teamID 4-ShipType
ConnectToServer(new string[]{
    "localhost",
    "8888",
    "0",
    "0",
    "1"
});
```

其中，传入参数对应的含义已在上方标明。修改对应的参数并运行 Client ，即可按照所加参数连接 Server 并添加相关的 player 信息。

## 3. 启动游戏
如果希望启动游戏，则需进行以下步骤：

1. 按照对应的参数启动 Server 。
2. 如果 Server 中设置 `TeamCount` 为 1， `ShipCount` 为 0， `HomeCount` 为 1 ，则可以直接启动 Client ，即可看到游戏开始。
3. 如果 Server 中的参数要求有更多的 Client 加入，则需要依次启动相应参数的 Client ，直到满足 Server 的条件。由于 Server 只要求有 Client 加入，而无需对应 Client 进程保持运行，可在加入一个 Client 后关闭其进程，再启动下一个 Client 。
   例如，如果希望加入一支队伍，这支队伍有一只船只，则可以按照如下步骤启动：
   1. Server 设置 `TeamCount` 为 1， `ShipCount` 为 1， `HomeCount` 为 1 。
   2. 启动一个 Client ，设置 `playerID` 为 0，`teamID` 为 0， `ShipType` 为 1 ，即加入一个 `home`。
   3. 关闭该 Client 进程，再重新启动一个 Client ，设置 `playerID` 为 1，`teamID` 为 0， `ShipType` 为 1 ，即加入一个 `ship`。
此时即可看到游戏开始。
