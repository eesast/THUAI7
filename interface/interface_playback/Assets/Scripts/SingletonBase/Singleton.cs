using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>
    where T : new()
{
    private static T Instance;
    public static T GetInstance()
    {
        if (Instance == null)
            Instance = new T();
        return Instance;
    }
}
