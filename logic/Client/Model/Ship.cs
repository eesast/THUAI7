using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    public enum TeamEnum
    {
        Red,
        Blue
    }

    public enum TypeEnum
    {
        CivilShip,
        WarShip,
        FlagShip
    }

    public enum StateEnum
    {
        Idle,
        Producing,
        Constructing,
        Recovering,
        Recycling,
        Attacking,
        Swinging,
        Deceased
    }

    public enum ProducerModuleEnum
    {

    }

    public enum ConstuctorModuleEnum
    {

    }

    public enum ArmorModuleEnum
    {

    }

    public enum ShieldModuleEnum
    {

    }

    public enum AttackerModuleEnum
    {

    }

    public class Ship : BindableObject
    {
        private TeamEnum team;
        private TypeEnum type;
        private StateEnum state;
        private int Hp;
        private ProducerModuleEnum producerModule;
        private ConstuctorModuleEnum constuctorModule;
        private ArmorModuleEnum armorModule;
        private ShieldModuleEnum shieldModule;
        private AttackerModuleEnum attackerModule;
        private string type_s;
        private string state_s;
        private string producerModule_s;
        private string constuctorModule_s;
        private string armorModule_s;
        private string shieldModule_s;
        private string attackerModule_s;
        public TeamEnum Team
        {
            get => team;
            set
            {
                team = value;
                OnPropertyChanged();
            }
        }
        public TypeEnum Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }
        public StateEnum State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }
        public int HP
        {
            get => Hp;
            set
            {
                Hp = value;
                OnPropertyChanged();
            }
        }
        public ProducerModuleEnum ProducerModule
        {
            get => producerModule;
            set
            {
                producerModule = value;
                OnPropertyChanged();
            }
        }
        public ConstuctorModuleEnum ConstuctorModule
        {
            get => constuctorModule;
            set
            {
                constuctorModule = value;
                OnPropertyChanged();
            }
        }
        public ArmorModuleEnum ArmorModule
        {
            get => armorModule;
            set
            {
                armorModule = value;
                OnPropertyChanged();
            }
        }
        public ShieldModuleEnum ShieldModule
        {
            get => shieldModule;
            set
            {
                shieldModule = value;
                OnPropertyChanged();
            }
        }
        public AttackerModuleEnum AttackerModule
        {
            get => attackerModule;
            set
            {
                attackerModule = value;
                OnPropertyChanged();
            }
        }
        public string Type_s
        {
            get => type_s;
            set
            {
                type_s = value;
                OnPropertyChanged();
            }
        }
        public string State_s
        {
            get => state_s;
            set
            {
                state_s = value;
                OnPropertyChanged();
            }
        }
        public string ProducerModule_s
        {
            get => producerModule_s;
            set
            {
                producerModule_s = value;
                OnPropertyChanged();
            }
        }
        public string ConstuctorModule_s
        {
            get => constuctorModule_s;
            set
            {
                constuctorModule_s = value;
                OnPropertyChanged();
            }
        }
        public string ArmorModule_s
        {
            get => armorModule_s;
            set
            {
                armorModule_s = value;
                OnPropertyChanged();
            }
        }
        public string ShieldModule_s
        {
            get => shieldModule_s;
            set
            {
                shieldModule_s = value;
                OnPropertyChanged();
            }
        }
        public string AttackerModule_s
        {
            get => attackerModule_s;
            set
            {
                attackerModule_s = value;
                OnPropertyChanged();
            }
        }
    }
}
