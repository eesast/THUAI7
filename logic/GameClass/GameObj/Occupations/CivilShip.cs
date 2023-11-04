using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class CivilShip : IOccupation
{
    public int MoveSpeed { get; } = GameData.CivilShipMoveSpeed;
    public int MaxHp { get; } = GameData.CivilShipMaxHP;
    public int ViewRange { get; } = GameData.CivilShipViewRange;
    public int Cost { get; } = GameData.CivilShipCost;
    public int BaseArmor { get; } = GameData.CivilShipBaseArmor;
    public int BaseShield { get; } = GameData.CivilShipBaseShield;
}