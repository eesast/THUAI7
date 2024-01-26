using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    Button LiveButton, PlaybackButton, PlayButton, TestButton;
    // Start is called before the first frame update
    void Start()
    {
        LiveButton = transform.Find("Live").GetComponent<Button>();
        PlaybackButton = transform.Find("Playback").GetComponent<Button>();
        PlayButton = transform.Find("Play").GetComponent<Button>();
        TestButton = transform.Find("Test").GetComponent<Button>();

        LiveButton.onClick.AddListener(GameControl.GetInstance().LoadLiveScene);
        PlaybackButton.onClick.AddListener(GameControl.GetInstance().LoadPlaybackScene);
        PlayButton.onClick.AddListener(GameControl.GetInstance().LoadPlayScene);
        TestButton.onClick.AddListener(GameControl.GetInstance().LoadTestScene);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
