using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructionData", menuName = "GameData/ConstructionData", order = 0)]
public class ConstructionData : ScriptableObject
{
    public int hpMax;
    public int economyProduceSpeed;
    public int recoverPointNum;
    public float attackRange;
    public int attackDamage;
}
