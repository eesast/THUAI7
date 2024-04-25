using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagConstructor1 : IConstructor
{
    public int Cost => GameData.FlagShipConstructor1Cost;
    public int ConstructSpeed => GameData.Constructor1Speed;
    public ConstructorType ConstructorModuleType => ConstructorType.Constructor1;
}