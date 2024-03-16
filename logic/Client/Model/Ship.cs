using Client.Util;
using Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    //public enum TeamEnum
    //{
    //    Red,
    //    Blue
    //}

    //public enum TypeEnum
    //{
    //    CivilShip,
    //    WarShip,
    //    FlagShip
    //}

    //public enum StateEnum
    //{
    //    Idle,
    //    Producing,
    //    Constructing,
    //    Recovering,
    //    Recycling,
    //    Attacking,
    //    Swinging,
    //    Deceased
    //}

    //public enum ProducerModuleEnum
    //{

    //}

    //public enum ConstuctorModuleEnum
    //{

    //}

    //public enum ArmorModuleEnum
    //{

    //}

    //public enum ShieldModuleEnum
    //{

    //}

    //public enum AttackerModuleEnum
    //{

    //}

    public class Ship : BindableObject
    {
        private long teamID;
        private ShipType type;
        private ShipState state;
        private int Hp;
        private ProducerType producerModule;
        private ConstructorType constuctorModule;
        private ArmorType armorModule;
        private ShieldType shieldModule;
        private WeaponType weaponModule;
        private string type_s;
        private string state_s;
        private string producerModule_s;
        private string constuctorModule_s;
        private string armorModule_s;
        private string shieldModule_s;
        private string weaponModule_s;
        

        public long TeamID
        {
            get => teamID;
            set
            {
                teamID = value;
                OnPropertyChanged();
            }
        }
        public ShipType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }
        public ShipState State
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
        public ProducerType ProducerModule
        {
            get => producerModule;
            set
            {
                producerModule = value;
                OnPropertyChanged();
            }
        }
        public ConstructorType ConstuctorModule
        {
            get => constuctorModule;
            set
            {
                constuctorModule = value;
                OnPropertyChanged();
            }
        }
        public ArmorType ArmorModule
        {
            get => armorModule;
            set
            {
                armorModule = value;
                OnPropertyChanged();
            }
        }
        public ShieldType ShieldModule
        {
            get => shieldModule;
            set
            {
                shieldModule = value;
                OnPropertyChanged();
            }
        }
        public WeaponType WeaponModule
        {
            get => weaponModule;
            set
            {
                weaponModule = value;
                OnPropertyChanged();
            }
        }
        public string Type_s
        {
            get => UtilInfo.ShipTypeNameDict[Type];
            set
            {
                type_s = value;
                OnPropertyChanged();
            }
        }
        public string State_s
        {
            get => UtilInfo.ShipStateNameDict[State];
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
        public string WeaponModule_s
        {
            get => weaponModule_s;
            set
            {
                weaponModule_s = value;
                OnPropertyChanged();
            }
        }
    }
}
