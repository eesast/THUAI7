using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class NullWeapon : IWeapon
{
    public BulletType BulletType => BulletType.Null;
    public int Cost => 0;
}
