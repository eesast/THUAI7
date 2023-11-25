using Preparation.Utility;

namespace Preparation.Interface;

public interface IOccupation
{
    public int MoveSpeed { get; }
    public int MaxHp { get; }
    public int ViewRange { get; }
}
