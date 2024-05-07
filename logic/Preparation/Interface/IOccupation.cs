using Preparation.Utility;

namespace Preparation.Interface;

public interface IOccupation
{
    public int Cost { get; }
    public int MoveSpeed { get; }
    public int MaxHp { get; }
    public int BaseArmor { get; }
    public int BaseShield { get; }
    public int ViewRange { get; }
    public bool IsModuleValid(ModuleType moduleType);
}
