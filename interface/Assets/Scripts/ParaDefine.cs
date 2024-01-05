using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
public class ParaDefine : SingletonDontDestory<ParaDefine>{
    public MessageOfMap map;
    public GameObject spaceG;
    public GameObject[] ruinG;
    public GameObject[] homeG;
    public GameObject shadowG;
    public GameObject[] asteroidG;
    public GameObject[] resourceG;
    public GameObject wormholeG;
    public GameObject[] communityG;
    public GameObject[] factoryG;
    public GameObject[] fortG;
    public GameObject[] civilianshipG;
    public GameObject[] militaryshipG;
    public GameObject[] flagshipG;
    public GameObject[] laserG;
    public GameObject[] plasmaG;
    public GameObject[] shellG;
    public GameObject[] missileG;
    public GameObject[] arcG;
    public GameObject PT(PlaceType _placeType, int teamKey = 0) {
        switch (_placeType) {
            case PlaceType.Home:
                return homeG[teamKey];
            case PlaceType.Space:
                return spaceG;
            case PlaceType.Ruin:
                return ruinG[GetRand()%2];
            case PlaceType.Shadow:
                return shadowG;
            case PlaceType.Asteroid:
                return asteroidG[GetRand()%3];
            case PlaceType.Resource:
                return resourceG[GetRand()%3];
            case PlaceType.Wormhole:
                return wormholeG;
            default: return null;
        }
    }
    public GameObject PT(ConstructionType _constructionType, int teamKey = 0) {
        switch (_constructionType) {
            case ConstructionType.Community:
                return communityG[teamKey];
            case ConstructionType.Factory:
                return factoryG[teamKey];
            case ConstructionType.Fort:
                return fortG[teamKey];
            default: return null;
        }
    }
    public GameObject PT(ShipType _shipType, int teamKey = 0) {
        switch (_shipType) {
            case ShipType.CivilianShip:
                return civilianshipG[teamKey];
            case ShipType.MilitaryShip:
                return militaryshipG[teamKey];
            case ShipType.FlagShip:
                return flagshipG[teamKey];
            default: return null;
        }
    }
    public GameObject PT(BulletType _bulletType, int teamKey = 0) {
        switch (_bulletType) {
            case BulletType.Laser:
                return laserG[teamKey];
            case BulletType.Plasma:
                return plasmaG[teamKey];
            case BulletType.Shell:
                return shellG[teamKey];
            case BulletType.Missile:
                return missileG[teamKey];
            case BulletType.Arc:
                return arcG[teamKey];
            default: return null;
        }
    }
    public Vector3 CellToMap(int x, int y){
        return new Vector3(y, 50 - x, 0);
    }
    private System.Random random = new System.Random();
    public int GetRand(){
        return random.Next();
    }
}
