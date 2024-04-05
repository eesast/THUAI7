using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelControl : MonoBehaviour
{
    public Dictionary<InteractControl.InteractOption, GameObject> buttonDic = new Dictionary<InteractControl.InteractOption, GameObject>();
    public GameObject button_, obj;
    void Start()
    {
        foreach (InteractControl.InteractOption option in Enum.GetValues(typeof(InteractControl.InteractOption)))
        {
            if (option == InteractControl.InteractOption.None)
                continue;
            // Debug.Log("Add button " + option);
            obj = Instantiate(button_, transform.Find("Content"));
            obj.name = "Button_" + option.ToString();
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = InteractControl.GetInstance().textDic[option];
            // Debug.Log(obj.GetComponent<Button>());
            buttonDic.TryAdd(option, obj);
            // Debug.Log(buttonDic.Count);
            buttonDic[option].GetComponent<Button>().onClick.AddListener(() => { PlayerControl.GetInstance().ButtonInteract(option); });
        }
    }

    void Update()
    {
        foreach (InteractControl.InteractOption option in Enum.GetValues(typeof(InteractControl.InteractOption)))
        {
            if (option == InteractControl.InteractOption.None)
                continue;
            if (PlayerControl.GetInstance().enabledInteract.Contains(option))
            {
                buttonDic[option].SetActive(true);
            }
            else
            {
                buttonDic[option].SetActive(false);
            }
        }
    }
}