using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : SingletonMono<UIControl>
{
    public Slider healthSlider1, healthSlider2, playSpeedSlider;
    public TextMeshProUGUI healthText1, healthText2, ecoText1, ecoText2, playSpeedText;
    public GameObject SidebarPanel;
    // Start is called before the first frame update
    void Start()
    {
        playSpeedSlider.value = CoreParam.playSpeed * 4;
        playSpeedText.text = CoreParam.playSpeed.ToString() + "x";
    }

    // Update is called once per frame
    void Update()
    {
        updateUI();
        if (PlayerControl.GetInstance().selectedInt.Count == 0)
        {
            SidebarPanel.SetActive(false);
        }
        else
        {
            SidebarPanel.SetActive(true);
        }
        CoreParam.playSpeed = playSpeedSlider.value / 4;
        playSpeedText.text = CoreParam.playSpeed.ToString() + "x";
    }
    public void updateUI()
    {
        if (CoreParam.currentFrame != null)
        {
            healthText1.text = "Health: " + CoreParam.currentFrame.AllMessage.RedHomeHp;
            healthSlider1.value = CoreParam.currentFrame.AllMessage.RedHomeHp / 48000f;
            ecoText1.text = "$" + CoreParam.currentFrame.AllMessage.RedTeamEnergy.ToString();
            healthText2.text = "Health: " + CoreParam.currentFrame.AllMessage.BlueHomeHp;
            healthSlider2.value = CoreParam.currentFrame.AllMessage.BlueHomeHp / 48000f;
            ecoText2.text = "$" + CoreParam.currentFrame.AllMessage.BlueTeamEnergy.ToString();
        }
    }
}
