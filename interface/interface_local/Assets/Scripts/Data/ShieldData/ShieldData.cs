using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldData", menuName = "GameData/ShieldData", order = 0)]
public class ShieldData : ScriptableObject
{
    public int cost;
    public int shield;
}
