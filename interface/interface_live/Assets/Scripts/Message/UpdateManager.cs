using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;
using Newtonsoft.Json;

public class UpdateManager : SingletonMono<UpdateManager>, IUpdateManager
{
    JsonSerializerSettings jSetting = new JsonSerializerSettings
    {

        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
    };
    public void UpdateMessageByJson(string jsonInfo)
    {
        jsonInfo = jsonInfo.Replace("List\"", "\"");
        if (jsonInfo.Contains("mapMessage"))
            CoreParam.firstFrame = JsonConvert.DeserializeObject<MessageToClient>(jsonInfo, jSetting);
        else
            CoreParam.frameQueue.Add(JsonConvert.DeserializeObject<MessageToClient>(jsonInfo, jSetting));
        CoreParam.cnt++;
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