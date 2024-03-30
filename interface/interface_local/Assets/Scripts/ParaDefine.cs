using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaDefine : SingletonMono<ParaDefine>
{
    public BulletData laserData, plasmaData, shellData, missileData, arcData;
    public ProducerData producer1Data, producer2Data, producer3Data;
    public ShipData civilShipData, militaryShipData, flagShipData;
}
