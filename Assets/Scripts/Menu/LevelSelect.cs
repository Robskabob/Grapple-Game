using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//change so that is just gives a level to another menu
public class LevelSelect : Menu
{
    public PlayMenu play;
    public MainMenu main;
    public LevelSelectUI UI;

    public void doBack()
    {
        main.allOff();
        play.gameObject.SetActive(true);
    }
    public void doLevel1()
    {
        main.inGame = true;
        main.game.CurrentLevel = "Level 7";
		SceneManager.LoadSceneAsync("Level 7", LoadSceneMode.Additive);
		//SceneManager.UnloadSceneAsync("Menu");
    }
}
