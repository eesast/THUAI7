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

            SaveEnabled = true;
            StartEnabled = true;

            SaveBtnClickedCommand = new AsyncRelayCommand(SaveBtnClicked);
            StartBtnClickedCommand = new AsyncRelayCommand(StartBtnClicked);
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


        private string? ip;
        public string? IP
        {
            get => ip;
            set
            {
                ip = value;
                if (ip == Downloader.Data.Config.Commands.IP)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        private string? port;
        public string? Port
        {
            get => port;
            set
            {
                port = value;
                if (port == Downloader.Data.Config.Commands.Port)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        private string? teamID;
        public string? TeamID
        {
            get => teamID;
            set
            {
                teamID = value;
                if (teamID == Downloader.Data.Config.Commands.TeamID)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        private string? playerID;
        public string? PlayerID
        {
            get => playerID;
            set
            {
                playerID = value;
                if (playerID == Downloader.Data.Config.Commands.PlayerID)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        private string? sweeperType;
        public string? SweeperType
        {
            get => sweeperType;
            set
            {
                sweeperType = value;
                if (sweeperType == Downloader.Data.Config.Commands.SweeperType)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        private string? playbackFile;
        public string? PlaybackFile
        {
            get => playbackFile;
            set
            {
                playbackFile = value;
                if (playbackFile == Downloader.Data.Config.Commands.PlaybackFile)
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
                OnPropertyChanged();
            }
        }

        public string? playbackSpeed;
        public string? PlaybackSpeed
        {
            get => playbackSpeed;
            set
            {
                playbackSpeed = value;
                if (playbackSpeed == Downloader.Data.Config.Commands.PlaybackSpeed.ToString())
                {
                    StartEnabled = true;
                }
                else
                {
                    StartEnabled = false;
                }
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
        private async Task SaveBtnClicked()
        {
            DebugAlert = "Save";
            var cntExp = await Task.Run(() => Save());
            if (cntExp == 0)
            {
                StartEnabled = true;
            }
        }

        public ICommand StartBtnClickedCommand { get; }
        private async Task StartBtnClicked()
        {
            await Task.Run(() => Start());
        }

        private int Save()
        {
            int cntExp = 0;
            Task.Run(() => {
                try
                {
                    if (IP == null)
                    {
                        throw new Exception("empty");
                    }
                    Downloader.Data.Config.Commands.IP = IP;
                }
                catch (Exception e)
                {
                    DebugAlert = "IP: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    if (Port == null)
                    {
                        throw new Exception("empty");
                    }
                    Downloader.Data.Config.Commands.Port = Port;
                }
                catch (Exception e)
                {
                    if (Port == null)
                    {
                        throw new Exception("empty");
                    }
                    DebugAlert = "Port: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    if (TeamID == null)
                    {
                        throw new Exception("empty");
                    }
                    Downloader.Data.Config.Commands.TeamID = TeamID;
                }
                catch (Exception e)
                {
                    DebugAlert = "Team ID: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    if (PlayerID == null)
                    {
                        throw new Exception("empty");
                    }
                    Downloader.Data.Config.Commands.PlayerID = PlayerID;
                }
                catch (Exception e)
                {
                    DebugAlert = "Player ID: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    if (SweeperType == null)
                    {
                        throw new Exception("empty");
                    }
                    Downloader.Data.Config.Commands.SweeperType = SweeperType;
                }
                catch (Exception e)
                {
                    DebugAlert = "Sweeper Type: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    Downloader.Data.Config.Commands.PlaybackFile = PlaybackFile;
                }
                catch (Exception e)
                {
                    DebugAlert = "Playback File: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            Task.Run(() => {
                try
                {
                    Downloader.Data.Config.Commands.PlaybackSpeed = Convert.ToDouble(PlaybackSpeed);
                }
                catch (Exception e)
                {
                    DebugAlert = "Playback Speed: " + e.Message;
                    StartEnabled = false;
                    cntExp++;
                }
            });
            return cntExp;
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
    }
}