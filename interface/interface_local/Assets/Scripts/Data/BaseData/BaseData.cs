using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseData", menuName = "GameData/BaseData", order = 0)]
public class BaseData : ScriptableObject
{
    public ShipDic maxShipNum;
    public ShipDic initialShipNum;
    public int maxTotalShipNum;
    public int initialEco;
}
