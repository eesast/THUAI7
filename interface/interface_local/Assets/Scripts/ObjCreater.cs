using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjCreater : SingletonMono<ObjCreater>
{
    public GameObject[] placeList;
    public void CreateObj(PlaceType placeType, Vector2 Pos)
    {
        switch (placeType)
        {
            case PlaceType.SPACE:
                if (placeList[0])
                    Instantiate(placeList[0], Pos, Quaternion.identity);
                break;
            case PlaceType.RUIN:
                if (placeList[1])
                    Instantiate(placeList[1], Pos, Quaternion.identity);
                break;
            case PlaceType.SHADOW:
                if (placeList[2])
                    Instantiate(placeList[2], Pos, Quaternion.identity);
                break;
            case PlaceType.ASTEROID:
                if (placeList[3])
                    Instantiate(placeList[3], Pos, Quaternion.identity);
                break;
            case PlaceType.RESOURCE:
                if (placeList[4])
                    Instantiate(placeList[4], Pos, Quaternion.identity);
                break;
            case PlaceType.CONSTRUCTION:
                if (placeList[5])
                    Instantiate(placeList[5], Pos, Quaternion.identity);
                break;
            case PlaceType.WORMHOLE:
                if (placeList[6])
                    Instantiate(placeList[6], Pos, Quaternion.identity);
                break;
            case PlaceType.HOME:
                if (placeList[7])
                    Instantiate(placeList[7], Pos, Quaternion.identity);
                break;
            default: break;
        }
    }
}
