using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Storage;

namespace installer.ViewModel
{
    public class InstallViewModel : BaseViewModel
    {
        private readonly Model.Downloader Downloader;
        private readonly IFolderPicker FolderPicker;

        public InstallViewModel(IFolderPicker folderPicker, Model.Downloader downloader)
        {
            Downloader = downloader;
            FolderPicker = folderPicker;
            downloadPath = Downloader.Data.InstallPath;
            installEnabled = false;
            BrowseBtnClickedCommand = new AsyncRelayCommand(BrowseBtnClicked);
            CheckUpdBtnClickedCommand = new RelayCommand(CheckUpdBtnClicked);
            DownloadBtnClickedCommand = new AsyncRelayCommand(DownloadBtnClicked);
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
        private string downloadPath;
        public string DownloadPath
        {
            get => downloadPath;
            set
            {
                downloadPath = value;
                InstallEnabled = (value != Downloader.Data.InstallPath && Directory.Exists(value));
                OnPropertyChanged();
            }
        }

        public ICommand BrowseBtnClickedCommand { get; }
        private async Task BrowseBtnClicked()
        {
            DebugAlert = "Browse Button Clicked";
            var b = InstallEnabled;
            InstallEnabled = false;
            var result = await FolderPicker.PickAsync(downloadPath);
            if (result.IsSuccessful)
            {
                DownloadPath = result.Folder.Path;
            }
            else
            {
                InstallEnabled = b;
            }
        }
        public ICommand CheckUpdBtnClickedCommand { get; }
        private async void CheckUpdBtnClicked()
        {
            DebugAlert = "Check Button Clicked";
            bool updated = await Task.Run(() => Downloader.CheckUpdate());
            if (updated)
            {
                DebugAlert = "Need to update.";
            }
            else
            {
                DebugAlert = "Nothing to update.";
            }
        }
        public ICommand DownloadBtnClickedCommand { get; }
        private async Task DownloadBtnClicked()
        {
            if (!InstallEnabled)
                return;
            InstallEnabled = false;
            DebugAlert = "Download Button Clicked";
            await Task.Run(() => Downloader.ResetInstallPath(downloadPath));
            await Task.Run(() => Downloader.Install());
            DownloadPath = Downloader.Data.InstallPath;
            InstallEnabled = true;
        }

        private bool installEnabled;
        public bool InstallEnabled
        {
            get => installEnabled;
            set
            {
                installEnabled = value;
                OnPropertyChanged("CheckEnabled");
                OnPropertyChanged();
            }
        }
        public bool CheckEnabled
        {
            get => !installEnabled;
        }
    }
}
