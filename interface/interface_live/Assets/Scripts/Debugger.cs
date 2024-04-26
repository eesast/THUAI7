using System.Collections;
using System.Collections.Generic;
using Protobuf;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
public class Debugger : SingletonMono<Debugger>
{
    public string info;
    public TextMeshProUGUI text;
    public MessageOfShip messageOfShip;
    public MessageOfObj messageOfObj;
    public MessageToClient messageToClient;
    public MessageOfAll messageOfAll;

    void Start()
    {
        // messageOfShip = new MessageOfShip()
        // {
        //     X = 1145,
        //     Y = 1919,
        //     Speed = 0,
        //     Hp = 0,
        //     Armor = 0,
        //     Shield = 0,
        //     TeamId = 0,
        //     PlayerId = 0,
        //     Guid = 0,
        //     ShipState = 0,
        //     ShipType = 0,
        //     ViewRange = 0,
        //     ProducerType = 0,
        //     ConstructorType = 0,
        //     ArmorType = 0,
        //     ShieldType = 0,
        //     WeaponType = 0,
        //     FacingDirection = 0.0
        // };

        // messageOfObj = new MessageOfObj()
        // {
        //     ShipMessage = messageOfShip,
        // };

        // messageToClient = new MessageToClient();
        // messageOfAll = new MessageOfAll()
        // {

        //     GameTime = 1,
        //     RedTeamScore = 2,
        //     BlueTeamScore = 3,
        //     RedTeamEnergy = 4,
        //     BlueTeamEnergy = 5,
        //     RedHomeHp = 6,
        //     BlueHomeHp = 7,
        // };
        // messageToClient.ObjMessage.Add(messageOfObj);
        // messageToClient.GameState = GameState.GameRunning;
        // messageToClient.AllMessage = messageOfAll;
        // info = JsonConvert.SerializeObject(messageToClient);
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // info = System.IO.File.ReadAllText(@"D:\SoftwareDepartment\THUAI7\THUAI7\interface\interface_live\Assets\frame.json");

    }

    // Update is called once per frame
    void Update()
    {
        // messageToClient.AllMessage.GameTime++;
        // info = JsonConvert.SerializeObject(messageToClient);
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // text.text = CoreParam.messageOfFrame.messageToClient.ToString() + "\n\nGameTime:" + CoreParam.messageOfFrame.messageToClient.AllMessage.GameTime;//JsonConvert.SerializeObject(CoreParam.messageOfFrame.messageToClient);


    }
}
