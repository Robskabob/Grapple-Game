using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Menu
{
    public MainMenu main;
    public Video video;
    public Audio audiosettings;
    public Controls controls;
    public GameMenu game;

    public Menu Selected;
    public Button[] Buttons = new Button[3];
    public Dictionary<Menu,Button> B = new Dictionary<Menu, Button>();

	protected override void Start()
	{
        base.Start();
        B.Add(video, Buttons[0]);
        B.Add(audiosettings, Buttons[1]);
        B.Add(controls, Buttons[2]);
	}

	public override void Open(Menu Next)
	{
        Selected.gameObject.SetActive(false);
        B[Selected].interactable = true;
        Selected = Next;
        B[Selected].interactable = false;
        Selected.gameObject.SetActive(true);
		//base.Open(Next);
	}
}

public abstract class SubOptionMenu : Menu
{
    public Menu Parent;

	public override void Back()
    {
        if (Last == null)
        {
            Debug.LogError("Last == null");
            return;
        }
        gameObject.SetActive(false);
        Last.gameObject.SetActive(true);
        Last = null;
    }
}