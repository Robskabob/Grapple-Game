using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    public MainMenu main;
    public LevelSelect levels;
    public MultiMenu multi;

    public void doPlay()
    {
        
        GameManager GM = GameManager.GM;
        GM.StartHost();
        multi.EnterLobby();
        //LocalPlayer.StartNewGame(PlayerPrefs.GetString("LastMap","Race"));
        //GM.l
        //SceneManager.LoadSceneAsync("Level 1");
        //SceneManager.UnloadSceneAsync("Menu");
    }
    public void doLevelSelect()
    {
        main.allOff();
        levels.gameObject.SetActive(true);
        levels.First.Select();
    }
    public void doMultiplayer() 
    {
        main.allOff();
        multi.gameObject.SetActive(true);
        multi.First.Select();
    }
    public void doBack()
    {
        main.allOff();
        main.gameObject.SetActive(true);
        main.First.Select();
    }
}
