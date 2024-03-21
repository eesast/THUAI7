using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthText, ecoText;
    BaseControl Base;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!Base)
            Base = GameObject.FindGameObjectWithTag("Base").GetComponent<BaseControl>();
        healthText.text = "Health: " + Base.messageOfBase.hp.ToString();
        healthSlider.value = Base.messageOfBase.hp / 24000f;
        ecoText.text = "Eco: " + Base.messageOfBase.economy.ToString();
    }
    public void BuildCivilship()
    {
        Debug.Log("button pushed");
        PlayerControl.GetInstance().selectedOption = InteractControl.InteractOption.BuildCivil;
    }
    public void BuildMilitaryship()
    {
        PlayerControl.GetInstance().selectedOption = InteractControl.InteractOption.BuildMilitary;
    }
    public void BuildFlagship()
    {
        PlayerControl.GetInstance().selectedOption = InteractControl.InteractOption.BuildFlag;
    }
}
