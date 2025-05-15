using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class HelpPanelControl : MonoBehaviour
{
    public GameObject helpPanel;
    public void helpPanelButtonClicked()
    {
        helpPanel.SetActive(true);
    }
    public void helpPanelButtonExit()
    {
        helpPanel.SetActive(false);
    }
}
