using Preparation.Utility;

namespace GameClass.GameObj.Areas;

public static class AreaFactory
{
    public static Immovable GetArea(XY pos, PlaceType placeType) => placeType switch
    {
        PlaceType.Home => new Home(pos, GameObjType.Home),
        PlaceType.Ruin => new Ruin(pos, GameObjType.Ruin),
        PlaceType.Shadow => new Shadow(pos, GameObjType.Shadow),
        PlaceType.Asteroid => new Asteroid(pos, GameObjType.Asteroid),
        PlaceType.Resource => new Resource(pos, GameObjType.Resource),
        PlaceType.Construction => new Construction(pos, GameObjType.Construction),
        PlaceType.Wormhole => new Wormhole(pos, GameObjType.Wormhole),
        _ => throw new System.NotImplementedException()
    };
    public static OutOfBoundBlock GetOutOfBoundBlock(XY pos) => new(pos);
}
