using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using installer.Model;

namespace installer.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly Downloader Downloader;

        public LoginViewModel(Downloader downloader)
        {
            Downloader = downloader;

            LoginBtnClickedCommand = new AsyncRelayCommand(LoginBtnClicked);
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

        string loginStatus = "offline";
        public string LoginStatus
        {
            get => loginStatus;
            set
            {
                loginStatus = value;
                OnPropertyChanged();
            }
        }

        string remStatus = "false";
        public string RemStatus
        {
            get => remStatus;
            set
            {
                remStatus = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginBtnClickedCommand { get; }
        private async Task LoginBtnClicked()
        {
            await Downloader.LoginAsync()
                .ContinueWith(t =>
                {
                    ID = Downloader.Username;
                    if (Downloader.Web.Status == Model.LoginStatus.logined)
                    {
                        if (Remember)
                            Downloader.RememberUser();
                        else
                            Downloader.ForgetUser();
                    }
                    LoginStatus = Downloader.Web.Status.ToString();
                    RemStatus = Remember.ToString();
                });
        }
    }
}