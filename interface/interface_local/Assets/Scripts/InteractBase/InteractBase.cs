using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBase : MonoBehaviour
{
    public InteractControl.InteractType interactType;
    public InteractControl.InteractOption interactOption;
    public bool tobeSelected, selected;
    public GameObject selectedSign, tobeSelectedSign, tobeseObj, seObj;
    public Vector2 moveOption, attackOption;
    void Update()
    {
        if (tobeSelected)
        {
            if (!tobeseObj)
                tobeseObj = Instantiate(tobeSelectedSign, transform.position, Quaternion.identity, transform);
        }
        else
        {
            if (tobeseObj)
                Destroy(tobeseObj);
        }
        if (selected)
        {
            if (!seObj)
                seObj = Instantiate(selectedSign, transform.position, Quaternion.identity, transform);
        }
        else
        {
            if (seObj)
                Destroy(seObj);
        }
    }
}