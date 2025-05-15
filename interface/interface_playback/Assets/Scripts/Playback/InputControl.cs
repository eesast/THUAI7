using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : SingletonMono<InputControl>
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AfterInputPlaySpeed(string playSpeedS)
    {
        float playSpeed = 0f;
        int decNum = 0;
        bool inputDot = false;
        while (playSpeedS != "")
        {
            if (playSpeedS[0] >= '0' && playSpeedS[0] <= '9')
            {
                if (inputDot)
                {
                    decNum++;
                    playSpeed = playSpeed + (playSpeedS[0] - '0') * Mathf.Pow(10, -decNum);
                }
                else
                {
                    playSpeed = playSpeed * 10 + (playSpeedS[0] - '0');
                }
            }
            else if (playSpeedS[0] == '.')
                inputDot = true;
            playSpeedS = playSpeedS.Remove(0, 1);
        }
        CoreParam.playSpeed = playSpeed > 0 ? playSpeed : 1;
        Debug.Log(CoreParam.playSpeed);
    }
    public void AfterInputFilename(string fileName)
    {
        CoreParam.fileName = fileName;
    }
}
