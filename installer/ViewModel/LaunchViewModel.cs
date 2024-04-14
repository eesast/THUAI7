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

namespace installer.ViewModel
{
    public class LaunchViewModel : BaseViewModel
    {
        private readonly Downloader Downloader;
        public LaunchViewModel(Downloader downloader)
        {
            Downloader = downloader;

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
                StartEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                startEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                StartEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                StartEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                StartEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                StartEnabled = !ipChanged
                            && !portChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }


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
                startEnabled = value;
                OnPropertyChanged();
            }
        }


        public ICommand SaveBtnClickedCommand { get; }
        public ICommand StartBtnClickedCommand { get; }

        private async Task SaveBtnClicked()
        {
            DebugAlert = "Save";
            await Task.Run(() => Save());
            StartEnabled = !ipChanged
                        && !portChanged
                        && !playbackFileChanged
                        && !playbackSpeedChanged
                        && !languageChanged;
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

        private void Start()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Server", "Server.exe"),
                Arguments = $"--ip {IP} --port {Port}"
            });
            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
            });
            
            if (CppSelect && string.IsNullOrEmpty(PlaybackFile))
            {
                for (int teamID = 0; teamID <= 1; teamID++)
                    for (int playerID = 0; playerID <= 4; playerID++)
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "cpp", "x64", "Debug", "API.exe"),
                            Arguments = $"-I {IP} -P {Port} -t {teamID} -p {playerID} -d"
                        });
            }
            else if (PySelect && string.IsNullOrEmpty(PlaybackFile))
            {
                for (int teamID = 0; teamID <= 1; teamID++)
                    for (int playerID = 0; playerID <= 4; playerID++)
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c python"
                                + Path.Combine(Downloader.Data.Config.InstallPath, "CAPI", "python", "PyAPI", "main.py")
                                + $"-I {IP} -P {Port} -t {teamID} -p {playerID} -d"
                        });
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