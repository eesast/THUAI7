using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using Unity.VisualScripting;
using UnityEngine;

public class InteractBase : MonoBehaviour
{
    public MessageOfObj messageOfObject;
    public bool tobeSelected, selected;
    public GameObject selectedSign, tobeSelectedSign, tobeseObj, seObj;
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