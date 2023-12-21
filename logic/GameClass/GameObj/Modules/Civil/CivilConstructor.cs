using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class CivilConstructor1 : IConstructor
{
    private const int cost = GameData.CivilShipConstructor1Cost;
    public int Cost => cost;
    private const int constructSpeed = GameData.Constructor1Speed;
    public int ConstructSpeed => constructSpeed;
}
public class CivilConstructor2 : IConstructor
{
    private const int cost = GameData.CivilShipConstructor2Cost;
    public int Cost => cost;
    private const int constructSpeed = GameData.Constructor2Speed;
    public int ConstructSpeed => constructSpeed;
}
public class CivilConstructor3 : IConstructor
{
    private const int cost = GameData.CivilShipConstructor3Cost;
    public int Cost => cost;
    private const int constructSpeed = GameData.Constructor2Speed;
    public int ConstructSpeed => constructSpeed;
}