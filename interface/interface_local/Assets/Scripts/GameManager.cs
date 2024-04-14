using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMono<GameManager>
{
    public GameObject gameoverUI;
    public TextMeshProUGUI scoreText;
    public bool gameover;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((MapControl.GetInstance().bases[0] == null || MapControl.GetInstance().bases[1] == null) && !gameover)
        {
            gameoverUI.SetActive(true);
            scoreText.text = "Score:" +
                (MapControl.GetInstance().bases[0].messageOfBase.economy - MapControl.GetInstance().bases[0].messageOfBase.scoreminus).ToString() + ":" +
                (MapControl.GetInstance().bases[1].messageOfBase.economy - MapControl.GetInstance().bases[1].messageOfBase.scoreminus).ToString();
            gameover = true;
        }
    }
    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}
