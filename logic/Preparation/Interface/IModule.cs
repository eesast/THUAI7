using Preparation.Utility;

namespace Preparation.Interface;

public interface IModule
{
    public int Cost { get; }
}
public interface IProducer : IModule
{
    public int ProduceSpeed { get; }
    public ProducerType ProducerModuleType { get; }
}
public interface IConstructor : IModule
{
    public int ConstructSpeed { get; }
    public ConstructorType ConstructorModuleType { get; }
}
public interface IArmor : IModule
{
    public int ArmorHP { get; }
    public ArmorType ArmorModuleType { get; }
}
public interface IShield : IModule
{
    public int ShieldHP { get; }
    public ShieldType ShieldModuleType { get; }
}
public interface IWeapon : IModule
{
    public BulletType BulletType { get; }
    public WeaponType WeaponModuleType { get; }
}
