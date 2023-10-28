using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class CivilShip : IOccupation
{
    private const int moveSpeed = GameData.CivilShipMoveSpeed;
    public int MoveSpeed => moveSpeed;
    private const int maxHp = GameData.CivilShipMaxHP;
    public int MaxHp => maxHp;
    private const int viewRange = GameData.CivilShipViewRange;
    public int ViewRange => viewRange;
}