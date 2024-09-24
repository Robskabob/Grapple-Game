using System.Collections.Generic;
using UnityEngine;

namespace KeySpace
{
    public class KeyMenu : MonoBehaviour 
    {
        KeySystem KS;
        public KeyBindMenu BindPrefab;
        public RectTransform ScrollView;

        public KeyCode LastPressed;
		private void Awake()
		{			
            KS = KeySystem.instance;

		}
		private void OnEnable()
        {
            int i = 0;
            float Hight = 25;

            foreach (KeyValuePair<KeyBinds, Bind> e in KS.KeyBindsInstance)
            {
                KeyBindMenu KBM = Instantiate(BindPrefab, ScrollView);
                Hight += KBM.Create(ref i, e, this, Hight);
            }
            ScrollView.sizeDelta = new Vector2(ScrollView.sizeDelta.x, Hight);
        }

        private void OnDisable()
        {
            KS.Save();
        }

        public void Reset() 
        {
            KS.ResetKeys();
        }
    }
}