using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class ObjectCreater : SingletonMono<ObjectCreater>
{
    public GameObject CreateObject(ShipType target, Vector2 position, Quaternion quaternion, Transform targetPa, int teamKey = 0)
    {
        GameObject obj = Instantiate(ParaDefine.GetInstance().PT(target), position, quaternion, targetPa);
        obj.GetComponent<ShipControl>().teamId = teamKey;
        return obj;
    }
    public GameObject CreateObject(ConstructionType target, Vector2 position, Quaternion quaternion, Transform targetPa, int teamKey = 0)
    {
        GameObject obj = Instantiate(ParaDefine.GetInstance().PT(target), position, quaternion, targetPa);
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
        return obj;
    }
    public GameObject CreateObject(PlaceType target, Vector2 position, Quaternion quaternion, Transform targetPa)
    {
        GameObject obj = Instantiate(ParaDefine.GetInstance().PT(target), position, quaternion, targetPa);
        obj.GetComponent<PlaceControl>().placeType = target;
        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
        return obj;
    }
    public GameObject CreateObject(BulletType target, Vector2 position, Quaternion quaternion, Transform targetPa)
    {
        GameObject obj = Instantiate(ParaDefine.GetInstance().PT(target), position, quaternion, targetPa);
        return obj;
    }
}
