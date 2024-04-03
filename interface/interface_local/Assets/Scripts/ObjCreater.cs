using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ObjCreater : SingletonMono<ObjCreater>
{
    public GameObject[] placeList, shiplist, bulletList, constructionList;
    public Transform mapfa;
    public GameObject CreateObj(PlaceType placeType, Vector2 Pos, Quaternion? quaternion = null)
    {
        switch (placeType)
        {
            case PlaceType.SPACE:
                if (placeList[0])
                    return Instantiate(placeList[0], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.RUIN:
                if (placeList[1])
                    return Instantiate(placeList[1], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.SHADOW:
                if (placeList[2])
                    return Instantiate(placeList[2], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.ASTEROID:
                if (placeList[3])
                    return Instantiate(placeList[3], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.RESOURCE:
                if (placeList[4])
                    return Instantiate(placeList[4], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.CONSTRUCTION:
                if (placeList[5])
                    return Instantiate(placeList[5], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.WORMHOLE:
                if (placeList[6])
                    return Instantiate(placeList[6], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
            case PlaceType.HOME:
                if (placeList[7])
                    return Instantiate(placeList[7], Pos, quaternion ?? Quaternion.identity, mapfa);
                break;
        }
        return null;
    }
    public GameObject CreateObj(ShipType shipType, Vector2 Pos)
    {
        switch (shipType)
        {
            case ShipType.CIVILIAN_SHIP:
                if (shiplist[0])
                    return Instantiate(shiplist[0], Pos, Quaternion.identity);
                break;
            case ShipType.MILITARY_SHIP:
                if (shiplist[1])
                    return Instantiate(shiplist[1], Pos, Quaternion.identity);
                break;
            case ShipType.FLAG_SHIP:
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
                case BulletType.LASER:
                    if (bulletList[0])
                        return Instantiate(bulletList[0], Pos, quaternion);
                    break;
                case BulletType.PLASMA:
                    if (bulletList[1])
                        return Instantiate(bulletList[1], Pos, quaternion);
                    break;
                case BulletType.SHELL:
                    if (bulletList[2])
                        return Instantiate(bulletList[2], Pos, quaternion);
                    break;
                case BulletType.MISSILE:
                    if (bulletList[3])
                        return Instantiate(bulletList[3], Pos, quaternion);
                    break;
                case BulletType.ARC:
                    if (bulletList[4])
                        return Instantiate(bulletList[4], Pos, quaternion);
                    break;
            }
            return null;
        }
    }
    public GameObject CreateObj(ConstructionType constructionType, Vector2 Pos)
    {
        {
            switch (constructionType)
            {
                case ConstructionType.FACTORY:
                    if (bulletList[0])
                        return Instantiate(constructionList[0], Pos, Quaternion.identity);
                    break;
                case ConstructionType.COMMUNITY:
                    if (bulletList[1])
                        return Instantiate(constructionList[1], Pos, Quaternion.identity);
                    break;
                case ConstructionType.FORT:
                    if (bulletList[2])
                        return Instantiate(constructionList[2], Pos, Quaternion.identity);
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