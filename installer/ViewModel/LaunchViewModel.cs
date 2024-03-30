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

            PlaybackSpeed = Downloader.Data.Config.Commands.PlaybackSpeed.ToString();
            StartEnabled = true;

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


        public string IP
        {
            get => Downloader.Data.Config.Commands.IP;
            set
            {
                Downloader.Data.Config.Commands.IP = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get => Downloader.Data.Config.Commands.Port;
            set
            {
                Downloader.Data.Config.Commands.Port = value;
                OnPropertyChanged();
            }
        }

        public string TeamID
        {
            get => Downloader.Data.Config.Commands.TeamID;
            set
            {
                Downloader.Data.Config.Commands.TeamID = value;
                OnPropertyChanged();
            }
        }

        public string PlayerID
        {
            get => Downloader.Data.Config.Commands.PlayerID;
            set
            {
                Downloader.Data.Config.Commands.PlayerID = value;
                OnPropertyChanged();
            }
        }

        public string SweeperType
        {
            get => Downloader.Data.Config.Commands.SweeperType;
            set
            {
                Downloader.Data.Config.Commands.SweeperType = value;
                OnPropertyChanged();
            }
        }

        public string PlaybackFile
        {
            get => Downloader.Data.Config.Commands.PlaybackFile;
            set
            {
                Downloader.Data.Config.Commands.PlaybackFile = value;
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
                try
                {
                    Downloader.Data.Config.Commands.PlaybackSpeed = Convert.ToDouble(value);
                    DebugAlert = null;
                    StartEnabled = true;
                }
                catch (Exception e)
                {
                    DebugAlert = e.ToString();
                    StartEnabled = false;
                }
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


        public ICommand StartBtnClickedCommand { get; }
        private async Task StartBtnClicked()
        {
            await Task.Run(() => Start());
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