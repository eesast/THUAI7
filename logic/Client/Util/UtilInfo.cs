using Protobuf;

namespace Client.Util
{
    public static class UtilInfo
    {
        public static readonly Dictionary<ShipType, string> ShipTypeNameDict = new()
        {
            { ShipType.NullShipType, "Null" },
            { ShipType.CivilianShip, "Civilian" },
            { ShipType.MilitaryShip, "Military" },
            { ShipType.FlagShip, "FlagShip" }
        };

        public static readonly Dictionary<ShipState, string> ShipStateNameDict = new()
        {
            { ShipState.Idle, "Idle" },
            { ShipState.Producing, "Producing" },
            { ShipState.Constructing, "Constructing" },
            { ShipState.Recovering, "Recovering" },
            { ShipState.Recycling, "Recycling" },
            { ShipState.Attacking, "Attacking" },
            { ShipState.Swinging, "Swinging" },
            { ShipState.Stunned, "Stunned" },
            { ShipState.Moving, "Moving" }

        };

        public static readonly Dictionary<ArmorType, string> ShipArmorTypeNameDict = new()
        {
            //{ ArmorType.NullArmorType, "Null" },
            //{ ArmorType.Armor1, "Armor1" },
            //{ ArmorType.Armor2, "Armor2" },
            //{ ArmorType.Armor3, "Armor3" }
            { ArmorType.NullArmorType, "Null" },
            { ArmorType.Armor1, "🪖🔸" },
            { ArmorType.Armor2, "🪖⭐" },
            { ArmorType.Armor3, "🪖🌟" }
        };

        public static readonly Dictionary<ShieldType, string> ShipShieldTypeNameDict = new()
        {
            { ShieldType.NullShieldType, "Null" },
            { ShieldType.Shield1, "🛡️🔸" },
            { ShieldType.Shield2, "🛡️⭐" },
            { ShieldType.Shield3, "🛡️🌟" }
        };

        public static readonly Dictionary<ConstructorType, string> ShipConstructorNameDict = new()
        {
            { ConstructorType.NullConstructorType, "Null" },
            { ConstructorType.Constructor1, "🔨🔸" },
            { ConstructorType.Constructor2, "🔨⭐" },
            { ConstructorType.Constructor3, "🔨🌟" }
        };

        public static readonly Dictionary<ProducerType, string> ShipProducerTypeNameDict = new()
        {
            { ProducerType.NullProducerType, "Null" },
            { ProducerType.Producer1, "⛏🔸" },
            { ProducerType.Producer2, "⛏⭐" },
            { ProducerType.Producer3, "⛏🌟" }
        };

        public static readonly Dictionary<WeaponType, string> ShipWeaponTypeNameDict = new()
        {
            { WeaponType.NullWeaponType, "Null" },
            { WeaponType.Lasergun, "Lasergun" },
            { WeaponType.Arcgun, "Arcgun" },
            { WeaponType.Plasmagun, "Plasmagun" },
            { WeaponType.Missilegun, "Missilegun" },
            { WeaponType.Shellgun, "Shellgun" }
        };

        public static bool isRedPlayerShipsEmpty = true;
        public static bool isBluePlayerShipsEmpty = false;

        public const int unitWidth = 10;
        public const int unitHeight = 10;
    }


}
