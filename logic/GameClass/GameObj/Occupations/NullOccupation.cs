using Preparation.Interface;

namespace GameClass.GameObj.Occupations;

public class NullOccupation : IOccupation
{
    public int MoveSpeed => 0;
    public int MaxHp => 0;
    public int ViewRange => 0;
}
