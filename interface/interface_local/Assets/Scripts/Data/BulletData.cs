using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "GameData/BulletData", order = 0)]
public class BulletData : ScriptableObject
{
    public float attackDistance;
    public float explosionRange;
    public float[] attackDamage;
    public float armorDamageMultiplier;
    public float shieldDamageMultiplier;
    public float speed;
}
