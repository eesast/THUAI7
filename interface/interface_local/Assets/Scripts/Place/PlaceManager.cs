using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public class PlaceManager : SingletonMono<PlaceManager>
{
    public List<Vector2> resource, emptyConstruction;
    public List<ConstructionControl> community, factory, fort;
}
