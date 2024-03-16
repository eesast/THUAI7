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
        public static Dictionary<ShipType, string> ShipTypeNameDict = new Dictionary<ShipType, string>{
            { ShipType.NullShipType, "NullShipType" },
            { ShipType.CivilianShip, "CivilianShip" },
            { ShipType.MilitaryShip, "MilitaryShip" },
            { ShipType.FlagShip, "FlagShip" }
        };

        public static Dictionary<ShipState, string> ShipStateNameDict = new Dictionary<ShipState, string>
        {
            { ShipState.Idle, "Idle" },
            { ShipState.Producing, "Producing" },
            { ShipState.Constructing, "Constructing" },
            { ShipState.Recovering, "Recovering" },
            { ShipState.Recycling, "Recycling" },
            { ShipState.Attacking, "Attacking" },
            { ShipState.Swinging, "Swinging" },
            { ShipState.Moving, "Moving" }
        };

        public static Dictionary<ArmorType, string> ShipArmorTypeNameDict = new Dictionary<ArmorType, string>
        {
            { ArmorType.NullArmorType, "NullArmorType" },
            { ArmorType.Armor1, "Armor1" },
            { ArmorType.Armor2, "Armor2" },
            { ArmorType.Armor3, "Armor3" }
        };

        public static Dictionary<ShieldType, string> ShipShieldTypeNameDict = new Dictionary<ShieldType, string>
        {
            { ShieldType.NullShieldType, "NullShieldType" },
            { ShieldType.Shield1, "Shield1" },
            { ShieldType.Shield2, "Shield2" },
            { ShieldType.Shield3, "Shield3" }
        };

        public static Dictionary<ConstructorType, string> ShipConstructorNameDict = new Dictionary<ConstructorType, string>
        {
            { ConstructorType.NullConstructorType, "NullConstructorType" },
            { ConstructorType.Constructor1, "Constructor1" },
            { ConstructorType.Constructor2, "Constructor2" },
            { ConstructorType.Constructor3, "Constructor3" }
        };

        public static Dictionary<ProducerType, string> ShipProducerTypeNameDict = new Dictionary<ProducerType, string>
        {
            { ProducerType.NullProducerType, "NullProducerType" },
            { ProducerType.Producer1, "Producer1" },
            { ProducerType.Producer2, "Producer2" },
            { ProducerType.Producer3, "Producer3" }
        };

        public static Dictionary<WeaponType, string> ShipWeaponTypeNameDict = new Dictionary<WeaponType, string>
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
