using System.Collections;
using System.Collections.Generic;
using Protobuf;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
public class Debugger : MonoBehaviour
{
    public string info;
    public TextMeshProUGUI text;
    public MessageOfShip messageOfShip;
    public MessageOfObj messageOfObj;

    void Start()
    {
        messageOfShip = new MessageOfShip()
        {
            X = 1145,
            Y = 1919,
            Speed = 0,
            Hp = 0,
            Armor = 0,
            Shield = 0,
            TeamId = 0,
            PlayerId = 0,
            Guid = 0,
            ShipState = 0,
            ShipType = 0,
            ViewRange = 0,
            ProducerType = 0,
            ConstructorType = 0,
            ArmorType = 0,
            ShieldType = 0,
            WeaponType = 0,
            FacingDirection = 0.0
        };

        messageOfObj = new MessageOfObj()
        {
            ShipMessage = messageOfShip,
        };
        info = JsonConvert.SerializeObject(messageOfObj);
        UpdateManager.GetInstance().UpdateMessageByJson(info);
        text.text = info + "\n" + JsonConvert.SerializeObject(CoreParam.messageOfFrame.messageOfObj);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
