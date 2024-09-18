using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KeySpace
{
	public class KeyBindMenu : MonoBehaviour
    {
        public KeyMenu KM;
        public KeyBind Bind;
        public int ID;
        public Text Name;
        public Toggle toggle;
        public UnityEngine.UI.Button Key1;
        public UnityEngine.UI.Button Axis1;
        public UnityEngine.UI.Button Key2;
        public UnityEngine.UI.Button Axis2;
        public Text KeyText1;
        public Text AxisText1;
        public Text KeyText2;
        public Text AxisText2;
        public Text ThresholdText;

        public bool SelectingK1;
        public bool SelectingK2;
        public bool SelectingA1;
        public bool SelectingA2;

        public int Create(int i, KeyValuePair<KeyBinds, KeyBind> e, KeyMenu km, float YPos)
        {
            transform.position += new Vector3(0, YPos, 0);
            KM = km;
            Bind = e.Value;
            ID = i++;
            Name.text = e.Key.ToString();

            toggle.isOn = Bind.Toggle;
            toggle.onValueChanged.AddListener(fliptoggle);

            Key1.onClick.AddListener(GetNewKey1);
            KeyText1.text = Bind.Key1.ToString();

            Key2.onClick.AddListener(GetNewKey2);
            KeyText2.text = Bind.Key2.ToString();


            Axis1.onClick.AddListener(GetNewAxis1);
            AxisText1.text = Bind.Axis1.ToString();

            Axis2.onClick.AddListener(GetNewAxis2);
            AxisText2.text = Bind.Axis2.ToString();
            return i;
        }
        public void fliptoggle(bool v) 
        {
            Bind.Toggle = v;
        }
        public void GetNewKey1()
        {
            SelectingK1 = true;
            KeyText1.text = ">>Press Key<<";
        }
        public void GetNewKey2()
        {
            SelectingK2 = true;
            KeyText2.text = ">>Press Key<<";
        }
        public void GetNewAxis1()
        {
            SelectingA1 = true;
            AxisText1.text = ">>Do Analog?<<";
        }
        public void GetNewAxis2()
        {
            SelectingA2 = true;
            AxisText2.text = ">>Do Analog?<<";
        }

        private void Update()
        {
            if (SelectingK1)
            {
                if (Input.anyKey)
                {
                    Bind.Key1 = KM.GetKeyPressed();//KM.LastPressed;
                    KeyText1.text = Bind.Key1.ToString();
                    SelectingK1 = false;
                    return;
                }
            }
            if (SelectingK2)
            {
                if (Input.anyKey)
                {
                    Bind.Key2 = KM.GetKeyPressed();//KM.LastPressed;
                    KeyText2.text = Bind.Key2.ToString();
                    SelectingK2 = false;
                    return;
                }
            }
            if (SelectingA1)
            {
                string name = KM.GetAxisPressed();
                if (name != "none")
                {
                    Bind.Axis1 = name;//KM.LastPressed;
                    AxisText1.text = name;
                    SelectingA1 = false;
                }
            }
            if (SelectingA2)
            {
                string name = KM.GetAxisPressed();
                if (name != "none")
                {
                    Bind.Axis2 = name;//KM.LastPressed;
                    AxisText2.text = name;
                    SelectingA2 = false;
                }
            }
        }
    }
}