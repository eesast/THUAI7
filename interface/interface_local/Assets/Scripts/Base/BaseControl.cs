using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControl : MonoBehaviour
{
    public InteractBase interactBase;
    public Vector2[] generatePos = new Vector2[2];
    [SerializeField]
    public MessageOfBase messageOfBase;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        interactBase = GetComponent<InteractBase>();
        generatePos[0] = transform.Find("GeneratePosition1").position;
        generatePos[1] = transform.Find("GeneratePosition2").position;
        StartCoroutine(economyUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        switch (interactBase.interactOption)
        {
            case InteractControl.InteractOption.BuildCivil:
                obj = ObjCreater.GetInstance().CreateObj(ShipType.CIVILIAN_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                break;
            case InteractControl.InteractOption.BuildMilitary:
                obj = ObjCreater.GetInstance().CreateObj(ShipType.MILITARY_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                break;
            case InteractControl.InteractOption.BuildFlag:
                obj = ObjCreater.GetInstance().CreateObj(ShipType.FLAG_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                break;
            default: break;
        }
    }
    IEnumerator economyUpdate()
    {
        messageOfBase.economy++;
        yield return new WaitForSeconds(1);
        StartCoroutine(economyUpdate());
    }
}
