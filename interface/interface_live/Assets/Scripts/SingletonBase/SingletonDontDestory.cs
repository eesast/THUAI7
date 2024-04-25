using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestory<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T Instance;
    public static T GetInstance()
    {
        return Instance;
    }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
