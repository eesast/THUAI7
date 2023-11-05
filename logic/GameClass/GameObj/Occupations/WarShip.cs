using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class WarShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.WarShipMoveSpeed;
    public int MaxHp { get; } = GameData.WarShipMaxHP;
    public int ViewRange { get; } = GameData.WarShipViewRange;
    public int Cost { get; } = GameData.WarShipCost;
    public int BaseArmor { get; } = GameData.WarShipBaseArmor;
    public int BaseShield { get; } = GameData.WarShipBaseShield;
}