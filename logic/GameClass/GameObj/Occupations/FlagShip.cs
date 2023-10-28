using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class FlagShip : IOccupation
{
    private const int moveSpeed = GameData.FlagShipMoveSpeed;
    public int MoveSpeed => moveSpeed;
    private const int maxHp = GameData.FlagShipMaxHP;
    public int MaxHp => maxHp;
    private const int viewRange = GameData.FlagShipViewRange;
    public int ViewRange => viewRange;
}