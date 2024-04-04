using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructorData", menuName = "GameData/ConstructorData", order = 0)]
public class ConstructorData : ScriptableObject
{
    public int cost;
    public int constructSpeed;
}
