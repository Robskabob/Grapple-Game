using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KeySpace
{
    public class Key : KeyBindMenu.Control<Bind.KeyBind>
    {
        public Toggle toggle;
        public static int[] values = (int[])System.Enum.GetValues(typeof(KeyCode));

        public void fliptoggle(bool v)
        {
            Ref.Toggle = v;
        }
        public override void Rebind()
        {
            Selecting = true;
            Text.text = ">>Press Key<<";
        }
        public override void Create(Bind.KeyBind control)
        {
            Ref = control;
            Text.text = Ref.GetName();
            BindButton.onClick.AddListener(Rebind);
        }
        public override bool GetAny()
        {
            return Input.anyKey;
        }

        public override void OnSelected()
        {
            KeyCode last = GetKeyPressed();
            Ref.Key = last;
            Text.text = Ref.GetName();
            Selecting = false;
        }
        public KeyCode GetKeyPressed()
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (Input.GetKey((KeyCode)values[i]))
                {
                    return (KeyCode)values[i];
                }
            }
            return KeyCode.None;
        }
    }
}