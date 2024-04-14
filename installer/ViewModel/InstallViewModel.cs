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
using installer.Services;

namespace installer.ViewModel
{
    public class InstallViewModel : BaseViewModel
    {
        private readonly Downloader Downloader;
        private readonly IFolderPicker FolderPicker;
        public ObservableCollection<LogRecord> LogCollection { get => Downloader.LogList.List; }

        public InstallViewModel(IFolderPicker folderPicker, Downloader downloader)
        {
            Downloader = downloader;
            FolderPicker = folderPicker;

            downloadPath = Downloader.Data.Config.InstallPath;

            BrowseBtnClickedCommand = new AsyncRelayCommand(BrowseBtnClicked);
            CheckUpdBtnClickedCommand = new RelayCommand(CheckUpdBtnClicked);
            DownloadBtnClickedCommand = new AsyncRelayCommand(DownloadBtnClicked);
            UpdateBtnClickedCommand = new AsyncRelayCommand(UpdateBtnClicked);

            Downloader.CloudReport.PropertyChanged += ProgressReport;
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
                DownloadEnabled =
                       !value.EndsWith(':') && !value.EndsWith('\\')
                    && Directory.Exists(value) && Local_Data.CountFile(value) == 0;
                CheckEnabled =
                       !value.EndsWith(':') && !value.EndsWith('\\')
                    && Directory.Exists(value) && Local_Data.CountFile(value) > 0;
                OnPropertyChanged();
            }
        }

        #region 进度报告区
        private double numPro = 0;
        public double NumPro
        {
            get => numPro; set
            {
                numPro = value;
                OnPropertyChanged();
            }
        }

        private string numReport = string.Empty;
        public string NumReport
        {
            get => numReport; set
            {
                numReport = value;
                OnPropertyChanged();
            }
        }

        private double filePro = 0;
        public double FilePro
        {
            get => filePro; set
            {
                filePro = value;
                OnPropertyChanged();
            }
        }

        private string fileReport = string.Empty;
        public string FileReport
        {
            get => fileReport; set
            {
                fileReport = value;
                OnPropertyChanged();
            }
        }

        private bool bigFileProEnabled = false;
        public bool BigFileProEnabled
        {
            get => bigFileProEnabled; set
            {
                bigFileProEnabled = value;
                OnPropertyChanged();
            }
        }

        private void ProgressReport(object? sender, EventArgs e)
        {
            var r = Downloader.CloudReport;
            NumPro = r.Count == 0 ? 1 : (double)r.ComCount / r.Count;
            NumReport = $"{r.ComCount} / {r.Count}";
            FilePro = r.Total == 0 ? 1 : (double)r.Completed / r.Total;
            FileReport = $"{FileService.GetFileSizeReport(r.Completed)} / {FileService.GetFileSizeReport(r.Total)}";
            BigFileProEnabled = r.BigFileTraceEnabled;
        }
        #endregion

        private bool installed;
        public bool Installed
        {
            get => installed;
            set
            {
                installed = value;
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

        public ICommand BrowseBtnClickedCommand { get; }
        private async Task BrowseBtnClicked()
        {
            // DebugAlert = "Browse Button Clicked";
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
        }
        public ICommand CheckUpdBtnClickedCommand { get; }
        private async void CheckUpdBtnClicked()
        {
            // DebugAlert = "Check Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            bool updated = await Downloader.CheckUpdateAsync();
            if (updated)
            {
                DebugAlert = "Need to update.";
                UpdateEnabled = true;
            }
            else
            {
                DebugAlert = "Nothing to update.";
                UpdateEnabled = false;
            }
            BrowseEnabled = true;
            CheckEnabled = true;
        }
        public ICommand DownloadBtnClickedCommand { get; }
        private async Task DownloadBtnClicked()
        {
            // DebugAlert = "Download Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            if (Downloader.Data.Installed)
            {
                await Downloader.ResetInstallPathAsync(DownloadPath);
            }
            else
            {
                await Downloader.InstallAsync(DownloadPath);
            }
            Installed = Downloader.Data.Installed;
            CheckEnabled = true;
            BrowseEnabled = true;
        }
        public ICommand UpdateBtnClickedCommand { get; }
        private async Task UpdateBtnClicked()
        {
            // DebugAlert = "Update Button Clicked";
            BrowseEnabled = false;
            CheckEnabled = false;
            DownloadEnabled = false;
            UpdateEnabled = false;
            await Downloader.UpdateAsync();
            CheckEnabled = true;
            BrowseEnabled = true;
        }
    }
}