using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace installer.ViewModel
{
    class MainViewModel : NotificationObject
    {
        public MainViewModel()
        {
            CounterClickedCommand = new RelayCommand(CounterClicked);
        }

        Model.Downloader Downloader { get => MauiProgram.Downloader; }
        private string count_txt = "Haven't clicked yet.";
        private string slider_txt = "0";
        private int count = 0;
        private double slider = 0;

        public string Count_Txt
        {
            get => count_txt;
            set { count_txt = value; OnPropertyChanged(); }
        }

        public double Slider
        {
            get => slider;
            set { slider = value; Slider_Txt = $"{slider}"; OnPropertyChanged(); }
        }

        public string Slider_Txt
        {
            get => slider_txt;
            set { slider_txt = value; OnPropertyChanged(); }
        }

        public ICommand CounterClickedCommand { get; }

        private void CounterClicked()
        {
            count++;

            if (count == 1)
                Count_Txt = $"Clicked {count} time";
            else
                Count_Txt = $"Clicked {count} times";

            SemanticScreenReader.Announce(count_txt);
        }
    }
}
