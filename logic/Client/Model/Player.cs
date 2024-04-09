using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class Player : BindableObject
    {
        private long team;
        public long Team
        {
            get => team;
            set
            {
                team = value;
                OnPropertyChanged();
            }
        }
        private int hp;
        public int Hp
        {
            get => hp;
            set
            {
                hp = value;
                OnPropertyChanged();
            }
        }
        private int money;
        public int Money
        {
            get => money;
            set
            {
                money = value;
                OnPropertyChanged();
            }
        }

        private int score;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<Ship> ships;
        public ObservableCollection<Ship> Ships
        {
            get
            {
                return ships ?? (ships = new ObservableCollection<Ship>());
            }
            set
            {
                if (value != null && value.Count > 0) 
                {
                    ships = value;
                    OnPropertyChanged(nameof(Sweepers));
                }
            }
        }
    }
}
