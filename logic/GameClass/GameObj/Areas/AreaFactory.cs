using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public static class AreaFactory
{
    public static Immovable GetArea(XY pos, PlaceType placeType) => placeType switch
    {
        PlaceType.Home => new Home(pos),
        PlaceType.Ruin => new Ruin(pos),
        PlaceType.Shadow => new Shadow(pos),
        PlaceType.Asteroid => new Asteroid(pos),
        PlaceType.Resource => new Resource(pos),
        PlaceType.Construction => new Construction(pos),
        PlaceType.Wormhole => new Wormhole(pos),
        _ => new NullArea(pos)
    };
    public static OutOfBoundBlock GetOutOfBoundBlock(XY pos) => new(pos);
}
