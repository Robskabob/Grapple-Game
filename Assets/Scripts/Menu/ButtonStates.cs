using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStates : MonoBehaviour
{
    public UnityEngine.UI.Button[] Texts;
    public Color SelectedColor;
    public int Selected;

    // Update is called once per frame
    void Update()
    {
        Texts[Selected].targetGraphic.color = SelectedColor;
    }

    public void Select(int Sel) 
    {
        Texts[Selected].targetGraphic.color = Color.white;
        Selected = Sel;
    }
}
