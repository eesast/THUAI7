using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public class Player
    {
        private bool team;
        public bool Team
        {
            get => team;
            set => team = value;
        }
        private int hp;
        public int Hp
        {
            get => hp;
            set => hp = value;
        }
        private int money;
        public int Money
        {
            get => money;
            set => money = value;
        }
        private List<Ship> ships;
        public List<Ship> Ships
        {
            get
            {
                return ships ?? (ships = new List<Ship>());
            }
            set
            {
                ships = value;
            }
        }
    }
}
