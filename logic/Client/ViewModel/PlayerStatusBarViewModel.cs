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

        public Player redPlayer, bluePlayer;
        public Player RedPlayer
        {
            get
            {
                return redPlayer ?? (redPlayer = new Player());
            }
            set
            {
                redPlayer = value;
                OnPropertyChanged();
            }
        }

        public Player BluePlayer
        {
            get => bluePlayer ?? (bluePlayer = new Player());
            set
            {
                bluePlayer = value;
                OnPropertyChanged();
            }
        }


    }

}
