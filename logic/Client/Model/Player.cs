﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<Sweeper> ships;
        public ObservableCollection<Sweeper> Sweepers
        {
            get
            {
                return ships ?? (ships = new ObservableCollection<Sweeper>());
            }
            set
            {
                ships = value;
                OnPropertyChanged();
            }
        }
    }
}
