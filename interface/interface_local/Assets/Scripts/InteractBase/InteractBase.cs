using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractBase : MonoBehaviour
{
    public InteractControl.InteractType interactType;
    public InteractControl.InteractOption interactOption;
    public bool tobeSelected, selected;
    public GameObject selectedSign, tobeSelectedSign, tobeseObj, seObj;
    public Vector2 moveOption, attackOption;
    public bool enableMove;
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
        if (tobeseObj)
            tobeseObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (seObj)
            seObj.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}