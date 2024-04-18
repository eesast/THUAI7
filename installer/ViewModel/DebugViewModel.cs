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
    public class Player
    {
        public int TeamID { get; set; }
        public int PlayerID { get; set; }
        public string PlayerMode { get; set; }

        public Player()
        {
            TeamID = 0;
            PlayerID = 1;
            PlayerMode = "API";
        }
    }


    public class DebugViewModel : LaunchViewModel
    {
        public ObservableCollection<Player> Players { get; }


        public DebugViewModel(Downloader downloader) : base(downloader)
        {
            Players = new ObservableCollection<Player>();

            AddBtnClickedCommand = new AsyncRelayCommand(AddBtnClicked);
            DelBtnClickedCommand = new AsyncRelayCommand(DelBtnClicked);
            ClientStartBtnClickedCommand = new AsyncRelayCommand(ClientStartBtnClicked);
            ServerStartBtnClickedCommand = new AsyncRelayCommand(ServerStartBtnClicked);
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
                    ClientVisible = true;
                    ServerVisible = false;
                }
                else if (mode == "Server")
                {
                    ClientVisible = false;
                    ServerVisible = true;
                }
                else
                {
                    ClientVisible = false;
                    ServerVisible = false;
                }
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
        #endregion


        #region Button
        public IAsyncRelayCommand AddBtnClickedCommand { get; }
        public IAsyncRelayCommand DelBtnClickedCommand { get; }
        public IAsyncRelayCommand ClientStartBtnClickedCommand { get; }
        public IAsyncRelayCommand ServerStartBtnClickedCommand { get; }


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
            DebugAlert = "Start";
            await Task.Run(() => ServerStart());
        }
        #endregion


        #region Start
        private void ClientStart()
        {
            Downloader.Data.Config.Commands.PlaybackFile = "";

            bool haveSpectator = false;
            for (int i = 0; i < Players.Count(); i++)
            {
                if (Players[i].PlayerMode == "API")
                {
                    if (CppSelect)
                        LaunchCppAPI(Players[i].TeamID, Players[i].PlayerID);
                    else if (PySelect)
                        LaunchPyAPI(Players[i].TeamID, Players[i].PlayerID);
                }
                if (Players[i].PlayerMode == "Manual")
                {
                    Downloader.Data.Config.Commands.TeamID = Players[i].TeamID;
                    Downloader.Data.Config.Commands.PlayerID = Players[i].PlayerID;
                    haveSpectator = true;
                    LaunchClient();
                }
            }
            if (!haveSpectator)
            {
                Downloader.Data.Config.Commands.TeamID = 0;
                Downloader.Data.Config.Commands.PlayerID = 2024;
                LaunchClient();
            }
        }

        private void ServerStart()
        {
            LaunchServer();
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
                FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Server", "Server.exe"),
                Arguments = $"--ip {IP} --port {Port} --teamCount {TeamCount} --shipNum {ShipCount}",
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
            Log.LogWarning("Server成功启动，请保持网络稳定。");
            OnServerLaunched?.Invoke(this, EventArgs.Empty);
            return true;
        }

        
        protected Process? client;
        public bool LaunchClient()
        {
            client = Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
            });
            if (client is null)
            {
                Log.LogError("未能启动Client!");
                return false;
            }
            Log.LogInfo("Client成功启动。");
            return true;
        }

        protected bool ExplorerLaunched_CppAPI = false;
        protected bool ExplorerLaunched_PyAPI = false;
        public bool LaunchCppAPI(int team, int player)
        {
            var exe = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp", "x64", "Debug", "API.exe");
            if (File.Exists(exe))
            {
                var cpp = Process.Start(new ProcessStartInfo()
                {
                    FileName = exe,
                    Arguments = $"-I {IP} -P {Port} -t {team} -p {player} -o"
                });
                if (cpp is null)
                {
                    Log.LogError($"未能启动API.exe, team:{team}, player: {player}!");
                    return false;
                }
                Log.LogInfo($"API.exe启动成功, team:{team}, player: {player}!");
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
            if (Directory.Exists(Path.Combine(p, "proto")))
            {

                var py = Process.Start(new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/c python "
                        + Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python", "PyAPI", "main.py")
                        + $" -I {IP} -P {Port} -t {team} -p {player} -o"
                });
                if (py is null)
                {
                    Log.LogError($"未能启动main.py, team:{team}, player: {player}!");
                    return false;
                }
                Log.LogError($"main.py启动成功, team:{team}, player: {player}!");
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
        #endregion
    }
}