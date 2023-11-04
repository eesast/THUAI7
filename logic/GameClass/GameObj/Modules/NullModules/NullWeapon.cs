using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class NullWeapon : IWeapon
{
    public static NullWeapon Instance { get; } = new();
    public BulletType BulletType => BulletType.Null;
    public int Cost => 0;
    private NullWeapon() { }
}
