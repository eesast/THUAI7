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
    //    CivilSweeper,
    //    WarSweeper,
    //    FlagSweeper
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

    public class Sweeper : BindableObject
    {
        private long teamID;
        private SweeperType type;
        private SweeperState state;
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
        public SweeperType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }
        public SweeperState State
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
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public ConstructorType ConstuctorModule
        {
            get => constuctorModule;
            set
            {
                constuctorModule = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public ArmorType ArmorModule
        {
            get => armorModule;
            set
            {
                armorModule = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public ShieldType ShieldModule
        {
            get => shieldModule;
            set
            {
                shieldModule = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public WeaponType WeaponModule
        {
            get => weaponModule;
            set
            {
                weaponModule = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string Type_s
        {
            get => UtilInfo.SweeperTypeNameDict[Type];
            //get => type_s;
            set
            {
                type_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string State_s
        {
            get => UtilInfo.SweeperStateNameDict[State];
            //get => state_s;
            set
            {
                state_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string ProducerModule_s
        {
            get => UtilInfo.SweeperProducerTypeNameDict[ProducerModule];
            //get => producerModule_s;
            set
            {
                producerModule_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string ConstuctorModule_s
        {
            get => UtilInfo.SweeperConstructorNameDict[ConstuctorModule];
            //get => constuctorModule_s;
            set
            {
                constuctorModule_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string ArmorModule_s
        {
            get => UtilInfo.SweeperArmorTypeNameDict[ArmorModule];
            //get => armorModule_s;
            set
            {
                armorModule_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string ShieldModule_s
        {
            get => UtilInfo.SweeperShieldTypeNameDict[ShieldModule];
            //get => shieldModule_s;
            set
            {
                shieldModule_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
        public string WeaponModule_s
        {
            get => UtilInfo.SweeperWeaponTypeNameDict[WeaponModule];
            //get => weaponModule_s;
            set
            {
                weaponModule_s = value;
                //if (Util.UtilInfo.isRedPlayerShipsEmpty)
                    OnPropertyChanged();
            }
        }
    }
}
