using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Model;
using Protobuf;

namespace Client.ViewModel
{
    public partial class GeneralViewModel
    {
        private int time;
        private int wormHole1HP;
        private int wormHole2HP;
        private int wormHole3HP;
        public int Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }
        public int WormHole1HP
        {
            get => wormHole1HP;
            set
            {
                wormHole1HP = value;
                OnPropertyChanged();
            }
        }
        public int WormHole2HP
        {
            get => wormHole2HP;
            set
            {
                wormHole2HP = value;
                OnPropertyChanged();
            }
        }
        public int WormHole3HP
        {
            get => wormHole3HP;
            set
            {
                wormHole3HP = value;
                OnPropertyChanged();
            }
        }

        private bool haveSetSlideLength = false;
        double lengthOfWormHole1HpSlide = 80;
        double lengthOfWormHole2HpSlide = 80;
        double lengthOfWormHole3HpSlide = 80;
        private string gameTime;

        private readonly int WormHoleFullHp = 18000;
        public string GameTime
        {
            get => gameTime;
            set
            {
                gameTime = value;
                OnPropertyChanged();
            }
        }
        public void SetGameTimeValue(MessageOfAll obj)
        {
            int min, sec;
            sec = obj.GameTime / 1000;
            min = sec / 60;
            sec = sec % 60;
            GameTime = "时间：";
            if (min / 10 == 0)
            {
                GameTime += "0";
            }
            GameTime += min.ToString() + ":";
            if (sec / 10 == 0)
            {
                GameTime += "0";
            }
            GameTime += sec.ToString();
        }

        public int WormHole1Length
        {
            get => wormHole1HP / 100 * (int)lengthOfWormHole1HpSlide;
        }
        public int WormHole2Length
        {
            get => wormHole2HP / 100 * (int)lengthOfWormHole2HpSlide;
        }
        public int WormHole3Length
        {
            get => wormHole3HP / 100 * (int)lengthOfWormHole3HpSlide;
        }

    }
}
