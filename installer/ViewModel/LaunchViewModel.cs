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
            TeamID = Downloader.Data.Config.Commands.TeamID;
            PlayerID = Downloader.Data.Config.Commands.PlayerID;
            SweeperType = Downloader.Data.Config.Commands.SweeperType;
            PlaybackFile = Downloader.Data.Config.Commands.PlaybackFile;
            PlaybackSpeed = Downloader.Data.Config.Commands.PlaybackSpeed.ToString();

            ipChanged = false;
            portChanged = false;
            teamIDChanged = false;
            playerIDChanged = false;
            sweeperTypeChanged = false;
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
        private string? teamID;
        private string? playerID;
        private string? sweeperType;
        private string? playbackFile;
        private string? playbackSpeed;
        private bool cppSelect;
        private bool pySelect;

        private bool ipChanged;
        private bool portChanged;
        private bool teamIDChanged;
        private bool playerIDChanged;
        private bool sweeperTypeChanged;
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
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }
        public string? TeamID
        {
            get => teamID;
            set
            {
                teamID = value;
                if (teamID == Downloader.Data.Config.Commands.TeamID)
                    teamIDChanged = false;
                else
                    teamIDChanged = true;
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }
        public string? PlayerID
        {
            get => playerID;
            set
            {
                playerID = value;
                if (playerID == Downloader.Data.Config.Commands.PlayerID)
                    playerIDChanged = false;
                else
                    playerIDChanged = true;
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }
        public string? SweeperType
        {
            get => sweeperType;
            set
            {
                sweeperType = value;
                if (sweeperType == Downloader.Data.Config.Commands.SweeperType)
                    sweeperTypeChanged = false;
                else
                    sweeperTypeChanged = true;
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
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
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
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
                startEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !sweeperTypeChanged
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
                OnPropertyChanged();
            }
        }


        private bool saveEnabled;
        private bool startEnabled;
        public bool SaveEnabled
        {
            get => saveEnabled;
            set
            {
                saveEnabled = value;
                OnPropertyChanged();
            }
        }
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
                        && !teamIDChanged
                        && !playerIDChanged
                        && !sweeperTypeChanged
                        && !playbackFileChanged
                        && !playbackSpeedChanged
                        && !languageChanged;
        }
        private async Task StartBtnClicked()
        {
            await Task.Run(() => Start());
        }

        private void Save()
        {
            Task.Run(() => {
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
            Task.Run(() => {
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
            Task.Run(() => {
                try
                {
                    if (TeamID == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.TeamID = TeamID;
                    teamIDChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Team ID: " + e.Message;
                }
            });
            Task.Run(() => {
                try
                {
                    if (PlayerID == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.PlayerID = PlayerID;
                    playerIDChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Player ID: " + e.Message;
                }
            });
            Task.Run(() => {
                try
                {
                    if (SweeperType == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.SweeperType = SweeperType;
                    sweeperTypeChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Sweeper Type: " + e.Message;
                }
            });
            Task.Run(() => {
                try
                {
                    if (PlaybackFile == null)
                        throw new Exception("empty");
                    Downloader.Data.Config.Commands.PlaybackFile = PlaybackFile;
                    playbackFileChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Playback File: " + e.Message;
                }
            });
            Task.Run(() => {
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
            Task.Run(() => {
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
            DebugAlert = IP + " "
                       + Port + " "
                       + TeamID + " "
                       + PlayerID + " "
                       + SweeperType + " "
                       + PlaybackFile + " "
                       + PlaybackSpeed;
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