using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace installer.ViewModel
{
    internal class LoginViewModel : NotificationObject
    {
        bool enabled = false;
        string txt1 = "False";

        public string Txt1
        {
            get => txt1;
            set { txt1 = value; OnPropertyChanged(); }
        }

        public ICommand BtnClickedCommand { get; }

        public LoginViewModel()
        {
            BtnClickedCommand = new AsyncRelayCommand(BtnClicked);
        }

        private async Task BtnClicked()
        {
            enabled = !enabled;
            Txt1 = enabled.ToString();
        }

    }

}
