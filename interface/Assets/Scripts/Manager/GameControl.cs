using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : SingletonDontDestory<GameControl>
{
    public enum GameState
    {
        StartMenu, Live, Playback, Play, Test, Pause
    }
    public GameState gameState;
    public bool initialized = false;
    public void LoadLiveScene()
    {
        GameControl.GetInstance().gameState = GameControl.GameState.Live;
        SceneManager.LoadScene("Gaming");
        initialized = false;
    }
    public void LoadPlaybackScene()
    {
        GameControl.GetInstance().gameState = GameControl.GameState.Playback;
        SceneManager.LoadScene("Gaming");
        initialized = false;
    }
    public void LoadPlayScene()
    {
        GameControl.GetInstance().gameState = GameControl.GameState.Play;
        SceneManager.LoadScene("Gaming");
        initialized = false;
    }
    public void LoadTestScene()
    {
        GameControl.GetInstance().gameState = GameControl.GameState.Test;
        SceneManager.LoadScene("TestScene");
        initialized = false;
    }
}
