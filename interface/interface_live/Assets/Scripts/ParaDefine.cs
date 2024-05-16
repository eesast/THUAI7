using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaDefine : SingletonMono<ParaDefine>
{
    // public BulletData laserData, plasmaData, shellData, missileData, arcData, fortBulletData;
    // public ProducerData producer1Data, producer2Data, producer3Data;
    // public ConstructorData constructor1Data, constructor2Data, constructor3Data;
    // public ShipData civilShipData, militaryShipData, flagShipData;
    // public ConstructionData factoryData, communityData, fortData;
    // public ArmorData armor1Data, armor2Data, armor3Data;
    // public ShieldData shield1Data, shield2Data, shield3Data;
    // public BaseData baseData;
    [Serializable]
    public class litColorSetting
    {
        public litColorSetting(Color _color, float _idensity)
        {
            this.color = _color;
            this.idensity = _idensity;
        }
        public Color color;
        public float idensity;
    }
    public litColorSetting[] Team0Color, Team1Color;
    public litColorSetting[] ResourceColor;
    public litColorSetting[] LaserColor, PlasmaColor;
}

[Serializable]
public struct ShipDic
{
    public int civilShipNum;
    public int militaryShipNum;
    public int flagShipNum;
}