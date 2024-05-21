using CommunityToolkit.Mvvm.Input;
using installer.Model;
using installer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace installer.ViewModel
{
    public class DebugViewModel : LaunchViewModel
    {
        public ObservableCollection<Player> Players { get => Downloader.Data.Config.Players; }

        public DebugViewModel(Downloader downloader) : base(downloader)
        {
            AddBtnClickedCommand = new AsyncRelayCommand(AddBtnClicked);
            DelBtnClickedCommand = new AsyncRelayCommand(DelBtnClicked);
            ClientStartBtnClickedCommand = new AsyncRelayCommand(ClientStartBtnClicked);
            ServerStartBtnClickedCommand = new AsyncRelayCommand(ServerStartBtnClicked);
            OnServerExited += ServerExited;
        }


        #region Property
        private string mode = "Server";
        public string Mode
        {
            get => mode;
            set
            {
                mode = value;
                if (mode == "Client")
                {
                    IPVisible = true;
                    ClientVisible = true;
                    ServerVisible = false;
                }
                else if (mode == "Server")
                {
                    IPVisible = false;
                    ClientVisible = false;
                    ServerVisible = true;
                }
                else
                {
                    IPVisible = true;
                    ClientVisible = false;
                    ServerVisible = false;
                }
            }
        }
        private bool ipVisible = false;
        public bool IPVisible
        {
            get => ipVisible;
            set
            {
                ipVisible = value;
                OnPropertyChanged();
            }
        }

        private bool clientVisible;
        private bool serverVisible;
        public bool ClientVisible
        {
            get => clientVisible;
            set
            {
                clientVisible = value;
                OnPropertyChanged();
            }
        }
        public bool ServerVisible
        {
            get => serverVisible;
            set
            {
                serverVisible = value;
                OnPropertyChanged();
            }
        }

        private int teamCount = 2;
        private int shipCount = 4;
        public int TeamCount
        {
            get => teamCount;
            set
            {
                teamCount = value;
                OnPropertyChanged();
            }
        }
        public int ShipCount
        {
            get => shipCount;
            set
            {
                shipCount = value;
                OnPropertyChanged();
            }
        }

        private bool haveSpectator = false;
        public bool HaveSpectator
        {
            get => haveSpectator;
            set
            {
                haveSpectator = value;
                OnPropertyChanged();
            }
        }
        private string spectatorID = "2024";
        public string SpectatorID
        {
            get => spectatorID;
            set
            {
                spectatorID = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #region Button
        public IAsyncRelayCommand AddBtnClickedCommand { get; }
        public IAsyncRelayCommand DelBtnClickedCommand { get; }
        public IAsyncRelayCommand ClientStartBtnClickedCommand { get; }
        public IAsyncRelayCommand ServerStartBtnClickedCommand { get; }

        private string serverStartBtnText = "启动";

        public string ServerStartBtnText
        {
            get => serverStartBtnText;
            set
            {
                serverStartBtnText = value;
                OnPropertyChanged();
            }
        }

        private bool serverStartMode = false;
        public bool ServerStartMode
        {
            get => serverStartMode;
            set
            {
                serverStartMode = value;
                ServerStartBtnText = value ? "停止" : "启动";
                OnPropertyChanged();
            }
        }


        private async Task AddBtnClicked()
        {
            await Task.Run(() => Players.Add(new Player()));
        }

        private async Task DelBtnClicked()
        {
            await Task.Run(() =>
            {
                if (Players.Count() > 0)
                    Players.RemoveAt(Players.Count() - 1);
            });
        }

        private async Task ClientStartBtnClicked()
        {
            DebugAlert = "Start";
            await Task.Run(() => ClientStart());
        }

        private async Task ServerStartBtnClicked()
        {
            StartEnabled = false;
            if (ServerStartMode)
            {
                DebugAlert = "Stop";
                await Task.Run(() => ServerStop());
            }
            else
            {
                DebugAlert = "Start";
                await Task.Run(() => ServerStart());
            }
            ServerStartMode = !ServerStartMode;
            StartEnabled = true;
        }
        #endregion


        #region Start
        private void ClientStart()
        {
            Downloader.Data.Config.Commands.PlaybackFile = "";

            bool haveManual = false;

            // 启动Manual很慢，先启动API
            for (int i = 0; i < Players.Count(); i++)
            {
                if (Players[i].PlayerMode == "API")
                {
                    if (CppSelect)
                        LaunchCppAPI(Players[i].TeamID, Players[i].PlayerID);
                    else if (PySelect)
                        LaunchPyAPI(Players[i].TeamID, Players[i].PlayerID);
                }
            }
            for (int i = 0; i < Players.Count(); i++)
            {
                if (Players[i].PlayerMode == "Manual")
                {
                    Downloader.Data.Config.Commands.TeamID = Players[i].TeamID;
                    Downloader.Data.Config.Commands.PlayerID = Players[i].PlayerID;
                    Downloader.Data.Config.Commands.ShipType = Players[i].ShipType;
                    LaunchClient(Players[i].TeamID, Players[i].PlayerID, Players[i].ShipType);
                    haveManual = true;
                }
            }
            if (HaveSpectator && !haveManual)
            {
                Downloader.Data.Config.Commands.TeamID = 0;
                try
                {
                    var id = Convert.ToInt32(SpectatorID);
                    if (id < 2024)
                    {
                        throw new Exception();
                    }
                    Downloader.Data.Config.Commands.PlayerID = id;
                    LaunchClient(0, id, 0);
                }
                catch (Exception)
                {
                    DebugAlert = "观战ID输入错误";
                    return;
                }
            }
        }

        private void ServerStart()
        {
            LaunchServer();
        }

        private void ServerStop()
        {
            server?.Kill();
        }

        private void ServerExited(object? sender, EventArgs e)
        {
            children.ForEach(c => c.Kill());
            children.Clear();
            ServerStartMode = false;
        }
        #endregion


        #region 启动方法组
        protected List<Process> children = new List<Process>();
        protected Process? server;
        public event EventHandler? OnServerLaunched;
        public event EventHandler? OnServerExited;

        public bool LaunchServer()
        {
            server = Process.Start(new ProcessStartInfo()
            {
                FileName = Downloader.Data.Config.DevServerPath ?? Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Server", "Server.exe"),
                Arguments = $"--ip 0.0.0.0 --port {Port} --teamCount {TeamCount} --shipNum {ShipCount}",
                WorkingDirectory = Downloader.Data.Config.InstallPath,
                RedirectStandardError = true,
            });
            if (server is null)
            {
                Log.LogError("Server未能启动！");
                return false;
            }
            server.EnableRaisingEvents = true;
            server.Exited += (_, _) =>
            {
                OnServerExited?.Invoke(this, EventArgs.Empty);
                Log.LogWarning("Server已退出。");
            };
            server.ErrorDataReceived += ProgramErrorReceived;
            Log.LogWarning("Server成功启动，请保持网络稳定。");
            OnServerLaunched?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public bool LaunchClient(int team, int player, int ship)
        {
            Downloader.Data.Config.Commands.TeamID = team;
            Downloader.Data.Config.Commands.PlayerID = player;
            Downloader.Data.Config.Commands.ShipType = ship;
            var p = Downloader.Data.Config.Commands.PlaybackFile;
            Downloader.Data.Config.Commands.PlaybackFile = null;

            if (File.Exists(Path.Combine(Downloader.Data.LogPath, $"lock.{team}.{player}.log")))
            {
                File.Delete(Path.Combine(Downloader.Data.LogPath, $"lock.{team}.{player}.log"));
            }

            var client = Process.Start(new ProcessStartInfo()
            {
                FileName = Downloader.Data.Config.DevClientPath ?? Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
                WorkingDirectory = Downloader.Data.Config.InstallPath,
                RedirectStandardError = true,
            });
            if (client is null)
            {
                Log.LogError("未能启动Client!");
                return false;
            }
            while (!File.Exists(Path.Combine(Downloader.Data.LogPath, $"lock.{team}.{player}.log")))
            {
                Thread.Sleep(500);
            }
            Thread.Sleep(500);
            File.Delete(Path.Combine(Downloader.Data.LogPath, $"lock.{team}.{player}.log"));
            Downloader.Data.Config.Commands.PlaybackFile = p;
            Log.LogInfo($"Client({team}: {player})成功启动。");
            client.EnableRaisingEvents = true;
            client.ErrorDataReceived += ProgramErrorReceived;
            client.BeginErrorReadLine();
            client.Exited += (_, _) =>
            {
                Log.LogWarning($"client({team}:{player})已退出({client.ExitCode})。");
            };
            client.ErrorDataReceived += ProgramErrorReceived;
            children.Add(client);
            return true;
        }

        protected bool ExplorerLaunched_CppAPI = false;
        protected bool ExplorerLaunched_PyAPI = false;
        public bool LaunchCppAPI(int team, int player)
        {
            var exe = Downloader.Data.Config.DevCppPath ?? Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp", "x64", "Debug", "API.exe");
            var logDir = Path.Combine(Downloader.Data.LogPath, $"Team{team}");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            if (File.Exists(exe))
            {
                var cpp = Process.Start(new ProcessStartInfo()
                {
                    FileName = Downloader.Data.Config.DevCppPath ?? exe,
                    Arguments = $"-I {IP} -P {Port} -t {team} -p {player} -o -d",
                    WorkingDirectory = logDir,
                    RedirectStandardError = true,
                });
                if (cpp is null)
                {
                    Log.LogError($"未能启动API.exe, team:{team}, player: {player}!");
                    return false;
                }
                Log.LogInfo($"API.exe启动成功, team:{team}, player: {player}!");
                cpp.EnableRaisingEvents = true;
                cpp.ErrorDataReceived += ProgramErrorReceived;
                cpp.BeginErrorReadLine();
                cpp.Exited += (_, _) =>
                {
                    Log.LogWarning($"API.exe({team}:{player})已退出({cpp.ExitCode})。");
                };
                children.Add(cpp);
                return true;
            }
            else
            {
                Log.LogError("请先生成cpp对应可执行文件后再启动，参见“Help-Launch-CPP可执行文件构建”");
                if (!ExplorerLaunched_CppAPI)
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "explorer.exe",
                        Arguments = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp")
                    });
                    ExplorerLaunched_CppAPI = true;
                }
                return false;
            }
        }

        public bool LaunchPyAPI(int team, int player)
        {
            var p = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python");
            var logDir = Path.Combine(Downloader.Data.LogPath, $"Team{team}");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            if (Directory.Exists(Path.Combine(p, "proto")))
            {
                var py = Process.Start(new ProcessStartInfo()
                {
                    FileName = "python.exe",
                    Arguments = (Downloader.Data.Config.DevPyPath ?? Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python", "PyAPI", "main.py"))
                        + $" -I {IP} -P {Port} -t {team} -p {player} -o -d",
                    WorkingDirectory = logDir,
                    RedirectStandardError = true,
                });
                if (py is null)
                {
                    Log.LogError($"未能启动main.py, team:{team}, player: {player}!");
                    return false;
                }
                Log.LogInfo($"main.py启动成功, team:{team}, player: {player}!");
                py.EnableRaisingEvents = true;
                py.ErrorDataReceived += ProgramErrorReceived;
                py.BeginErrorReadLine();
                py.Exited += (_, _) =>
                {
                    Log.LogWarning($"main.py({team}:{player})已退出({py.ExitCode})。");
                };
                children.Add(py);
                return true;
            }
            else
            {
                DebugAlert = "请构建proto后安装，参见“Help-Launch-Python proto构建”";
                if (!ExplorerLaunched_PyAPI)
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = "explorer.exe",
                        Arguments = p
                    });
                    ExplorerLaunched_PyAPI = true;
                }
                return false;
            }
        }

        protected virtual void ProgramErrorReceived(object sender, DataReceivedEventArgs e)
        {
            var program = sender as Process;
            Log.LogError(string.Format("error occurs in {0}:\n {1}",
                program is null ? "(anonymous)" :
                    (program.StartInfo.FileName + ' ' + program.StartInfo.Arguments),
                e.Data ?? "Unhandled error."));
        }
        #endregion
    }
}