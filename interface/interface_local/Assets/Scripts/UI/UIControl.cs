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
        healthText.text = "Health: " + Base.health.ToString();
        healthSlider.value = Base.health / 24000f;
        ecoText.text = "Eco: " + Base.economy.ToString();
    }
}
