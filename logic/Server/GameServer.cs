using GameClass.GameObj;
using GameClass.GameObj.Map;
using GameClass.MapGenerator;
using Gaming;
using Newtonsoft.Json;
using Playback;
using Preparation.Utility;
using Protobuf;
using System.Collections.Concurrent;
using Timothy.FrameRateTask;
using System.Net.Http.Json;
using System.Collections;

namespace Server
{
    public static class GameServerLogging
    {
        public static readonly Logger logger = new("GameServer");
    }
    public class ContestResult
    {
        public int score;
    }
    partial class GameServer : ServerBase
    {
        private readonly ConcurrentDictionary<long, (SemaphoreSlim, SemaphoreSlim)> semaDict0 = new(); //for spectator and team0 player
        private readonly ConcurrentDictionary<long, (SemaphoreSlim, SemaphoreSlim)> semaDict1 = new();
        // private object semaDictLock = new();
        protected readonly ArgumentOptions options;
        private readonly HttpSender? httpSender;
        private readonly object gameLock = new();
        private MessageToClient currentGameInfo = new();
        private readonly MessageOfObj currentMapMsg = new();
        private readonly object newsLock = new();
        private readonly List<MessageOfNews> currentNews = [];
        private readonly SemaphoreSlim endGameSem = new(0);
        protected readonly Game game;
        private readonly uint spectatorMinPlayerID = 2023;
        public int playerNum;
        public int TeamCount => options.TeamCount;
        protected long[][] communicationToGameID; // 通信用的ID映射到游戏内的ID，0指向队伍1，1指向队伍2，通信中0为大本营，1-5为船
        private readonly object messageToAllClientsLock = new();
        public static readonly long SendMessageToClientIntervalInMilliseconds = 50;
        private readonly MessageWriter? mwr = null;
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
            GameServerLogging.logger.ConsoleLog("Game starts!");
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
                GameServerLogging.logger.ConsoleLog("Successfully Created StartLockFile!");
            }
        }

        public override void WaitForEnd()
        {
            endGameSem.Wait();
            mwr?.Dispose();
        }

        private void SaveGameResult(string path)
        {
            Dictionary<string, int> result = [];
            int[] score = GetScore();
            result.Add("RedTeam", score[0]);
            result.Add("BlueTeam", score[1]);
            JsonSerializer serializer = new();
            using StreamWriter sw = new(path);
            using JsonTextWriter writer = new(sw);
            serializer.Serialize(writer, result);

        }

        protected void SendGameResult(int[] scores, int mode)		// 天梯的 Server 给网站发消息记录比赛结果
        {
            httpSender?.SendHttpRequest(scores, mode).Wait();

        }

        protected async Task<double[]> GetLadderScore(int mode, double[] scores)
        {

            string? url2 = Environment.GetEnvironmentVariable("SCORE_URL");
            if (url2 != null)
            {
                try
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new("Bearer", options.Token);
                    var response = await httpClient.PostAsync(url2, JsonContent.Create(new { HttpHeaders = options.Token }));
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容为字符串
                        var jsonString = await response.Content.ReadAsStringAsync();

                        // 解析 JSON 字符串
                        var result = JsonConvert.DeserializeObject<List<ContestResult>>(jsonString);
                        double[] org = (from r in result select (double)(r.score)).ToArray();
                        double[] final = Cal(org, scores);
                        return final;


                        // 从 JSON 文档中获取需要的数据
                        // 例如：var result = jsonDocument.RootElement.GetProperty("result");
                        // Console.WriteLine(result.ToString());
                        //double[] finalscores = Cal(result, scores);
                    }
                    else return new double[0];

                }
                catch (Exception e)
                {
                    Console.WriteLine("Fail to send msg to web!");
                    Console.WriteLine(e);
                    return new double[0];
                }
            }
            else return new double[0];
        }

        protected double[] Cal(double[] orgScore, double[] competitionScore)
        {
            // 调整顺序，让第一个元素成为获胜者，便于计算
            bool reverse = false; // 记录是否需要调整
            if (competitionScore[0] < competitionScore[1])
            {
                reverse = true;
            }
            else if (competitionScore[0] == competitionScore[1])
            {
                if (orgScore[0] == orgScore[1])
                {
                    // 完全平局，不改变天梯分数
                    return orgScore;
                }
                if (orgScore[0] > orgScore[1])
                {
                    // 本次游戏平局，但一方天梯分数高，另一方天梯分数低，需要将两者向中间略微靠拢，因此天梯分数低的定为获胜者
                    reverse = true;
                }
            }
            if (reverse)
            {
                // 如果需要换，换两者的顺序
                double t = competitionScore[1];
                competitionScore[1] = competitionScore[0];
                competitionScore[0] = t;
                t = orgScore[1];
                orgScore[1] = orgScore[0];
                orgScore[0] = t;
            }
            double[] resScore = [0, 0];
            double deltaWeight = 1000.0; // 差距悬殊判断参数
            double delta = (orgScore[0] - orgScore[1]) / deltaWeight;
            // 盈利者天梯得分权值、落败者天梯得分权值
            double firstnerGet = 9e-6;
            double secondrGet = 5e-6;
            double deltaScore = 2100.0; // 两队竞争分差超过多少时就认为非常大
            double correctRate = (orgScore[0] - orgScore[1]) / (deltaWeight * 1.2); // 订正的幅度，该值越小，则在势均力敌时天梯分数改变越大
            double correct = 0.5 * (Math.Tanh((competitionScore[0] - competitionScore[1] - deltaScore) / deltaScore - correctRate) + 1.0); // 一场比赛中，在双方势均力敌时，减小天梯分数的改变量
            resScore[0] = orgScore[0] + Math.Round(competitionScore[0] * competitionScore[0] * firstnerGet * (1 - Math.Tanh(delta)) * correct); // 胜者所加天梯分
            resScore[1] = orgScore[1] - Math.Round(
            (competitionScore[0] - competitionScore[1]) * (competitionScore[0] - competitionScore[1]) * secondrGet * (1 - Math.Tanh(delta)) * correct); // 败者所扣天梯分
                                                                                                                                                        // 如果换过，再换回来
            if (reverse)
            {
                double t = resScore[1];
                resScore[1] = resScore[0];
                resScore[0] = t;
            }
            return resScore;
        }


        private void OnGameEnd()
        {
            game.ClearAllLists();
            mwr?.Flush();
            if (options.ResultFileName != DefaultArgumentOptions.FileName)
                SaveGameResult(options.ResultFileName.EndsWith(".json")
                             ? options.ResultFileName
                             : options.ResultFileName + ".json");
            int[] scores = GetScore();
            double[] doubleArray = scores.Select(x => (double)x).ToArray();
            if (options.Mode == 2)
                scores = GetLadderScore(options.Mode, doubleArray).Result;
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
                        foreach (Base team in game.TeamList)
                        {
                            MessageOfObj? msg = CopyInfo.Auto(team, time);
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
                        currentGameInfo.AllMessage = GetMessageOfAll(game.GameMap.Timer.NowTime());
                        mwr?.WriteOne(currentGameInfo);
                        break;
                    default:
                        break;
                }
            }
            lock (spectatorJoinLock)
            {
                foreach (var kvp in semaDict0)
                {
                    kvp.Value.Item1.Release();
                }
                foreach (var kvp in semaDict1)
                {
                    kvp.Value.Item1.Release();
                }
                // 若此时观战者加入，则死锁，所以需要 spectatorJoinLock

                foreach (var kvp in semaDict0)
                {
                    kvp.Value.Item2.Wait();
                }
                foreach (var kvp in semaDict1)
                {
                    kvp.Value.Item2.Wait();
                }
            }
        }

        private bool PlayerDeceased(int playerID)    //# 这里需要判断大本营deceased吗？
        {
            return game.GameMap.GameObjDict[GameObjType.Ship].Cast<Ship>()?.Find(
                ship => ship.PlayerID == playerID && ship.ShipState == ShipStateType.Deceased
                ) != null;
        }

        public override int[] GetMoney()
        {
            int[] money = new int[2]; // 0代表RedTeam，1代表BlueTeam
            foreach (Base team in game.TeamList)
            {
                money[(int)team.TeamID] = (int)game.GetTeamMoney(team.TeamID);
            }
            return money;
        }

        public override int[] GetScore()
        {
            int[] score = new int[2]; // 0代表RedTeam，1代表BlueTeam
            foreach (Base team in game.TeamList)
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
            if (playerID == 0 || (1 <= playerID && playerID <= options.ShipCount))
                return true;
            return false;
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
            int[] energy = GetMoney();
            msg.RedTeamEnergy = energy[0];
            msg.BlueTeamEnergy = energy[1];
            msg.RedHomeHp = (int)game.TeamList[0].Home.HP;
            msg.BlueHomeHp = (int)game.TeamList[1].Home.HP;
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
            if (options.MapResource == DefaultArgumentOptions.MapResource)
                game = new(MapInfo.defaultMapStruct, options.TeamCount);
            else
            {
                // txt文本方案
                if (options.MapResource.EndsWith(".txt"))
                {
                    try
                    {
                        uint[,] map = new uint[GameData.MapRows, GameData.MapCols];
                        string? line;
                        int i = 0, j = 0;
                        using StreamReader sr = new(options.MapResource);
                        #region 读取txt地图
                        while (!sr.EndOfStream && i < GameData.MapRows)
                        {
                            if ((line = sr.ReadLine()) != null)
                            {
                                string[] nums = line.Split(' ');
                                foreach (string item in nums)
                                {
                                    if (item.Length > 1)//以兼容原方案
                                        map[i, j] = (uint)int.Parse(item);
                                    else
                                        //2022-04-22 by LHR 十六进制编码地图方案（防止地图编辑员瞎眼x
                                        map[i, j] = (uint)MapEncoder.Hex2Dec(char.Parse(item));
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
                        #endregion
                        game = new(new(GameData.MapRows, GameData.MapCols, map), options.TeamCount);
                    }
                    catch
                    {
                        game = new(MapInfo.defaultMapStruct, options.TeamCount);
                    }
                }
                // MapStruct二进制方案
                else if (options.MapResource.EndsWith(".map"))
                {
                    try
                    {
                        game = new(MapStruct.FromFile(options.MapResource), options.TeamCount);
                    }
                    catch
                    {
                        game = new(MapInfo.defaultMapStruct, options.TeamCount);
                    }
                }
                else
                {
                    game = new(MapInfo.defaultMapStruct, options.TeamCount);
                }
            }
            currentMapMsg = new() { MapMessage = MapMsg() };
            playerNum = options.ShipCount + options.HomeCount;
            communicationToGameID = new long[TeamCount][];
            for (int i = 0; i < TeamCount; i++)
            {
                communicationToGameID[i] = new long[options.ShipCount + options.HomeCount];
            }
            //创建server时先设定待加入对象都是invalid
            for (int team = 0; team < TeamCount; team++)
            {
                communicationToGameID[team][0] = GameObj.invalidID; // team
                for (int i = 1; i <= options.ShipCount; i++)
                {
                    communicationToGameID[team][i] = GameObj.invalidID; //ship
                }
            }

            if (options.FileName != DefaultArgumentOptions.FileName)
            {
                try
                {
                    mwr = new(options.FileName, options.TeamCount, options.ShipCount);
                }
                catch
                {
                    GameServerLogging.logger.ConsoleLog($"Error: Cannot create the playback file: {options.FileName}!");
                }
            }


            if (options.Url != DefaultArgumentOptions.Url && options.Token != DefaultArgumentOptions.Token)
            {
                httpSender = new(options.Url, options.Token);
            }
        }
    }
}
