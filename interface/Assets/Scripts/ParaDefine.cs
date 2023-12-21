using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaDefine : SingletonDontDestory<ParaDefine>
{
    public enum PlaceType {
        Space,
        Ruin,
        Home,
        Shadow,
        Asteroid,
        Resource,
        Community,
        Factory,
        Fortress,
    };
    public GameObject spaceG, ruinG, homeG, shadowG, asteroidG, resourceG, communityG, factoryG, fortressG;
    public GameObject PT(PlaceType _placeType) {
        switch (_placeType) {
            case PlaceType.Space:
                return spaceG;
            case PlaceType.Ruin:
                return ruinG;
            case PlaceType.Home:
                return homeG;
            case PlaceType.Shadow:
                return shadowG;
            case PlaceType.Asteroid:
                return asteroidG;
            case PlaceType.Community:
                return communityG;
            case PlaceType.Factory:
                return factoryG;
            case PlaceType.Fortress:
                return fortressG;
            case PlaceType.Resource:
                return resourceG;
            default: return null;
        }
    }

}
