using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

[Serializable]
public struct GameObjectList
{
    public List<GameObject> p;
}
public class ObjCreater : SingletonMono<ObjCreater>
{
    public List<GameObjectList> placeList;
    public GameObject[] shiplist, bulletList, constructionList;
    public Transform mapfa;
    public GameObject CreateObj(PlaceType placeType, Vector2 Pos, Quaternion? quaternion = null)
    {
        switch (placeType)
        {
            case PlaceType.Space:
                if (placeList[0].p.Count > 0)
                    return Instantiate(placeList[0].p[Tool.GetInstance().GetRandom(0, placeList[0].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Ruin:
                if (placeList[1].p.Count > 0)
                    return Instantiate(placeList[1].p[Tool.GetInstance().GetRandom(0, placeList[1].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Shadow:
                if (placeList[2].p.Count > 0)
                    return Instantiate(placeList[2].p[Tool.GetInstance().GetRandom(0, placeList[2].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Asteroid:
                if (placeList[3].p.Count > 0)
                    return Instantiate(placeList[3].p[Tool.GetInstance().GetRandom(0, placeList[3].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Resource:
                if (placeList[4].p.Count > 0)
                    return Instantiate(placeList[4].p[Tool.GetInstance().GetRandom(0, placeList[4].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Construction:
                if (placeList[5].p.Count > 0)
                    return Instantiate(placeList[5].p[Tool.GetInstance().GetRandom(0, placeList[5].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Wormhole:
                if (placeList[6].p.Count > 0)
                    return Instantiate(placeList[6].p[Tool.GetInstance().GetRandom(0, placeList[6].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.Home:
                if (placeList[7].p.Count > 0)
                    return Instantiate(placeList[7].p[Tool.GetInstance().GetRandom(0, placeList[7].p.Count)], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
        }
        return null;
    }
    public GameObject CreateObj(ShipType shipType, Vector2 Pos)
    {
        switch (shipType)
        {
            case ShipType.CivilianShip:
                if (shiplist[0])
                    return Instantiate(shiplist[0], Pos, Quaternion.identity);
                break;
            case ShipType.MilitaryShip:
                if (shiplist[1])
                    return Instantiate(shiplist[1], Pos, Quaternion.identity);
                break;
            case ShipType.FlagShip:
                if (shiplist[2])
                    return Instantiate(shiplist[2], Pos, Quaternion.identity);
                break;
        }
        return null;
    }
    public GameObject CreateObj(BulletType bulletType, Vector2 Pos, Quaternion quaternion)
    {
        {
            switch (bulletType)
            {
                case BulletType.Laser:
                    if (bulletList[0])
                        return Instantiate(bulletList[0], Pos, quaternion);
                    break;
                case BulletType.Plasma:
                    if (bulletList[1])
                        return Instantiate(bulletList[1], Pos, quaternion);
                    break;
                case BulletType.Shell:
                    if (bulletList[2])
                        return Instantiate(bulletList[2], Pos, quaternion);
                    break;
                case BulletType.Missile:
                    if (bulletList[3])
                        return Instantiate(bulletList[3], Pos, quaternion);
                    break;
                case BulletType.Arc:
                    if (bulletList[4])
                        return Instantiate(bulletList[4], Pos, quaternion);
                    break;
            }
            return null;
        }
    }
    public GameObject CreateObj(ConstructionType constructionType, Vector2 Pos, bool flip = false)
    {
        {
            switch (constructionType)
            {
                case ConstructionType.Factory:
                    if (bulletList[0])
                        return Instantiate(constructionList[0], Pos, Quaternion.identity);
                    break;
                case ConstructionType.Community:
                    if (bulletList[1])
                        return Instantiate(constructionList[1], Pos, Quaternion.identity);
                    break;
                case ConstructionType.Fort:
                    if (bulletList[2])
                        if (!flip)
                            return Instantiate(constructionList[2], Pos, Quaternion.Euler(0, 0, 0));
                        else
                            return Instantiate(constructionList[2], Pos, Quaternion.Euler(0, 0, 180));
                    break;
                default:
                    break;
            }
            return null;
        }
    }
    void Start()
    {
        mapfa = GameObject.Find("Map").transform;
    }
}