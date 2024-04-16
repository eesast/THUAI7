using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using installer.Model;
using installer.Data;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace installer.ViewModel
{
    public class LaunchViewModel : BaseViewModel
    {
        private readonly Downloader Downloader;
        protected ListLogger Log = new ListLogger();
        public ObservableCollection<LogRecord> LogList { get => Log.List; }
        public LaunchViewModel(Downloader downloader)
        {
            Downloader = downloader;

            Mode = "Debug";

            IP = Downloader.Data.Config.Commands.IP;
            Port = Downloader.Data.Config.Commands.Port;
            PlaybackFile = Downloader.Data.Config.Commands.PlaybackFile;
            PlaybackSpeed = Downloader.Data.Config.Commands.PlaybackSpeed.ToString();

            ipChanged = false;
            portChanged = false;
            playbackFileChanged = false;
            playbackSpeedChanged = false;

            switch (Downloader.Data.Config.Commands.Language)
            {
                case LanguageOption.cpp:
                    CppSelect = true;
                    PySelect = false;
                    languageChanged = false;
                    break;
                case LanguageOption.python:
                    CppSelect = false;
                    PySelect = true;
                    languageChanged = false;
                    break;
                default:
                    CppSelect = false;
                    PySelect = false;
                    languageChanged = true;
                    break;
            }


            SaveEnabled = true;
            StartEnabled = true;

            SaveBtnClickedCommand = new AsyncRelayCommand(SaveBtnClicked);
            StartBtnClickedCommand = new AsyncRelayCommand(StartBtnClicked);
        }

        public bool Installed { get => Downloader.Data.Config.Installed; }

        #region 参数
        private string? mode;
        public string? Mode
        {
            get => mode;
            set
            {
                mode = value;
                OnPropertyChanged();
                if (mode == "Playback")
                {
                    playbackFileChanged = true;
                    SaveEnabled = true;
                    StartEnabled = false;
                    PlaybackVisible = true;
                    DebugVisible = false;
                }
                else if (mode == "Debug")
                {
                    PlaybackFile = "";
                    SaveEnabled = true;
                    StartEnabled = false;
                    PlaybackVisible = false;
                    DebugVisible = true;
                }
                else
                {
                    SaveEnabled = false;
                    StartEnabled = false;
                    PlaybackVisible = false;
                    DebugVisible = false;
                }
            }
        }

        private bool playbackVisible;
        private bool debugVisible;
        public bool PlaybackVisible
        {
            get => playbackVisible;
            set
            {
                playbackVisible = value;
                OnPropertyChanged();
            }
        }
        public bool DebugVisible
        {
            get => debugVisible;
            set
            {
                debugVisible = value;
                OnPropertyChanged();
            }
        }

        private string? ip;
        private string? port;
        private string? playbackFile;
        private string? playbackSpeed;
        private bool cppSelect;
        private bool pySelect;

        private bool ipChanged;
        private bool portChanged;
        private bool playbackFileChanged;
        private bool playbackSpeedChanged;
        private bool languageChanged;

        public string? IP
        {
            get => ip;
            set
            {
                ip = value;
                if (ip == Downloader.Data.Config.Commands.IP)
                    ipChanged = false;
                else
                    ipChanged = true;
                StartEnabled = true;
                OnPropertyChanged();
            }
        }
        public string? Port
        {
            get => port;
            set
            {
                port = value;
                if (port == Downloader.Data.Config.Commands.Port)
                    portChanged = false;
                else
                    portChanged = true;
                StartEnabled = true;
                OnPropertyChanged();
            }
        }

        public string? PlaybackFile
        {
            get => playbackFile;
            set
            {
                playbackFile = value;
                if (playbackFile == Downloader.Data.Config.Commands.PlaybackFile)
                    playbackFileChanged = false;
                else
                    playbackFileChanged = true;
                StartEnabled = true;
                OnPropertyChanged();
            }
        }
        public string? PlaybackSpeed
        {
            get => playbackSpeed;
            set
            {
                playbackSpeed = value;
                if (playbackSpeed == Downloader.Data.Config.Commands.PlaybackSpeed.ToString())
                    playbackSpeedChanged = false;
                else
                    playbackSpeedChanged = true;
                StartEnabled = true;
                OnPropertyChanged();
            }
        }
        public bool CppSelect
        {
            get => cppSelect;
            set
            {
                cppSelect = value;
                if (cppSelect)
                {
                    PySelect = false;
                    if (Downloader.Data.Config.Commands.Language != LanguageOption.cpp)
                        languageChanged = true;
                    else
                        languageChanged = false;
                }
                if (!cppSelect && !PySelect)
                    languageChanged = true;
                StartEnabled = true;
                OnPropertyChanged();
            }
        }
        public bool PySelect
        {
            get => pySelect;
            set
            {
                pySelect = value;
                if (pySelect)
                {
                    CppSelect = false;
                    if (Downloader.Data.Config.Commands.Language != LanguageOption.python)
                        languageChanged = true;
                    else
                        languageChanged = false;
                }
                if (!cppSelect && !PySelect)
                    languageChanged = true;
                StartEnabled = true;
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

        private bool saveEnabled;
        public bool SaveEnabled
        {
            get => saveEnabled;
            set
            {
                saveEnabled = value;
                OnPropertyChanged();
            }
        }
        private bool startEnabled;
        public bool StartEnabled
        {

            get => startEnabled;
            set
            {
                startEnabled = value
                            && Installed
                            && !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }


        public ICommand SaveBtnClickedCommand { get; }
        public ICommand StartBtnClickedCommand { get; }

        private async Task SaveBtnClicked()
        {
            DebugAlert = "Save";
            await Task.Run(() => Save());
            StartEnabled = true;
        }
        private async Task StartBtnClicked()
        {
            DebugAlert = "Start";
            await Task.Run(() => Start());
            DebugAlert = "";
        }

        private void Save()
        {
            Task.Run(() =>
            {
                try
                {
                    if (IP == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.IP = IP;
                    ipChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "IP: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    if (Port == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.Port = Port;
                    portChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Port: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    if (Mode == "Playback" && string.IsNullOrEmpty(PlaybackFile))
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.PlaybackFile = PlaybackFile;
                    playbackFileChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Playback File: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    Downloader.Data.Config.Commands.PlaybackSpeed = Convert.ToDouble(PlaybackSpeed);
                    playbackSpeedChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Playback Speed: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    if (CppSelect)
                    {
                        Downloader.Data.Config.Commands.Language = LanguageOption.cpp;
                        languageChanged = false;
                    }
                    else if (PySelect)
                    {
                        Downloader.Data.Config.Commands.Language = LanguageOption.python;
                        languageChanged = false;
                    }
                    else
                        throw new Exception("empty");
                }
                catch (Exception e)
                {
                    DebugAlert = "Language: " + e.Message;
                }
            });
        }

        private bool serverStarted;
        private List<Process> children = new List<Process>();
        private Process? server;
        private void Start()
        {
            if (Mode == "Playback")
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
                });
            }
            else if (Mode == "Debug")
            {
                serverStarted = false;
                Log.LogInfo("Server Start!");
                server = Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Server", "Server.exe"),
                    Arguments = $"--ip {IP} --port {Port} --teamCount {TeamCount} --shipNum {ShipCount}",
                    RedirectStandardOutput = true
                });
                if (server is null)
                {
                    Log.LogError("未能启动Server!");
                    return;
                }
                server.EnableRaisingEvents = true;
                server.OutputDataReceived += (_, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Log.LogInfo(args.Data);
                        if (args.Data.Contains("Server begins to listen"))
                            serverStarted = true;
                    }
                };
                server.Exited += (_, _) =>
                {
                    while (children.Count > 0)
                    {
                        children[0].Kill(true);
                        children.RemoveAt(0);
                    }
                    Log.LogWarning("Server已退出!");
                };
                server.BeginOutputReadLine();
                DateTime t = DateTime.Now;
                while (!serverStarted && (DateTime.Now - t).TotalSeconds < 20) ;
                Log.LogWarning("Server成功启动，请保持网络稳定");
                var client = Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
                });
                if (client is null)
                {
                    Log.LogError("未能启动Client!");
                    return;
                }
                children.Add(client);
                if (CppSelect && string.IsNullOrEmpty(PlaybackFile))
                {
                    var exe = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp", "x64", "Debug", "API.exe");
                    if (File.Exists(exe))
                    {
                        for (int teamID = 0; teamID <= TeamCount; teamID++)
                            for (int playerID = 0; playerID <= ShipCount + 1; playerID++)
                            {
                                var cpp = Process.Start(new ProcessStartInfo()
                                {
                                    FileName = exe,
                                    Arguments = $"-I {IP} -P {Port} -t {teamID} -p {playerID} -o"
                                });
                                if (cpp is null)
                                {
                                    Log.LogError($"未能启动API.exe, team:{teamID}, player: {playerID}!");
                                    return;
                                }
                                children.Add(cpp);
                            }
                    }
                    else
                    {
                        DebugAlert = "请先生成cpp对应可执行文件后再启动，参见“Help-Launch-CPP可执行文件构建”";
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp")
                        });
                    }
                }
                else if (PySelect && string.IsNullOrEmpty(PlaybackFile))
                {
                    var p = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python");
                    if (Directory.Exists(Path.Combine(p, "proto")))
                    {
                        for (int teamID = 0; teamID <= TeamCount; teamID++)
                            for (int playerID = 0; playerID <= ShipCount + 1; playerID++)
                            {
                                var py = Process.Start(new ProcessStartInfo()
                                {
                                    FileName = "cmd.exe",
                                    Arguments = "/c python"
                                        + Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python", "PyAPI", "main.py")
                                        + $" -I {IP} -P {Port} -t {teamID} -p {playerID} -o"
                                });
                                if (py is null)
                                {
                                    Log.LogError($"未能启动main.py, team:{teamID}, player: {playerID}!");
                                    return;
                                }
                                children.Add(py);
                            }
                    }
                    else
                    {
                        DebugAlert = "请构建proto后安装，参见“Help-Launch-Python proto构建”";
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = p
                        });
                    }
                }
            }
        }

        private string? debugAlert;
        public string? DebugAlert
        {
            get => debugAlert;
            set
            {
                debugAlert = value;
                OnPropertyChanged();
            }
        }
    }
}