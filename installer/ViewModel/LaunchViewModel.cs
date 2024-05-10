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
        protected Downloader Downloader;
        public bool Installed { get => Downloader.Data.Config.Installed; }

        protected ListLogger Log = new ListLogger();
        public ObservableCollection<LogRecord> LogList { get => Log.List; }

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
            Log.PartnerInfo = "[Launcher]";
            Log.Partner.Add(Downloader.Log);

            SaveBtnClickedCommand = new AsyncRelayCommand(SaveBtnClicked);
        }


        #region Parameters
        protected string? ip;
        protected string? port;
        protected string? playbackFile;
        protected string? playbackSpeed;
        protected bool cppSelect;
        protected bool pySelect;

        protected bool ipChanged;
        protected bool portChanged;
        protected bool playbackFileChanged;
        protected bool playbackSpeedChanged;
        protected bool languageChanged;

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
                if (!(value?.EndsWith(".thuaipb") ?? false))
                    return;
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
        #endregion


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


        #region Save
        public IAsyncRelayCommand SaveBtnClickedCommand { get; }
        private async Task SaveBtnClicked()
        {
            DebugAlert = "Save";
            await Task.Run(() => Save());
            StartEnabled = true;
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
        #endregion
    }
}