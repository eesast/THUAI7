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
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Storage;
using System.Windows.Input;

namespace installer.ViewModel
{
    public class PlaybackViewModel : LaunchViewModel
    {
        private readonly IFilePicker FilePicker;
        public PlaybackViewModel(IFilePicker filePicker, Downloader downloader) : base(downloader)
        {
            FilePicker = filePicker;

            options = new PickOptions()
            {
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new [] { "application/com.thueesast.thuaiplayback" } },
                        { DevicePlatform.iOS, new [] { "com.thueesast.thuaiplayback" } },
                        { DevicePlatform.WinUI, new [] { ".thuaipb" } },
                        { DevicePlatform.macOS, new [] { "thuaipb" } }
                    }
                ),
                PickerTitle = "Please select a playback file"
            };

            BrowseBtnClickedCommand = new RelayCommand(BrowseBtnClicked);
            PlaybackStartBtnClickedCommand = new AsyncRelayCommand(PlaybackStartBtnClicked);
        }
        public ICommand BrowseBtnClickedCommand { get; }

        public IAsyncRelayCommand PlaybackStartBtnClickedCommand { get; }

        protected PickOptions options;


        private bool browseEnabled = true;
        public bool BrowseEnabled
        {
            get => browseEnabled;
            set
            {
                browseEnabled = value;
                OnPropertyChanged();
            }
        }

        private async Task PlaybackStartBtnClicked()
        {
            PlaybackFile = playbackFile;
            if (PlaybackFile == "")
                return;
            await Task.Run(() => LaunchPlayback());
        }

        private void BrowseBtnClicked()
        {
            BrowseEnabled = false;
            FilePicker.PickAsync(options).ContinueWith(result =>
            {
                if (result is not null)
                {
                    var p = result?.Result?.FullPath;
                    PlaybackFile = string.IsNullOrEmpty(p) ? PlaybackFile : p;
                }
            });
            BrowseEnabled = true;
        }

        public bool LaunchPlayback()
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(Downloader.Data.Config.InstallPath, "logic", "Client", "Client.exe"),
            }) is not null;
        }
    }
}
