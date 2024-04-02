using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaDefine : SingletonMono<ParaDefine>
{
    public BulletData laserData, plasmaData, shellData, missileData, arcData;
    public ProducerData producer1Data, producer2Data, producer3Data;
    public ShipData civilShipData, militaryShipData, flagShipData;
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
