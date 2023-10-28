using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class WarShip : IOccupation
{
    private const int moveSpeed = GameData.WarShipMoveSpeed;
    public int MoveSpeed => moveSpeed;
    private const int maxHp = GameData.WarShipMaxHP;
    public int MaxHp => maxHp;
    private const int viewRange = GameData.WarShipViewRange;
    public int ViewRange => viewRange;
}