using GameClass.GameObj;
using Gaming;
using GameClass.MapGenerator;
using Newtonsoft.Json;
using Playback;
using Preparation.Utility;
using Protobuf;
using System.Collections.Concurrent;
using Timothy.FrameRateTask;
using Utility = Preparation.Utility;

namespace Server
{
    partial class GameServer : ServerBase
    {
        private ConcurrentDictionary<long, (SemaphoreSlim, SemaphoreSlim)> semaDict = new();
        // private object semaDictLock = new();
        protected readonly ArgumentOptions options;
        private HttpSender? httpSender;
        private readonly object gameLock = new();
        private MessageToClient currentGameInfo = new();
        private MessageOfObj currentMapMsg = new();
        private readonly object newsLock = new();
        private List<MessageOfNews> currentNews = [];
        private SemaphoreSlim endGameSem = new(0);
        protected readonly Game game;
        private readonly uint spectatorMinPlayerID = 2023;
        public int playerNum;
        public int TeamCount => options.TeamCount;
        protected long[][] communicationToGameID; // 通信用的ID映射到游戏内的ID，0指向队伍1，1指向队伍2，通信中0为大本营，1-5为船
        private readonly object messageToAllClientsLock = new();
        public static readonly long SendMessageToClientIntervalInMilliseconds = 50;
        private MessageWriter? mwr = null;
        private readonly object spectatorJoinLock = new();

        public void StartGame()
        {
            if (game.GameMap.Timer.IsGaming) return;
            foreach (var team in communicationToGameID)
            {
                foreach (var id in team)
                {
                    if (id == GameObj.invalidID) return;//如果有未初始化的玩家，不开始游戏
                }
            }
            Console.WriteLine("Game starts!");
            CreateStartFile();
            game.StartGame((int)options.GameTimeInSecond * 1000);
            Thread.Sleep(1);
            new Thread(() =>
            {
                bool flag = true;
                new FrameRateTaskExecutor<int>
                (
                    () => game.GameMap.Timer.IsGaming,
                    () =>
                    {
                        if (flag == true)
                        {
                            ReportGame(GameState.GameStart);
                            flag = false;
                        }
                        else ReportGame(GameState.GameRunning);
                    },
                    SendMessageToClientIntervalInMilliseconds,
                    () =>
                    {
                        ReportGame(GameState.GameEnd);  // 最后发一次消息，唤醒发消息的线程，防止发消息的线程由于有概率处在 Wait 状态而卡住
                        OnGameEnd();
                        return 0;
                    }
                ).Start();
            })
            { IsBackground = true }.Start();
        }

        public void CreateStartFile()
        {
            if (options.StartLockFile != DefaultArgumentOptions.FileName)
            {
                using var _ = File.Create(options.StartLockFile);
                Console.WriteLine("Successfully Created StartLockFile!");
            }
        }

        public override void WaitForEnd()
        {
            this.endGameSem.Wait();
            mwr?.Dispose();
        }

        private void SaveGameResult(string path)
        {
            Dictionary<string, int> result = [];
            int[] score = GetScore();
            result.Add("RedTeam", score[0]);
            result.Add("BlueTeam", score[1]);
            JsonSerializer serializer = new();
            using (StreamWriter sw = new(path))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, result);
                }
            }
        }

        protected void SendGameResult(int[] scores, int mode)		// 天梯的 Server 给网站发消息记录比赛结果
        {
            httpSender?.SendHttpRequest(scores, mode).Wait();
        }

        private void OnGameEnd()
        {
            game.ClearAllLists();
            mwr?.Flush();
            if (options.ResultFileName != DefaultArgumentOptions.FileName)
                SaveGameResult(options.ResultFileName.EndsWith(".json") ? options.ResultFileName : options.ResultFileName + ".json");
            int[] scores = GetScore();
            SendGameResult(scores, options.Mode);
            endGameSem.Release();
        }
        public void ReportGame(GameState gameState, bool requiredGaming = true)
        {
            var gameObjList = game.GetGameObj();
            currentGameInfo = new();
            lock (messageToAllClientsLock)
            {
                switch (gameState)
                {
                    case GameState.GameRunning:
                    case GameState.GameEnd:
                    case GameState.GameStart:
                        if (gameState == GameState.GameStart || IsSpectatorJoin)
                        {
                            currentGameInfo.ObjMessage.Add(currentMapMsg);
                            IsSpectatorJoin = false;
                        }
                        long time = Environment.TickCount64;
                        foreach (GameObj gameObj in gameObjList.Cast<GameObj>())
                        {
                            MessageOfObj? msg = CopyInfo.Auto(gameObj, time);
                            if (msg != null) currentGameInfo.ObjMessage.Add(msg);
                        }
                        lock (newsLock)
                        {
                            foreach (var news in currentNews)
                            {
                                MessageOfObj? msg = CopyInfo.Auto(news);
                                if (msg != null) currentGameInfo.ObjMessage.Add(msg);
                            }
                            currentNews.Clear();
                        }
                        currentGameInfo.GameState = gameState;
                        currentGameInfo.AllMessage = GetMessageOfAll(game.GameMap.Timer.nowTime());
                        mwr?.WriteOne(currentGameInfo);
                        break;
                    default:
                        break;
                }
            }
            lock (spectatorJoinLock)
            {
                foreach (var kvp in semaDict)
                {
                    kvp.Value.Item1.Release();
                }

                // 若此时观战者加入，则死锁，所以需要 spectatorJoinLock

                foreach (var kvp in semaDict)
                {
                    kvp.Value.Item2.Wait();
                }
            }
        }

        private bool PlayerDeceased(int playerID)    //# 这里需要判断大本营deceased吗？
        {
            game.GameMap.GameObjLockDict[GameObjType.Ship].EnterReadLock();
            try
            {
                foreach (Ship ship in game.GameMap.GameObjDict[GameObjType.Ship].Cast<Ship>())
                {
                    if (ship.ShipID == playerID && ship.ShipState == ShipStateType.Deceased) return true;
                }
            }
            finally
            {
                game.GameMap.GameObjLockDict[GameObjType.Ship].ExitReadLock();
            }

            return false;
        }

        public override int[] GetMoney()
        {
            int[] money = new int[2]; // 0代表RedTeam，1代表BlueTeam
            foreach (Team team in game.TeamList)
            {
                money[(int)team.TeamID] = (int)game.GetTeamMoney(team.TeamID);
            }
            return money;
        }

        public override int[] GetScore()
        {
            int[] score = new int[2]; // 0代表RedTeam，1代表BlueTeam
            foreach (Team team in game.TeamList)
            {
                score[(int)team.TeamID] = (int)game.GetTeamScore(team.TeamID);
            }
            return score;
        }

        //# 需要改进为非static
        private uint GetBirthPointIdx(long playerID)  // 获取出生点位置
        {
            return (uint)playerID + 1; // ID从0-8,出生点从1-9
        }

        private bool ValidPlayerID(long playerID)
        {
            if ((0 <= playerID && playerID < options.ShipCount) || (options.MaxShipCount <= playerID && playerID < options.MaxShipCount + options.HomeCount))
                return true;
            return false;
        }

        private int PlayerIDToTeamID(long playerID)
        {
            if (0 <= playerID && playerID < options.MaxPlayerNumPerTeam) return 0;
            if (options.MaxPlayerNumPerTeam <= playerID && playerID < options.MaxPlayerNumPerTeam * 2) return 1;
            return -1;
        }

        private MessageOfAll GetMessageOfAll(int time)
        {
            MessageOfAll msg = new()
            {
                GameTime = time
            };
            int[] score = GetScore();
            msg.RedTeamScore = score[0];
            msg.BlueTeamScore = score[1];
            return msg;
        }

        private MessageOfMap MapMsg()
        {
            MessageOfMap msgOfMap = new()
            {
                Height = game.GameMap.Height,
                Width = game.GameMap.Width
            };
            for (int i = 0; i < game.GameMap.Height; i++)
            {
                msgOfMap.Rows.Add(new MessageOfMap.Types.Row());
                for (int j = 0; j < game.GameMap.Width; j++)
                {
                    msgOfMap.Rows[i].Cols.Add(Transformation.PlaceTypeToProto(game.GameMap.ProtoGameMap[i, j]));
                }
            }
            return msgOfMap;
        }

        public GameServer(ArgumentOptions options)
        {
            this.options = options;
            if (options.mapResource == DefaultArgumentOptions.MapResource)
                game = new(MapInfo.defaultMapStruct, options.TeamCount);
            else
            {
                uint[,] map = new uint[GameData.MapRows, GameData.MapCols];
                try
                {
                    string? line;
                    int i = 0, j = 0;
                    using (StreamReader sr = new(options.mapResource))
                    {
                        while (!sr.EndOfStream && i < GameData.MapRows)
                        {
                            if ((line = sr.ReadLine()) != null)
                            {
                                string[] nums = line.Split(' ');
                                foreach (string item in nums)
                                {
                                    if (item.Length > 1)//以兼容原方案
                                    {
                                        map[i, j] = (uint)int.Parse(item);
                                    }
                                    else
                                    {
                                        //2022-04-22 by LHR 十六进制编码地图方案（防止地图编辑员瞎眼x
                                        map[i, j] = (uint)MapEncoder.Hex2Dec(char.Parse(item));
                                    }
                                    j++;
                                    if (j >= GameData.MapCols)
                                    {
                                        j = 0;
                                        break;
                                    }
                                }
                                i++;
                            }
                        }
                    }
                }
                catch
                {
                    map = MapInfo.defaultMap;
                }
                finally
                {
                    MapStruct mapResource = new(GameData.MapRows, GameData.MapCols, map);
                    game = new(mapResource, options.TeamCount);
                }
            }
            currentMapMsg = new() { MapMessage = MapMsg() };
            playerNum = options.ShipCount + options.HomeCount;
            communicationToGameID = new long[TeamCount][];
            for (int i = 0; i < TeamCount; i++)
            {
                communicationToGameID[i] = new long[options.MaxShipCount + options.HomeCount];
            }
            //创建server时先设定待加入对象都是invalid
            for (int team = 0; team < TeamCount; team++)
            {
                for (int i = 0; i < options.MaxShipCount; i++)
                {
                    communicationToGameID[team][i] = GameObj.invalidID;
                }
                for (int i = options.MaxShipCount; i < options.MaxShipCount + options.HomeCount; i++)
                {
                    communicationToGameID[team][i] = GameObj.invalidID;
                }
            }

            if (options.FileName != DefaultArgumentOptions.FileName)
            {
                try
                {
                    mwr = new MessageWriter(options.FileName, options.TeamCount, options.ShipCount);
                }
                catch
                {
                    Console.WriteLine($"Error: Cannot create the playback file: {options.FileName}!");
                }
            }

            if (options.Url != DefaultArgumentOptions.Url && options.Token != DefaultArgumentOptions.Token)
            {
                this.httpSender = new HttpSender(options.Url, options.Token);
            }
        }
    }
}
