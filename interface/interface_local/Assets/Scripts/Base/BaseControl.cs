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
        messageOfBase.economy = 400;
        StartCoroutine(economyUpdate());
    }

    // Update is called once per frame
    void Update()
    {
        switch (interactBase.interactOption)
        {
            case InteractControl.InteractOption.BuildCivil:
                if (CostEconomy(ParaDefine.GetInstance().civilShipData.cost))
                {
                    obj = ObjCreater.GetInstance().CreateObj(ShipType.CIVILIAN_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                    obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                }
                break;
            case InteractControl.InteractOption.BuildMilitary:
                if (CostEconomy(ParaDefine.GetInstance().militaryShipData.cost))
                {
                    obj = ObjCreater.GetInstance().CreateObj(ShipType.MILITARY_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                    obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                }
                break;
            case InteractControl.InteractOption.BuildFlag:
                if (CostEconomy(ParaDefine.GetInstance().flagShipData.cost))
                {
                    obj = ObjCreater.GetInstance().CreateObj(ShipType.FLAG_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
                    obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
                }
                break;
            default: break;
        }
    }
    IEnumerator economyUpdate()
    {
        AddEconomy(1);
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(economyUpdate());
    }
    public void AddEconomy(int economy)
    {
        messageOfBase.economy += economy;
    }
    public void TakeDamage(int damage)
    {
        messageOfBase.hp -= damage;
        if (messageOfBase.hp < 0)
            messageOfBase.hp = 0;
        if (messageOfBase.hp == 0)
        {
            DestroyBase();
        }
    }
    public bool CostEconomy(int cost)
    {
        if (messageOfBase.economy < cost)
            return false;
        messageOfBase.economy -= cost;
        return true;
    }
    void DestroyBase()
    {
        Destroy(gameObject);
    }
}
