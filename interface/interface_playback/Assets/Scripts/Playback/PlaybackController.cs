using Playback;
using Protobuf;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlaybackController : MonoBehaviour
{
    // Start is called before the first frame update
    float GameTime = 0;

    byte[] bytes = null;
    MessageToClient responseVal;
    MessageReader reader;
    public static bool isInitial;

    IEnumerator WebReader()
    {
        while (CoreParam.fileName == "")
            yield return 0;
        // if (!CoreParam.fileName.EndsWith(PlayBackConstant.ExtendedName))
        //     CoreParam.fileName += PlayBackConstant.ExtendedName;
        UnityWebRequest request = UnityWebRequest.Get(CoreParam.fileName);
        request.timeout = 5;
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            yield break;
        }
        bytes = request.downloadHandler.data;
        reader = new MessageReader(bytes); responseVal = reader.ReadOne();
        while (responseVal != null)
        {
            ChartControl.GetInstance().score1.Add(new Tuple<int, int>(responseVal.AllMessage.GameTime, responseVal.AllMessage.RedTeamScore));
            ChartControl.GetInstance().score2.Add(new Tuple<int, int>(responseVal.AllMessage.GameTime, responseVal.AllMessage.BlueTeamScore));
            ChartControl.GetInstance().energy1.Add(new Tuple<int, int>(responseVal.AllMessage.GameTime, responseVal.AllMessage.RedTeamEnergy));
            ChartControl.GetInstance().energy2.Add(new Tuple<int, int>(responseVal.AllMessage.GameTime, responseVal.AllMessage.BlueTeamEnergy));
            responseVal = reader.ReadOne();
        }
        // Debug.Log(ChartControl.GetInstance().score[0]);
        reader = new MessageReader(bytes);
    }

    void Start()
    {
        // CoreParam.fileName = "D:\\Users\\hanzhifeng\\Downloads\\初赛回放2.thuaipb";
        StartCoroutine(WebReader());

        isInitial = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (reader != null)
        {
            GameTime += Time.deltaTime * CoreParam.playSpeed;
            if (!isInitial)
                responseVal = reader.ReadOne();
            if (isInitial && responseVal.AllMessage.GameTime / 1000.0f > GameTime)
                return;
            while (isInitial)
            {
                responseVal = reader.ReadOne();
                if (responseVal.AllMessage.GameTime / 1000.0f > GameTime)
                    break;
            }
            // Debug.Log(responseVal.AllMessage.GameTime / 1000.0f + "    " + GameTime);
            if (responseVal == null)
            {
                SceneManager.LoadScene("GameEnd");
            }
            else if (!isInitial)
            {
                CoreParam.map = responseVal.ObjMessage[0].MapMessage;
                CoreParam.firstFrame = responseVal;
                isInitial = true;
            }
            else
            {
                CoreParam.frameQueue.Add(responseVal);
            }
        }
    }

}