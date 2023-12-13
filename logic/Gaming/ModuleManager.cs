using GameClass.GameObj;
using Preparation.Utility;

namespace Gaming
{
    public partial class Game
    {
        private readonly ModuleManager moduleManager;
        private class ModuleManager
        {
            public bool PurchaseProducer(Ship ship, ProducerType producerType, int parameter)
            {
                return false;
            }
            public bool PurchaseConstructor(Ship ship, ConstructorType constructorType, int parameter)
            {
                return false;
            }
            public bool PurchaseArmor(Ship ship, ArmorType armorType, int parameter)
            {
                return false;
            }
            public bool PurchaseShield(Ship ship, ShieldType shieldType, int parameter)
            {
                return false;
            }
            public bool PurchaseWeapon(Ship ship, WeaponType weaponType, int parameter)
            {
                return false;
            }
        }
    }
}
