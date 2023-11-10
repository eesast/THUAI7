using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class FlagShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.FlagShipMoveSpeed;
    public int MaxHp { get; } = GameData.FlagShipMaxHP;
    public int ViewRange { get; } = GameData.FlagShipViewRange;
    public int Cost { get; } = GameData.FlagShipCost;
    public int BaseArmor { get; } = GameData.FlagShipBaseArmor;
    public int BaseShield { get; } = GameData.FlagShipBaseShield;
}