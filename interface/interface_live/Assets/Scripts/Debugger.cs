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
    public MessageOfShip messageOfShip1, messageOfShip2;
    public MessageOfObj messageOfObj1, messageOfObj2;
    public MessageToClient messageToClient;
    public MessageOfAll messageOfAll;
    public int shipx = 1000;
    void Start()
    {
        // messageOfShip1 = new MessageOfShip()
        // {
        //     X = 1145,
        //     Y = 1919,
        //     Speed = 0,
        //     Hp = 0,
        //     Armor = 0,
        //     Shield = 0,
        //     TeamId = 0,
        //     PlayerId = 1,
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
        // messageOfShip2 = new MessageOfShip()
        // {
        //     X = 1919,
        //     Y = 19190,
        //     Speed = 0,
        //     Hp = 0,
        //     Armor = 0,
        //     Shield = 0,
        //     TeamId = 0,
        //     PlayerId = 1,
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

        // messageOfObj1 = new MessageOfObj()
        // {
        //     ShipMessage = messageOfShip1,
        // };
        // messageOfObj2 = new MessageOfObj()
        // {
        //     ShipMessage = messageOfShip2,
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
        // messageToClient.ObjMessage.Add(messageOfObj1);
        // messageToClient.ObjMessage.Add(messageOfObj2);
        // messageToClient.GameState = GameState.GameRunning;
        // messageToClient.AllMessage = messageOfAll;
        // info = JsonConvert.SerializeObject(messageToClient);
        // Debug.Log("myjson:" + info.ToString());


        // MessageOfNews messageOfNews = new MessageOfNews()
        // {

        // };
        // messageOfNews.NewsCase


        info = System.IO.File.ReadAllText(@"D:\SoftwareDepartment\THUAI7\THUAI7\interface\interface_live\Assets\message(1).json");
        UpdateManager.GetInstance().UpdateMessageByJson(info);
        info = System.IO.File.ReadAllText(@"D:\SoftwareDepartment\THUAI7\THUAI7\interface\interface_live\Assets\message.json");
        MessageOfFort messageOfFort = new MessageOfFort()
        {
            X = 45,
            Y = 50,
            Hp = 100,
            TeamId = 1,
        };
        MessageOfObj messageOfObj = new MessageOfObj()
        {
            FortMessage = messageOfFort,
        };
        messageToClient = JsonConvert.DeserializeObject<MessageToClient>(info, new JsonSerializerSettings
        {

            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        });
        messageToClient.ObjMessage.Add(messageOfObj);
        info = JsonConvert.SerializeObject(messageToClient);
        UpdateManager.GetInstance().UpdateMessageByJson(info);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(info.Contains(@"""shipMessage"": {
        // ""x"": " + shipx.ToString()));
        // info = info.Replace(@"""shipMessage"": {
        //         ""x"": " + shipx.ToString(),
        //         @"""shipMessage"": {
        //         ""x"": " + ((shipx + 10)).ToString());
        // shipx = ((shipx + 10));
        // Debug.Log(info);

        // info = System.IO.File.ReadAllText(@"D:\SoftwareDepartment\THUAI7\THUAI7\interface\interface_live\Assets\message.json");
        // UpdateManager.GetInstance().UpdateMessageByJson(info);

        // info = info.Replace(@"""shipMessage"": {
        //         ""x"": " + shipx.ToString(),
        //         @"""shipMessage"": {
        //         ""x"": " + ((shipx + 10)).ToString());
        // shipx = ((shipx + 10));
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // info = info.Replace(@"""shipMessage"": {
        //         ""x"": " + shipx.ToString(),
        //         @"""shipMessage"": {
        //         ""x"": " + ((shipx + 10)).ToString());
        // shipx = ((shipx + 10));
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // info = info.Replace(@"""shipMessage"": {
        //         ""x"": " + shipx.ToString(),
        //         @"""shipMessage"": {
        //         ""x"": " + ((shipx + 10)).ToString());
        // shipx = ((shipx + 10));
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // info = info.Replace(@"""shipMessage"": {
        //         ""x"": " + shipx.ToString(),
        //         @"""shipMessage"": {
        //         ""x"": " + ((shipx + 10)).ToString());
        // shipx = ((shipx + 10));
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // Debug.Log(info);
        // messageToClient.AllMessage.GameTime++;
        // info = JsonConvert.SerializeObject(messageToClient);
        // UpdateManager.GetInstance().UpdateMessageByJson(info);
        // text.text = CoreParam.messageOfFrame.messageToClient.ToString() + "\n\nGameTime:" + CoreParam.messageOfFrame.messageToClient.AllMessage.GameTime;//JsonConvert.SerializeObject(CoreParam.messageOfFrame.messageToClient);


    }
}
