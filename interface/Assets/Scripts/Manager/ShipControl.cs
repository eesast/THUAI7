using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    public int teamId = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameControl.GetInstance().gameState == GameControl.GameState.Test)
        {
            gameObject.AddComponent<TestPlayerControl>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(teamId, gameObject.transform);
    }
}
