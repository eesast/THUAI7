using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreater : SingletonMono<ObjectCreater>
{
    public GameObject CreateObject(GameObject targetG, Vector2 position, Quaternion quaternion, Transform targetPa, int teamKey = -1)
    {
        GameObject obj = Instantiate(targetG, position, quaternion, targetPa);
        if (targetG.transform.childCount == 0) return obj;
        Tuple<Color, Color> col = RendererControl.GetInstance().GetColFromTeam(teamKey);
        obj.transform.Find("mask1").GetComponent<SpriteRenderer>().material.color = col.Item1;
        obj.transform.Find("mask2").GetComponent<SpriteRenderer>().material.color = col.Item2;
        return obj;
    }
}
