using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class Player : BindableObject
    {
        private bool team;
        public bool Team
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
        private ObservableCollection<Ship> ships;
        public ObservableCollection<Ship> Ships
        {
            get
            {
                return ships ?? (ships = new ObservableCollection<Ship>());
            }
            set
            {
                ships = value;
                OnPropertyChanged();
            }
        }
    }
}
