using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateManager : SingletonMono<UpdateManager>, IUpdateManager
{
    JsonSerializerSettings jSetting = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };
    public void UpdateMessageByJson(string jsonInfo)
    {
        Debugger.GetInstance().text.text = jsonInfo;
        CoreParam.frameQueue.Add(JsonConvert.DeserializeObject<MessageToClient>(jsonInfo, jSetting));
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}