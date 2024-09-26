using System.Collections.Generic;
using UnityEngine.UI;

public class BasicMenu : Menu
{
    public MainMenu main;
    public List<Menu> Menus;
    public List<Button> Buttons;

    protected override void Start()
    {
        for (int i = 0; i < Menus.Count; i++)
        {
            Menu M = Menus[i];
            Button B = Buttons[i];
            B.onClick.AddListener(() => doMenu(i));
        }
    }
    public void doMenu(int i)
    {
        main.allOff();
        Menus[i].gameObject.SetActive(true);
        Menus[i].First.Select();
    }
    public void doBack()
    {
        main.allOff();
        main.gameObject.SetActive(true);
        main.First.Select();
    }
}
