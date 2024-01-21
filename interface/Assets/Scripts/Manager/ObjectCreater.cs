using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreater : SingletonMono<ObjectCreater>
{
    public GameObject CreateObject(GameObject targetG, Vector2 position, Quaternion quaternion, Transform targetPa, int teamKey = 0)
    {
        GameObject obj = Instantiate(targetG, position, quaternion, targetPa);
        if (targetG.transform.childCount == 0)
            return obj;
        Debug.Log("step1");
        RendererControl.GetInstance().SetColToChild(teamKey, obj.transform);
        Debug.Log("step3");
        // obj.transform.Find("mask1").GetComponent<Renderer>().GetPropertyBlock(CurrentPropertyBlock);
        // Tuple<MaterialPropertyBlock, MaterialPropertyBlock> col = RendererControl.GetInstance().GetColFromTeam(teamKey, CurrentPropertyBlock);
        // obj.transform.Find("mask1").GetComponent<Renderer>().SetPropertyBlock(col.Item1);
        // obj.transform.Find("mask2").GetComponent<Renderer>().SetPropertyBlock(col.Item2);
        return obj;
    }
}
