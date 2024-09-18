using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : Window
{    
    public override string Name => "Inspector";
    public Text ItemName;
    public Text Position;
    public Text Rotation;
    public Text Scale;

    public Text a;
    public List<Text> Texts;

	private void Update()
	{
        if(Context.Selected != null) 
        {
            Hierarchy.Element E = Context.Selected;
            Position.text = E.T.position.ToString();
            Rotation.text = E.T.rotation.ToString();
            Scale.text = E.T.localScale.ToString();

			if (E is Folder F) 
            {
                ItemName.text = F.Name;
            }
			if (E is PreFab PF) 
            {
                ItemName.text = PF.Name;
                mapdata.savedata sd = PF.Fab.SaveFab();
                a.text = sd.GetType().ToString();
                FieldInfo[] FI = sd.GetType().GetFields();
                if(Texts.Count < FI.Length)
                {
                    for (int i = Texts.Count; i < FI.Length; i++)
                    {
                        Text T = Instantiate(a,a.transform.parent);
                        T.transform.Translate(Vector3.down*40*(i+1));
                        Texts.Add(T);
                    }
                }
                for(int i = 0; i < FI.Length; i++) 
                {
                    Texts[i].text = FI[i].Name + FI[i].GetValue(sd).ToString();                    
                }
                //
                //if (FI.Length > 0)
                //if (FI.Length > 1)
                //    c.text = FI[1].Name + FI[1].GetValue(PF.Fab).ToString();
            }
        }
	}
}
