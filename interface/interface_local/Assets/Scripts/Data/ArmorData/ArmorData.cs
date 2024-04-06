using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmorData", menuName = "GameData/ArmorData", order = 0)]
public class ArmorData : ScriptableObject
{
    public int cost;
    public int armor;
}
