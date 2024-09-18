using System.Collections.Generic;
using UnityEngine;

namespace KeySpace
{
    public class KeyMenu : MonoBehaviour 
    {
        KeySystem KS;
        public KeyBindMenu BindPrefab;
        public RectTransform ScrollView;

        private int[] values;
        private string[] names = new string[]{
            "Left X",
            "Left Y",
            "Right X",
            "Right Y",
            "Mouse X",
            "Mouse Y",
            "Mouse ScrollWheel",
            "RB",
            "LB",
            "RT",
            "LT",
            "Dpad Horizontal",
            "Dpad Vertical",
        };

        public KeyCode LastPressed;
		private void Awake()
		{			
            KS = KeySystem.instance;

		}
		private void OnEnable()
        {
			if (values != null)
                return;

            int i = 0;
            float size = 40;
            ScrollView.sizeDelta = new Vector2(ScrollView.sizeDelta.x, KS.KeyBindsInstance.Count * size * 1.75f + size / 2);
            foreach (KeyValuePair<KeyBinds, KeyBind> e in KS.KeyBindsInstance)
            {
                KeyBindMenu KBM = Instantiate(BindPrefab, ScrollView);
                i = KBM.Create(i, e,this,((-.25f - i) * size) );// - ((ScrollView.rect.height - size) / 2));
            }
            values = (int[])System.Enum.GetValues(typeof(KeyCode));
        }

        private void OnDisable()
        {
            KS.Save();
        }

        public void Reset() 
        {
            KS.ResetKeys();
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
        public string GetAxisPressed()
        {
            //string[] names = Input.GetJoystickNames();
            //Debug.Log(names.ToString() + " " + names.Length);
            for (int i = 0; i < names.Length; i++)
            {
                Debug.Log("Test Axis: " +  names[i]);
                if (Input.GetAxis(names[i]) > .5f || Input.GetAxis(names[i]) < -.5f)
                {
                    return names[i];
                }
            }
            return "none";
        }

        void Update()
        {
            //for (int i = 0; i < values.Length; i++)
            //{
            //    if (Input.GetKey((KeyCode)values[i]))
            //    {
            //        LastPressed = (KeyCode)values[i];
            //    }
            //}
        }
    }
}