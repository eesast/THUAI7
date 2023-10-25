using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Modules;

public class FlagConstructor1 : IConstructor
{
    private const int cost = GameData.FlagShipConstructor1Cost;
    public int Cost => cost;
    private const int constructSpeed = GameData.Constructor1Speed;
    public int ConstructSpeed => constructSpeed;
}