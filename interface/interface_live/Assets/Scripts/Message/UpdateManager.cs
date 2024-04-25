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

        CoreParam.messageOfFrame.messageOfObj = JsonConvert.DeserializeObject<MessageOfObj>(jsonInfo, jSetting);
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