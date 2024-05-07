using GameClass.GameObj;
using Preparation.Utility;

namespace Gaming
{
    public partial class Game
    {
        private readonly ModuleManager moduleManager;
        private class ModuleManager
        {
            public bool InstallModule(Ship ship, ModuleType moduleType)
            {
                return ship.InstallModule(moduleType);
            }
        }
    }
}
