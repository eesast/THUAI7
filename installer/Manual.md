# 下载器

- 第一次打开下载器后会在 `C:\Users\用户名\Documents\` 内生成文件夹 `THUAI7`，里面存放了下载器的一些配置信息，非必要不要手动修改。

## Installer

### 下载

下载和移动功能均需要选择空的文件夹路径。

- 第一次下载完成后，按钮上的 `下载` 会变为 `移动`。之后选择新的空文件夹路径将会进行移动操作。
- 如果第一次下载出现问题，且 `是否已下载选手包` 点亮，此时请关闭下载器，删除下载的选手包，将 `C:\Users\用户名\Documents\THUAI7\config.json` 中的 `"Installed": true` 改为 `"Installed": false`

### 更新

更新前需要先进行检查更新操作。

- **下载后不要对其他非空文件路径进行更新操作，这将会删除多余的文件**

第一次下载完成后，请检查更新，第一次下载的安装包通常版本较旧。

## Launcher

- C++：选手需要在 `%InstallPath%\CAPI\cpp\CAPI.sln` 中开发，且一般只需要修改 `%InstallPath%\CAPI\cpp\API\src\AI.cpp`，然后生成项目。
- Python：选手需要确保电脑已经安装了 `python` 和 `pip`，然后执行 `%InstallPath%\CAPI\python\generate_proto.cmd`（Windows） 或 `%InstallPath%\CAPI\python\generate_proto.cmd`（Mac/Linux），等待 protos 文件夹生成完毕，在 `%InstallPath%\CAPI\python\PyAPI\AI.py` 中开发。

当启动器中的 `IP` `Port` `Language` `Playback File` `Playback Speed` 被修改后，需要先点击 `保存`。

### Debug

**启动器的正常工作可能需要管理员权限**

#### 本地调试流程

1. 输入 `Port`，默认为 `8888`
2. 选择 `Server` 调整 `Team Count` 和 `Ship Count`，启动 *Server*
3. 选择 `Client`，输入 `127.0.0.1`，加入对应的数量的 *Player*，调整各个 *Player* 的参数和  `Language` 后启动 *Client*
   - *Player* 数量为 $\mathrm{TeamCount}*(\mathrm{ShipCount}+1)$， *Player* 全部加入后 *Server* 才会开始游戏
   - `Player ID` 为 `0` 表示 `Home`，其他为 `Ship`
   - `Player Mode` 为 `API` 表示执行选手在 *CAPI* 中写的代码的 *Player*；为 `Manual` 表示一个手动操控的 *Player*。手动操控模式下，`Player ID` 不能为 `0`，`Ship Type` 不能为 `0`，同时一台电脑不建议有多个手动操控的 *Player*。

#### 联机调试流程

1. 其中一台电脑启动 *Server*
2. 多台电脑输入 *Server* 电脑的  `IP` 和 `Port`，`IP` 可以通过在 cmd 中输入 `ipconfig` 查看
3. 所有电脑凑够对应数量的 *Player*，启动 *Client*

#### Spectator

- 如果一台电脑不存在 `Player Mode` 为 `Manual` 的 *Player*，需要启动 *Spectator*，来观看本局比赛。如果存在 `Player Mode` 为 `Manual` 的 *Player*，就不要启动 *Spectator*。
- *Spectator* 的 `ID` 应大于等于 `2024`，观察同场比赛的多个 *Spectator* 需要有不同的 `ID`。

### Playback

每次调试后会在 `%InstallPath%\logic\Server` 中生成 `114514.thuai7.pb`，在 `Playback File` 中输入 `114514.thuai7.pb`，点击 `保存` 和 `启动`。也可以对回放文件进行改名，输入对应文件名即可。

## Login

暂时没有功能

## Other

欢迎在比赛群中提出有关比赛的各种问题！

**祝大家 Debug 快乐！**
