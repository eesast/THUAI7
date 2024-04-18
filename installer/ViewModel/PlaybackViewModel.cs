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

namespace installer.ViewModel
{
    public class PlaybackViewModel : LaunchViewModel
    {
        public PlaybackViewModel(Downloader downloader) : base(downloader)
        {
            PlaybackStartBtnClickedCommand = new AsyncRelayCommand(PlaybackStartBtnClicked);
        }

        public IAsyncRelayCommand PlaybackStartBtnClickedCommand { get; }
        private async Task PlaybackStartBtnClicked()
        {
            if (PlaybackFile == "")
                return;
            await Task.Run(() => LaunchPlayback());
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
