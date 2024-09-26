using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SFB;

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
    public void doOpenFolder() 
    {
        string str = "";
        string[] strs = StandaloneFileBrowser.OpenFolderPanel("Open Folder", Application.persistentDataPath, true);
        for (int i = 0; i < strs.Length; i++)
        {
            str += $"{i}|{strs[i]} \n";
        }
        strs = StandaloneFileBrowser.OpenFilePanel("Open Folder", Application.persistentDataPath,"dat", true);
        for (int i = 0; i < strs.Length; i++)
        {
            str += $"{i}|{strs[i]} \n";
        }
        Debug.Log(str);
    }
    public void doLevel1()
    {
        main.inGame = true;
        main.game.CurrentLevel = "Level 7";
		SceneManager.LoadSceneAsync("Level 7", LoadSceneMode.Additive);
		//SceneManager.UnloadSceneAsync("Menu");
    }
}
