using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ObservableCollection<Exception> Exceptions { get; private set; }

        public InstallViewModel(IFolderPicker folderPicker, Model.Downloader downloader)
        {
            Downloader = downloader;
            FolderPicker = folderPicker;
            Exceptions = new ObservableCollection<Exception>();

            downloadPath = Downloader.Data.InstallPath;
            
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
        private string? debugAlert4;
        public string? DebugAlert4
        {
            get => debugAlert4;
            set
            {
                debugAlert4 = value;
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
                       !value.EndsWith(':') && !value.EndsWith('\\')
                    && Directory.Exists(value) && Local_Data.CountFile(value) == 0;
                CheckEnabled =
                       !value.EndsWith(':') && !value.EndsWith('\\')
                    && Directory.Exists(value) && Local_Data.CountFile(value) > 0;
                OnPropertyChanged();
            }
        }
        private bool browseEnabled = true;
        public bool BrowseEnabled
        {
            get => browseEnabled;
            set
            {
                browseEnabled = Downloader.Data.Installed && value;
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
        
        // private int cnt = 1;
        public void UpdateExceptions()
        {
            while (Downloader.Exceptions.Count > 0)
            {
                var exception = Downloader.Exceptions.Pop();
                if (exception != null)
                {
                    Exceptions.Add(exception);
                }
            }
            // Exceptions.Add(new Exception($"exp {cnt}"));
            // cnt ++;
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
            }
            else
            {
                DownloadPath = DownloadPath;
            }
            BrowseEnabled = true;
            UpdateExceptions();
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
            UpdateExceptions();
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
                await Task.Run(() => Downloader.Install(DownloadPath));
            }
            CheckEnabled = true;
            BrowseEnabled = true;
            UpdateExceptions();
            // DebugAlert2 = "Installed" + Downloader.Data.Installed.ToString();
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
            UpdateExceptions();
        }
    }
}