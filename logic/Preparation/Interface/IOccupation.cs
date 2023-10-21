using Preparation.Utility;
using System.Collections.Generic;

namespace Preparation.Interface
{
    public interface IOccupation
    {
        public int MoveSpeed { get; }
        public int MaxHp { get; }
        public int ViewRange { get; }
    }
    public class CivilShip : IOccupation
    {
        private const int moveSpeed = GameData.CivilShipMoveSpeed;
        public int MoveSpeed => moveSpeed;
        private const int maxHp = GameData.CivilShipMaxHP;
        public int MaxHp => maxHp;
        private const int viewRange = GameData.CivilShipViewRange;
        public int ViewRange => viewRange;
    }
    public class WarShip : IOccupation
    {
        private const int moveSpeed = GameData.WarShipMoveSpeed;
        public int MoveSpeed => moveSpeed;
        private const int maxHp = GameData.WarShipMaxHP;
        public int MaxHp => maxHp;
        private const int viewRange = GameData.WarShipViewRange;
        public int ViewRange => viewRange;
    }
    public class FlagShip : IOccupation
    {
        private const int moveSpeed = GameData.FlagShipMoveSpeed;
        public int MoveSpeed => moveSpeed;
        private const int maxHp = GameData.FlagShipMaxHP;
        public int MaxHp => maxHp;
        private const int viewRange = GameData.FlagShipViewRange;
        public int ViewRange => viewRange;
    }
    public static class OccupationFactory
    {
        public static IOccupation FindIOccupation(ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.CivilShip:
                    return new CivilShip();
                case ShipType.WarShip:
                    return new WarShip();
                case ShipType.FlagShip:
                    return new FlagShip();
                default:
                    return new CivilShip();
            }
        }
    }
}
