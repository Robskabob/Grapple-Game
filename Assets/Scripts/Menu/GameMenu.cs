using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : Menu
{
    public OptionsMenu options;
    public MainMenu main;
    public string CurrentLevel;

    public void doResume()
    {
        main.allOff();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void doOptions()
    {
        main.allOff();
        options.gameObject.SetActive(true);
    }
    public void doQuit()
    {
        main.inGame = false;
        //SceneManager.LoadSceneAsync("Menu");
        if (CurrentLevel != null)
            GameManager.GM.Level.UnLoad();
        GameManager.GM.StopHost();
        GameManager.GM.StopServer();
        Time.timeScale = 1;
        CurrentLevel = null;
        main.allOff();
        main.Light.gameObject.SetActive(true);
        main.gameObject.SetActive(true);
    }
}