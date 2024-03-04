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

    public class Ship
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
            set => team = value;
        }
        public TypeEnum Type
        {
            get => type;
            set => type = value;
        }
        public StateEnum State
        {
            get => state;
            set => state = value;
        }
        public int HP
        {
            get => Hp;
            set => Hp = value;
        }
        public ProducerModuleEnum ProducerModule
        {
            get => producerModule;
            set => producerModule = value;
        }
        public ConstuctorModuleEnum ConstuctorModule
        {
            get => constuctorModule;
            set => constuctorModule = value;
        }
        public ArmorModuleEnum ArmorModule
        {
            get => armorModule;
            set => armorModule = value;
        }
        public ShieldModuleEnum ShieldModule
        {
            get => shieldModule;
            set => shieldModule = value;
        }
        public AttackerModuleEnum AttackerModule
        {
            get => attackerModule;
            set => attackerModule = value;
        }
        public string Type_s
        {
            get => type_s;
            set => type_s = value;
        }
        public string State_s
        {
            get => state_s;
            set => state_s = value;
        }
        public string ProducerModule_s
        {
            get => producerModule_s;
            set => producerModule_s = value;
        }
        public string ConstuctorModule_s
        {
            get => constuctorModule_s;
            set => constuctorModule_s = value;
        }
        public string ArmorModule_s
        {
            get => armorModule_s;
            set => armorModule_s = value;
        }
        public string ShieldModule_s
        {
            get => shieldModule_s;
            set => shieldModule_s = value;
        }
        public string AttackerModule_s
        {
            get => attackerModule_s;
            set => attackerModule_s = value;
        }
    }
}
