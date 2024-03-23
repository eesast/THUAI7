using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Singleton<Tool>
{
    System.Random a = new System.Random();
    public int GetRandom(int min, int max)
    {
        return a.Next(min, max);
    }
}
