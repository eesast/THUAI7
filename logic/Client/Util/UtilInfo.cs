using Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Util
{
    public static class UtilInfo
    {
        public static Dictionary<SweeperType, string> SweeperTypeNameDict = new Dictionary<SweeperType, string>{
            { SweeperType.NullSweeperType, "NullSweeperType" },
            { SweeperType.CivilianSweeper, "CivilianSweeper" },
            { SweeperType.MilitarySweeper, "MilitarySweeper" },
            { SweeperType.FlagSweeper, "FlagSweeper" }
        };

        public static Dictionary<SweeperState, string> SweeperStateNameDict = new Dictionary<SweeperState, string>
        {
            { SweeperState.Idle, "Idle" },
            { SweeperState.Producing, "Producing" },
            { SweeperState.Constructing, "Constructing" },
            { SweeperState.Recovering, "Recovering" },
            { SweeperState.Recycling, "Recycling" },
            { SweeperState.Attacking, "Attacking" },
            { SweeperState.Swinging, "Swinging" },
            { SweeperState.Moving, "Moving" }
        };

        public static Dictionary<ArmorType, string> SweeperArmorTypeNameDict = new Dictionary<ArmorType, string>
        {
            { ArmorType.NullArmorType, "NullArmorType" },
            { ArmorType.Armor1, "Armor1" },
            { ArmorType.Armor2, "Armor2" },
            { ArmorType.Armor3, "Armor3" }
        };

        public static Dictionary<ShieldType, string> SweeperShieldTypeNameDict = new Dictionary<ShieldType, string>
        {
            { ShieldType.NullShieldType, "NullShieldType" },
            { ShieldType.Shield1, "Shield1" },
            { ShieldType.Shield2, "Shield2" },
            { ShieldType.Shield3, "Shield3" }
        };

        public static Dictionary<ConstructorType, string> SweeperConstructorNameDict = new Dictionary<ConstructorType, string>
        {
            { ConstructorType.NullConstructorType, "NullConstructorType" },
            { ConstructorType.Constructor1, "Constructor1" },
            { ConstructorType.Constructor2, "Constructor2" },
            { ConstructorType.Constructor3, "Constructor3" }
        };

        public static Dictionary<ProducerType, string> SweeperProducerTypeNameDict = new Dictionary<ProducerType, string>
        {
            { ProducerType.NullProducerType, "NullProducerType" },
            { ProducerType.Producer1, "Producer1" },
            { ProducerType.Producer2, "Producer2" },
            { ProducerType.Producer3, "Producer3" }
        };

        public static Dictionary<WeaponType, string> SweeperWeaponTypeNameDict = new Dictionary<WeaponType, string>
        {
            { WeaponType.NullWeaponType, "NullWeaponType" },
            { WeaponType.Lasergun, "Lasergun" },
            { WeaponType.Arcgun, "Arcgun" },
            { WeaponType.Plasmagun, "Plasmagun" },
            { WeaponType.Missilegun, "Missilegun" },
            { WeaponType.Shellgun, "Shellgun" }
        };
    }


}
