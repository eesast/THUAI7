using System.Collections.Generic;
using UnityEngine;

public class PlaceManager : SingletonMono<PlaceManager>
{
    public List<Vector2> resource, emptyConstruction;
    public List<ConstructionControl> community, factory, fort;
}
