using Preparation.Utility.Value;
using System;


namespace Preparation.Utility
{
    public static class GameData
    {
        public const int NumOfStepPerSecond = 100;          // 每秒行走的步数
        public const int FrameDuration = 50;                // 每帧时长
        public const int CheckInterval = 10;                // 检查间隔
        public const uint GameDurationInSecond = 60 * 10;   // 游戏时长
        public const int LimitOfStopAndMove = 15;           // 停止和移动的最大间隔

        public const int TolerancesLength = 3;
        public const int AdjustLength = 3;

        public const int MaxShipNum = 5;                // 最大舰船数量
        public const int MaxCivilShipNum = 2;           // 最大民用舰船数量
        public const int MaxWarShipNum = 2;             // 最大军用舰船数量
        public const int MaxFlagShipNum = 1;            // 最大旗舰数量

        public const int NumOfPosGridPerCell = 1000;    // 每格的【坐标单位】数
        public const int MapLength = 50000;             // 地图长度
        public const int MapRows = 50;                  // 行数
        public const int MapCols = 50;                  // 列数

        public const int InitialMoney = 5000;             // 初始金钱

        public static bool IsGameObjMap(GameObjType gameObjType) => (uint)gameObjType > 3;
        public static bool NeedCopy(GameObjType gameObjType)
        {
            return gameObjType != GameObjType.Null &&
                   gameObjType != GameObjType.Ruin &&
                   gameObjType != GameObjType.Shadow &&
                   gameObjType != GameObjType.Asteroid &&
                   gameObjType != GameObjType.OutOfBoundBlock;
        }
        public static XY GetCellCenterPos(int x, int y)  // 求格子的中心坐标
            => new(x * NumOfPosGridPerCell + NumOfPosGridPerCell / 2,
                   y * NumOfPosGridPerCell + NumOfPosGridPerCell / 2);

        public static int PosGridToCellX(XY pos)  // 求坐标所在的格子的x坐标
            => pos.x / NumOfPosGridPerCell;
        public static int PosGridToCellY(XY pos)  // 求坐标所在的格子的y坐标
            => pos.y / NumOfPosGridPerCell;
        public static CellXY PosGridToCellXY(XY pos)  // 求坐标所在的格子的xy坐标
            => new(PosGridToCellX(pos), PosGridToCellY(pos));

        public static bool IsInTheSameCell(XY pos1, XY pos2) => PosGridToCellXY(pos1) == PosGridToCellXY(pos2);
        public static bool PartInTheSameCell(XY pos1, XY pos2)
        {
            return Math.Abs((pos1 - pos2).x) < ShipRadius + (NumOfPosGridPerCell / 2)
                && Math.Abs((pos1 - pos2).y) < ShipRadius + (NumOfPosGridPerCell / 2);
        }
        public static bool ApproachToInteract(XY pos1, XY pos2)
        {
            return Math.Abs(PosGridToCellX(pos1) - PosGridToCellX(pos2)) <= 1
                && Math.Abs(PosGridToCellY(pos1) - PosGridToCellY(pos2)) <= 1;
        }
        public static bool ApproachToInteractInACross(XY pos1, XY pos2)
        {
            if (pos1 == pos2) return false;
            return (Math.Abs(PosGridToCellX(pos1) - PosGridToCellX(pos2))
                  + Math.Abs(PosGridToCellY(pos1) - PosGridToCellY(pos2))) <= 1;
        }
        public static bool IsInTheRange(XY pos1, XY pos2, int range)
        {
            return (pos1 - pos2).Length() <= range;
        }

        public const int ShipRadius = 400;
        public static readonly XY PosNotInGame = new(1, 1);

        public const int BulletRadius = 200;            // 子弹半径
        public const int LaserRange = 4000;             // 激光射程
        public const int LaserDamage = 800;             // 激光伤害
        public const double LaserArmorModifier = 1.5;   // 激光装甲修正
        public const double LaserShieldModifier = 0.6;  // 激光护盾修正
        public const int LaserSpeed = 20000;            // 激光速度
        public const int LaserCastTime = 500;           // 激光前摇时间
        public const int LaserSwingTime = 1000;         // 激光后摇时间
        public const int PlasmaRange = 4000;            // 等离子射程
        public const int PlasmaDamage = 1000;           // 等离子伤害
        public const double PlasmaArmorModifier = 2.0;  // 等离子装甲修正
        public const double PlasmaShieldModifier = 0.4; // 等离子护盾修正
        public const int PlasmaSpeed = 10000;           // 等离子速度
        public const int PlasmaCastTime = 800;          // 等离子前摇时间
        public const int PlasmaSwingTime = 1600;         // 等离子后摇时间
        public const int ShellRange = 4000;             // 炮弹射程
        public const int ShellDamage = 1200;            // 炮弹伤害
        public const double ShellArmorModifier = 0.4;   // 炮弹装甲修正
        public const double ShellShieldModifier = 1.5;  // 炮弹护盾修正
        public const int ShellSpeed = 8000;             // 炮弹速度
        public const int ShellCastTime = 500;           // 炮弹前摇时间
        public const int ShellSwingTime = 1000;         // 炮弹后摇时间
        public const int MissileRange = 6000;           // 导弹射程
        public const int MissileBombRange = 1600;       // 导弹爆炸范围
        public const int MissileDamage = 1100;          // 导弹伤害
        public const double MissileArmorModifier = 1.0; // 导弹装甲修正
        public const int MissileSpeed = 6000;           // 导弹速度
        public const int MissileCastTime = 1200;        // 导弹前摇时间
        public const int MissileSwingTime = 1800;       // 导弹后摇时间
        public const int ArcRange = 6000;               // 电弧射程
        public const int ArcDamageMin = 800;            // 电弧伤害
        public const int ArcDamageMax = 1600;           // 电弧伤害
        public const double ArcArmorModifier = 2.0;     // 电弧装甲修正
        public const double ArcShieldModifier = 2.0;    // 电弧护盾修正
        public const int ArcSpeed = 8000;               // 电弧速度
        public const int ArcCastTime = 1200;            // 电弧前摇时间
        public const int ArcSwingTime = 1800;           // 电弧后摇时间

        public const int CivilShipCost = 4000;
        public const int CivilShipMaxHP = 3000;
        public const int CivilShipMoveSpeed = 3000;
        public const int CivilShipViewRange = 8000;
        public const int CivilShipBaseArmor = 0;
        public const int CivilShipBaseShield = 0;
        public const int CivilShipProducer1Cost = 0;
        public const int CivilShipProducer2Cost = 4000;
        public const int CivilShipProducer3Cost = 8000;
        public const int CivilShipConstructor1Cost = 0;
        public const int CivilShipConstructor2Cost = 4000;
        public const int CivilShipConstructor3Cost = 8000;
        public const int CivilShipArmor1Cost = 6000;
        public const int CivilShipShield1Cost = 6000;
        public const int CivilShipLaserGunCost = 10000;

        public const int WarShipCost = 12000;
        public const int WarShipMaxHP = 4000;
        public const int WarShipMoveSpeed = 2800;
        public const int WarShipViewRange = 8000;
        public const int WarShipBaseArmor = 400;
        public const int WarShipBaseShield = 400;
        public const int WarShipArmor1Cost = 6000;
        public const int WarShipArmor2Cost = 12000;
        public const int WarShipArmor3Cost = 18000;
        public const int WarShipShield1Cost = 6000;
        public const int WarShipShield2Cost = 12000;
        public const int WarShipShield3Cost = 18000;
        public const int WarShipLaserGunCost = 0;
        public const int WarShipPlasmaGunCost = 12000;
        public const int WarShipShellGunCost = 13000;
        public const int WarShipMissileGunCost = 18000;
        public const int WarShipArcGunCost = 24000;

        public const int FlagShipCost = 40000;
        public const int FlagShipMaxHP = 12000;
        public const int FlagShipMoveSpeed = 2700;
        public const int FlagShipViewRange = 8000;
        public const int FlagShipBaseArmor = 800;
        public const int FlagShipBaseShield = 800;
        public const int FlagShipProducer1Cost = 400;
        public const int FlagShipConstructor1Cost = 400;
        public const int FlagShipArmor1Cost = 6000;
        public const int FlagShipArmor2Cost = 12000;
        public const int FlagShipArmor3Cost = 18000;
        public const int FlagShipShield1Cost = 6000;
        public const int FlagShipShield2Cost = 12000;
        public const int FlagShipShield3Cost = 18000;
        public const int FlagShipLaserGunCost = 0;
        public const int FlagShipPlasmaGunCost = 12000;
        public const int FlagShipShellGunCost = 13000;
        public const int FlagShipMissileGunCost = 18000;
        public const int FlagShipArcGunCost = 24000;

        public const int ScoreHomePerSecond = 100;
        public const int ScoreFactoryPerSecond = 200;
        public const int ScoreProducer1PerSecond = 100;
        public const int ScoreProducer2PerSecond = 200;
        public const int ScoreProducer3PerSecond = 300;
        public const int ScoreConstructionDamaged = 200;
        public static int ScoreShipKilled(int totalScore) => totalScore / 5;
        public static int ScoreShipRecovered(int totalRecovery) => totalRecovery * 6 / 5;
        public static int ScoreShipRecycled(int remainingHP) => remainingHP / 2;

        public const int Constructor1Speed = 300;
        public const int Constructor2Speed = 400;
        public const int Constructor3Speed = 500;
        public const int Armor1 = 2000;
        public const int Armor2 = 3000;
        public const int Armor3 = 4000;
        public const int Shield1 = 2000;
        public const int Shield2 = 3000;
        public const int Shield3 = 4000;

        public const int ResourceHP = 32000;
        public const int FactoryHP = 12000;
        public const int CommunityHP = 10000;
        public const int FortHP = 16000;
        public const int WormholeHP = 24000;
        public const int HomeHP = 48000;

        public const int FortRange = 6000;
        public const int FortDamage = 300;
    }
}
