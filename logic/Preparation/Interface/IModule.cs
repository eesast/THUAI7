using Preparation.Utility;

namespace Preparation.Interface;

public interface IModule
{
    public int Cost { get; }
}
public interface IProducer : IModule
{
    public int ProduceSpeed { get; }
}
public interface IConstructor : IModule
{
    public int ConstructSpeed { get; }
}
public interface IArmor : IModule
{
    public int ArmorHP { get; }
}
public interface IShield : IModule
{
    public int ShieldHP { get; }
}
public interface IWeapon : IModule
{
    public BulletType BulletType { get; }
}
