using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Storage;
using installer.Model;

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
            DebugAlert3 = Downloader.Data.Installed.ToString();
            BrowseBtnClickedCommand = new AsyncRelayCommand(BrowseBtnClicked);
            CheckUpdBtnClickedCommand = new RelayCommand(CheckUpdBtnClicked);
            DownloadBtnClickedCommand = new AsyncRelayCommand(DownloadBtnClicked);
            UpdateBtnClickedCommand = new AsyncRelayCommand(UpdateBtnClicked);
        }

        private string? debugAlert1;
        public string? DebugAlert1
        {
            get => debugAlert1;
            set
            {
                debugAlert1 = value;
                OnPropertyChanged();
            }
        }
        private string? debugAlert2;
        public string? DebugAlert2
        {
            get => debugAlert2;
            set
            {
                debugAlert2 = value;
                OnPropertyChanged();
            }
        }
        private string? debugAlert3;
        public string? DebugAlert3
        {
            get => debugAlert3;
            set
            {
                debugAlert3 = value;
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
                DownloadEnabled =
                       Directory.Exists(value)
                    && Directory.GetFiles(value).Length == 0 && Directory.GetDirectories(value).Length == 0;
                CheckEnabled =
                       Directory.Exists(value)
                    && (Directory.GetFiles(value).Length > 0 || Directory.GetDirectories(value).Length > 0);
                OnPropertyChanged();
            }
        }
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
        private bool checkEnabled = false;
        public bool CheckEnabled
        {
            get => checkEnabled;
            set
            {
                checkEnabled = value;
                OnPropertyChanged();
            }
        }
        private bool downloadEnabled = false;
        public bool DownloadEnabled
        {
            get => downloadEnabled;
            set
            {
                downloadEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool updateEnabled = false;
        public bool UpdateEnabled
        {
            get => updateEnabled;
            set
            {
                updateEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseBtnClickedCommand { get; }
        private async Task BrowseBtnClicked()
        {
            DebugAlert1 = "Browse Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            var result = await FolderPicker.PickAsync(DownloadPath);
            if (result.IsSuccessful)
            {
                DownloadPath = result.Folder.Path;
                // DebugAlert2 = result.Folder.Path.ToString();
            }
            else
            {
                DownloadPath = DownloadPath;
            }
            BrowseEnabled = true;

        }
        public ICommand CheckUpdBtnClickedCommand { get; }
        private async void CheckUpdBtnClicked()
        {
            DebugAlert1 = "Check Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            bool updated = await Task.Run(() => Downloader.CheckUpdate());
            if (updated)
            {
                DebugAlert1 = "Need to update.";
                UpdateEnabled = true;
            }
            else
            {
                DebugAlert1 = "Nothing to update.";
                UpdateEnabled = false;
            }
            BrowseEnabled = true;
            CheckEnabled = true;
        }
        public ICommand DownloadBtnClickedCommand { get; }
        private async Task DownloadBtnClicked()
        {
            DebugAlert1 = "Download Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            if (Downloader.Data.Installed)
            {
                await Task.Run(() => Downloader.ResetInstallPath(DownloadPath));
            }
            else
            {
                await Task.Run(() => Downloader.Install());
            }
            DebugAlert2 = Downloader.Data.InstallPath;
            CheckEnabled = true;
            BrowseEnabled = true;
        }
        public ICommand UpdateBtnClickedCommand { get; }
        private async Task UpdateBtnClicked()
        {
            DebugAlert1 = "Update Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            await Task.Run(() => Downloader.Update());
            CheckEnabled = true;
            BrowseEnabled = true;
        }
    }
}