using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace installer.ViewModel
{
    public class LaunchViewModel : BaseViewModel
    {
        public LaunchViewModel()
        {
            ip = "127.0.0.1";
            port = "8888";
            teamID = "0";
            playerID = "0";
            sweeperType = "0";
            playbackFile = "D:\\Playback";
            playbackSpeed = "2.0";

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

        private string ip;
        public string IP
        {
            get => ip;
            set
            {
                ip = value;
                OnPropertyChanged();
            }
        }
        private string port;
        public string Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged();
            }
        }
        private string teamID;
        public string TeamID
        {
            get => teamID;
            set
            {
                teamID = value;
                OnPropertyChanged();
            }
        }
        private string playerID;
        public string PlayerID
        {
            get => playerID;
            set
            {
                playerID = value;
                OnPropertyChanged();
            }
        }
        private string sweeperType;
        public string SweeperType
        {
            get => sweeperType;
            set
            {
                sweeperType = value;
                OnPropertyChanged();
            }
        }
        private string playbackFile;
        public string PlaybackFile
        {
            get => playbackFile;
            set
            {
                playbackFile = value;
                OnPropertyChanged();
            }
        }
        private string playbackSpeed;
        public string PlaybackSpeed
        {
            get => playbackSpeed;
            set
            {
                playbackSpeed = value;
                OnPropertyChanged();
            }
        }

        private bool startEnabled;
        public bool StartEnabled
        {
            get => startEnabled;
            set
            {
                startEnabled = true;
                OnPropertyChanged();
            }
        }

        public ICommand StartBtnClickedCommand { get; }
        private async Task StartBtnClicked()
        {
            await Task.Run(() => Start());
        }

        private void Start()
        {
            DebugAlert = IP.ToString() + " "
                       + Port.ToString() + " "
                       + TeamID.ToString() + " "
                       + PlayerID.ToString() + " "
                       + SweeperType.ToString() + " "
                       + PlaybackFile.ToString() + " "
                       + PlaybackSpeed.ToString();
        }
    }
}