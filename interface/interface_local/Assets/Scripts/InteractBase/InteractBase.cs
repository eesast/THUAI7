using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBase : MonoBehaviour
{

    public bool tobeSelected, selected;
    public GameObject selectedSign, tobeSelectedSign, tobeseObj, seObj;
    void Update()
    {
        if (tobeSelected)
        {
            if (!tobeseObj)
                tobeseObj = Instantiate(tobeSelectedSign, transform.position, Quaternion.identity);
        }
        else
        {
            if (tobeseObj)
                Destroy(tobeseObj);
        }
        if (selected)
        {
            if (!seObj)
                seObj = Instantiate(selectedSign, transform.position, Quaternion.identity);
        }
        else
        {
            if (seObj)
                Destroy(seObj);
        }
    }
}