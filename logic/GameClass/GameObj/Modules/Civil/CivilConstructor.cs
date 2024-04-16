using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilConstructor1 : IConstructor
{
    public int Cost => GameData.CivilShipConstructor1Cost;
    public int ConstructSpeed => GameData.Constructor1Speed;
    public ConstructorType ConstructorModuleType => ConstructorType.Constructor1;
}

public class CivilConstructor2 : IConstructor
{
    public int Cost => GameData.CivilShipConstructor2Cost;
    public int ConstructSpeed => GameData.Constructor2Speed;
    public ConstructorType ConstructorModuleType => ConstructorType.Constructor2;
}

public class CivilConstructor3 : IConstructor
{
    public int Cost => GameData.CivilShipConstructor3Cost;
    public int ConstructSpeed => GameData.Constructor3Speed;
    public ConstructorType ConstructorModuleType => ConstructorType.Constructor3;
}