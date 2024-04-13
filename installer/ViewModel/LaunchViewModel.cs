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
            TeamID = Downloader.Data.Config.Commands.TeamID.ToString();
            PlayerID = Downloader.Data.Config.Commands.PlayerID.ToString();
            ShipType = Downloader.Data.Config.Commands.ShipType.ToString();
            PlaybackFile = Downloader.Data.Config.Commands.PlaybackFile;
            PlaybackSpeed = Downloader.Data.Config.Commands.PlaybackSpeed.ToString();

            ipChanged = false;
            portChanged = false;
            teamIDChanged = false;
            playerIDChanged = false;
            shipTypeChanged = false;
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
        private string? shipType;
        private string? playbackFile;
        private string? playbackSpeed;
        private bool cppSelect;
        private bool pySelect;

        private bool ipChanged;
        private bool portChanged;
        private bool teamIDChanged;
        private bool playerIDChanged;
        private bool shipTypeChanged;
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                            && !shipTypeChanged
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
                if (teamID == Downloader.Data.Config.Commands.TeamID.ToString())
                    teamIDChanged = false;
                else
                    teamIDChanged = true;
                StartEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                if (playerID == Downloader.Data.Config.Commands.PlayerID.ToString())
                    playerIDChanged = false;
                else
                    playerIDChanged = true;
                StartEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
                OnPropertyChanged();
            }
        }
        public string? ShipType
        {
            get => shipType;
            set
            {
                shipType = value;
                if (shipType == Downloader.Data.Config.Commands.ShipType.ToString())
                    shipTypeChanged = false;
                else
                    shipTypeChanged = true;
                StartEnabled = !ipChanged
                            && !portChanged
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
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
                            && !teamIDChanged
                            && !playerIDChanged
                            && !shipTypeChanged
                            && !playbackFileChanged
                            && !playbackSpeedChanged
                            && !languageChanged;
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
                        && !shipTypeChanged
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
                    if (TeamID == null)
                        throw new Exception("empty");
                    var tmp = Convert.ToInt16(TeamID);
                    if (tmp != 0 && tmp != 1)
                        throw new Exception("Team ID can only be 0 or 1");
                    Downloader.Data.Config.Commands.TeamID = tmp;
                    teamIDChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Team ID: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    if (PlayerID == null)
                        throw new Exception("empty");
                    var tmp = Convert.ToInt16(PlayerID);
                    if (tmp < 0 || tmp > 3)
                        throw new Exception("Player ID can only be an integer from 0 to 3");
                    Downloader.Data.Config.Commands.PlayerID = tmp;
                    playerIDChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Player ID: " + e.Message;
                }
            });
            Task.Run(() =>
            {
                try
                {
                    if (ShipType == null)
                        throw new Exception("empty");
                    var tmp = Convert.ToInt16(ShipType);
                    if (tmp < 0 || tmp > 4)
                        throw new Exception("Ship Type can only be an integer from 0 to 4");
                    Downloader.Data.Config.Commands.ShipType = tmp;
                    shipTypeChanged = false;
                }
                catch (Exception e)
                {
                    DebugAlert = "Ship Type: " + e.Message;
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
            DebugAlert = IP + " "
                       + Port + " "
                       + TeamID + " "
                       + PlayerID + " "
                       + ShipType + " "
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