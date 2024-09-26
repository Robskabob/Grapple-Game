using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Menu : UIBehaviour
{
    public Selectable First;
    public Menu Last;
    protected override void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(First,$"First is NULL {name} dosn't have a default selected UI object Controllers will not be able to use this menu");
    }
    protected virtual void Update()
    {
        if (KeySpace.KeySystem.GetBindDown(KeySpace.KeyBinds.MainMenu)) 
        {
            Back();
        }
    }
    public virtual void Open(Menu Next) 
    {        
		gameObject.SetActive(false);
        Next.gameObject.SetActive(true);
        Next.First.Select();
        Next.Last = this;
    }
    public virtual void Back()
    {
        if(Last == null) 
        {
            Debug.LogError("Last == null");
            return;
        }
        gameObject.SetActive(false);
        Last.gameObject.SetActive(true);
        Last.First.Select();
        Last = null;
    }
}
public class MainMenu : Menu
{
    public OptionsMenu options;
    public PlayMenu play;
    public MultiMenu multi;
    public GameMenu game;
    public LevelSelect levels;
    public Controls controls;
    public Audio audiosettings;
    public Video video;

    public List<Menu> Menus;

    public Transform Light;
    public Transform Camera;
    public EventSystem ES;
    public bool inGame = false;
    // Start is called before the first frame update
    public void allOff()
    {
        options.gameObject.SetActive(false);
        play.gameObject.SetActive(false);
        game.gameObject.SetActive(false);
        levels.gameObject.SetActive(false);
        controls.gameObject.SetActive(false);
        audiosettings.gameObject.SetActive(false);
        video.gameObject.SetActive(false);
        multi.gameObject.SetActive(false);
        gameObject.SetActive(false);

        for (int i = 0; i < Menus.Count; i++)
        {
            Menus[i].gameObject.SetActive(false);
        }
    }

	new protected void Reset()
	{
        gameObject.SetActive(true);
        allOff();

    }

	// Update is called once per frame

	public void doPlay()
    {
        allOff();
        play.gameObject.SetActive(true);
        ES.SetSelectedGameObject(play.First.gameObject);
        Debug.Log(ES.currentSelectedGameObject);
    }
    public void doOptions()
    {
        allOff();
        options.gameObject.SetActive(true);
        options.First.Select();
        Debug.Log(ES.currentSelectedGameObject);
    }
    public void doExitGame()
    {
        Application.Quit();
    }
    public override void Back()
    {
        if (Last == null)
        {
            Application.Quit();
            return;
        }
        gameObject.SetActive(false);
        Last.gameObject.SetActive(true);
        Last = null;
    }
}
