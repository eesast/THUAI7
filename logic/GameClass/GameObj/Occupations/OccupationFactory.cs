using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public static class OccupationFactory
{
    public static IOccupation FindIOccupation(ShipType shipType) => shipType switch
    {
        ShipType.CivilShip => new CivilShip(),
        ShipType.WarShip => new WarShip(),
        ShipType.FlagShip => new FlagShip(),
        _ => throw new System.NotImplementedException(),
    };
}