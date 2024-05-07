using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : SingletonMono<UIControl>
{
    public Slider healthSlider1, healthSlider2;
    public TextMeshProUGUI healthText1, healthText2, ecoText1, ecoText2;
    BaseControl base1, base2;
    public GameObject SidebarPanel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateUI();
        if (PlayerControl.GetInstance().enabledInteract.Count == 0)
        {
            SidebarPanel.SetActive(false);
        }
        else
        {

            SidebarPanel.SetActive(true);
        }
    }
    public void updateUI()
    {
        if (GameObject.Find("Base1"))
        {
            if (GameObject.Find("Base1").TryGetComponent<BaseControl>(out base1))
            {
                healthText1.text = "Health: " + base1.messageOfBase.hp.ToString();
                healthSlider1.value = base1.messageOfBase.hp / 24000f;
                ecoText1.text = "$" + base1.messageOfBase.economy.ToString();
            }
        }
        if (GameObject.Find("Base2"))
        {
            if (GameObject.Find("Base2").TryGetComponent<BaseControl>(out base2))
            {
                healthText2.text = "Health: " + base2.messageOfBase.hp.ToString();
                healthSlider2.value = base2.messageOfBase.hp / 24000f;
                ecoText2.text = "$" + base2.messageOfBase.economy.ToString();
            }
        }

    }
}
