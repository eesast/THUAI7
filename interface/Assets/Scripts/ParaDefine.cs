using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
public class ParaDefine : SingletonDontDestory<ParaDefine>
{
    public GameObject spaceG, ruinG, homeplaceG, shadowG, asteroidG, resourceG, communityG, factoryG, fortG;
    public GameObject civilianshipG, militaryshipG, flagshipG;
    public GameObject laserG, plasmaG, shellG, missileG, arcG;
    public GameObject PT(PlaceType _placeType) {
        switch (_placeType) {
            case PlaceType.Space:
                return spaceG;
            case PlaceType.Ruin:
                return ruinG;
            case PlaceType.HomePlace:
                return homeplaceG;
            case PlaceType.Shadow:
                return shadowG;
            case PlaceType.Asteroid:
                return asteroidG;
            case PlaceType.Resource:
                return resourceG;
            default: return null;
        }
    }
    public GameObject PT(ConstructionType _constructionType) {
        switch (_constructionType) {
            case ConstructionType.Community:
                return communityG;
            case ConstructionType.Factory:
                return factoryG;
            case ConstructionType.Fort:
                return fortG;
            default: return null;
        }
    }
    public GameObject PT(ShipType _shipType) {
        switch (_shipType) {
            case ShipType.CivilianShip:
                return civilianshipG;
            case ShipType.MilitaryShip:
                return militaryshipG;
            case ShipType.FlagShip:
                return flagshipG;
            default: return null;
        }
    }
    public GameObject PT(BulletType _bulletType) {
        switch (_bulletType) {
            case BulletType.Laser:
                return laserG;
            case BulletType.Plasma:
                return plasmaG;
            case BulletType.Shell:
                return shellG;
            case BulletType.Missile:
                return missileG;
            case BulletType.Arc:
                return arcG;
            default: return null;
        }
    }
}
