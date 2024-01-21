using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
public class Debugger : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.CivilianShip), new Vector3(0, 0), Quaternion.identity, GameObject.Find("Ship").transform, 0);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.MilitaryShip), new Vector3(5, 0), Quaternion.identity, GameObject.Find("Ship").transform, 0);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.FlagShip), new Vector3(10, 0), Quaternion.identity, GameObject.Find("Ship").transform, 0);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ConstructionType.Fort), new Vector3(15, -5), Quaternion.identity, GameObject.Find("Ship").transform, 0);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.CivilianShip), new Vector3(0, -5), Quaternion.identity, GameObject.Find("Ship").transform, 1);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.MilitaryShip), new Vector3(5, -5), Quaternion.identity, GameObject.Find("Ship").transform, 1);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ShipType.FlagShip), new Vector3(10, -5), Quaternion.identity, GameObject.Find("Ship").transform, 1);
        ObjectCreater.GetInstance().CreateObject(ParaDefine.GetInstance().PT(ConstructionType.Fort), new Vector3(15, -5), Quaternion.identity, GameObject.Find("Ship").transform, 1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
