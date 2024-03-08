using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : MonoBehaviour
{
    public int economy = 0;
    public int health = 24000;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(economyUpdate());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator economyUpdate()
    {
        economy++;
        yield return new WaitForSeconds(1);
        StartCoroutine(economyUpdate());
    }
}
