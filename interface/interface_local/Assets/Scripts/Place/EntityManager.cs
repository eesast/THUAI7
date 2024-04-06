using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMono<EntityManager>
{
    public List<Vector2> resource, emptyConstruction;
    public List<ConstructionControl> community, factory, fort;
    public List<ShipControl> ship;
}
