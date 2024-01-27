using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace installer.ViewModel
{
    internal class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            LoginBtnClickedCommand = new RelayCommand(LoginBtnClicked);
        }

        Model.Downloader Downloader
        {
            get => MauiProgram.Downloader;
        }
        public string Username
        {
            get => Downloader.Username;
            set
            {
                Downloader.Username = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get => Downloader.Password;
            set
            {
                Downloader.Password = value;
                OnPropertyChanged();
            }
        }
        string? id;
        public string? ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        public bool Remember
        {
            get => Downloader.RememberMe;
            set
            {
                Downloader.RememberMe = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginBtnClickedCommand { get; }
        private void LoginBtnClicked()
        {
            var task = Downloader.Login();
            task.ContinueWith(t =>
            {
                ID = Downloader.UserId;
                if (Remember)
                    Downloader.RememberUser();
                else
                    Downloader.ForgetUser();
            });
        }
    }
}