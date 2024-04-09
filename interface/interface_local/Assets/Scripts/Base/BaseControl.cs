using TMPro;
using UnityEngine;

public class BaseControl : MonoBehaviour
{
    public InteractBase interactBase;
    public Vector2[] generatePos = new Vector2[2];
    [SerializeField]
    public MessageOfBase messageOfBase;
    public GameObject obj;
    float timer = 0;
    int deltaEco, ecoStamp = 0;
    // Start is called before the first frame update
    void Start()
    {
        interactBase = GetComponent<InteractBase>();
        generatePos[0] = transform.Find("GeneratePosition1").position;
        generatePos[1] = transform.Find("GeneratePosition2").position;
        messageOfBase.economy = ParaDefine.GetInstance().baseData.initialEco;
        messageOfBase.shipNum.civilShipNum = ParaDefine.GetInstance().baseData.initialShipNum.civilShipNum;
        messageOfBase.shipNum.militaryShipNum = ParaDefine.GetInstance().baseData.initialShipNum.militaryShipNum;
        messageOfBase.shipNum.flagShipNum = ParaDefine.GetInstance().baseData.initialShipNum.flagShipNum;
        messageOfBase.shipTotalNum = messageOfBase.shipNum.civilShipNum + messageOfBase.shipNum.militaryShipNum + messageOfBase.shipNum.flagShipNum;
        for (int i = 1; i <= messageOfBase.shipNum.civilShipNum; i++)
            BuildCivil();
        for (int i = 1; i <= messageOfBase.shipNum.militaryShipNum; i++)
            BuildMilitary();
        for (int i = 1; i <= messageOfBase.shipNum.flagShipNum; i++)
            BuildFlag();
    }
    void BuildCivil()
    {
        obj = ObjCreater.GetInstance().CreateObj(ShipType.CIVILIAN_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
        obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
        messageOfBase.shipNum.civilShipNum++;
        messageOfBase.shipTotalNum++;
    }
    void BuildMilitary()
    {
        obj = ObjCreater.GetInstance().CreateObj(ShipType.MILITARY_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
        obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
        messageOfBase.shipNum.militaryShipNum++;
        messageOfBase.shipTotalNum++;
    }
    void BuildFlag()
    {
        obj = ObjCreater.GetInstance().CreateObj(ShipType.FLAG_SHIP, generatePos[Tool.GetInstance().GetRandom(0, 2)]);
        obj.GetComponent<ShipControl>().messageOfShip.playerTeam = messageOfBase.playerTeam;
        messageOfBase.shipNum.flagShipNum++;
        messageOfBase.shipTotalNum++;
    }

    // Update is called once per frame
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(messageOfBase.playerTeam, gameObject.transform, 0.5f);
        switch (interactBase.interactOption)
        {
            case InteractControl.InteractOption.BuildCivil:
                if (messageOfBase.shipNum.civilShipNum < ParaDefine.GetInstance().baseData.maxShipNum.civilShipNum &&
                messageOfBase.shipTotalNum < ParaDefine.GetInstance().baseData.maxTotalShipNum &&
                CostEconomy(ParaDefine.GetInstance().civilShipData.cost))
                {
                    BuildCivil();
                }
                break;
            case InteractControl.InteractOption.BuildMilitary:
                if (messageOfBase.shipNum.militaryShipNum < ParaDefine.GetInstance().baseData.maxShipNum.militaryShipNum &&
                messageOfBase.shipTotalNum < ParaDefine.GetInstance().baseData.maxTotalShipNum &&
                CostEconomy(ParaDefine.GetInstance().militaryShipData.cost))
                {
                    BuildMilitary();
                }
                break;
            case InteractControl.InteractOption.BuildFlag:
                if (messageOfBase.shipNum.flagShipNum < ParaDefine.GetInstance().baseData.maxShipNum.flagShipNum &&
                messageOfBase.shipTotalNum < ParaDefine.GetInstance().baseData.maxTotalShipNum &&
                CostEconomy(ParaDefine.GetInstance().flagShipData.cost))
                {
                    BuildFlag();
                }
                break;
            default: break;
        }
        timer += Time.deltaTime;
        deltaEco = (int)((timer - ecoStamp * 1.0f / 20) * 20);
        ecoStamp += deltaEco;
        AddEconomy(deltaEco);
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
        UIControl.GetInstance().updateUI();
        Destroy(gameObject);
    }
}
