using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelControl : MonoBehaviour
{
    public int flag;
    public InteractControl.InteractType interactType;
    public GameObject BaseControlPanel;
    public GameObject ShipControlPanel;
    // Start is called before the first frame update
    void Start()
    {
        flag = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (flag == 1)
        {
            ShipControlPanel.SetActive(false);
            BaseControlPanel.SetActive(true);
        }
        else if (flag == 2)
        {
            ShipControlPanel.SetActive(true);
            BaseControlPanel.SetActive(false);
        }
        else if (flag == 0)
        {
            ShipControlPanel.SetActive(false);
            BaseControlPanel.SetActive(false);
        }

    }
}
