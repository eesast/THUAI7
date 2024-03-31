using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProducerData", menuName = "GameData/ProducerData", order = 0)]
public class ProducerData : ScriptableObject
{
    public int cost;
    public int miningSpeed;
}
