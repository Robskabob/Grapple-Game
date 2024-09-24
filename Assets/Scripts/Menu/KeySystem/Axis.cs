using UnityEngine;
using UnityEngine.UI;

namespace KeySpace
{
    public class Axis : KeyBindMenu.Control<Bind.AxisBind>
    {
        public InputField MultF;
        public InputField ThresholdF;

        public Slider MultS;
        public Slider ThresholdS;

        public static string[] names = new string[]{
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
        
        public override void Create(Bind.AxisBind control)
        {
            Ref = control;
            Text.text = Ref.GetName();
            BindButton.onClick.AddListener(Rebind);

            MultF.text = Ref.Mult.ToString();
            MultS.value = Ref.Mult;

            ThresholdF.text = Ref.Mult.ToString();
            ThresholdS.value = Ref.Threshold;
        }
        public override void Rebind()
        {
            Selecting = true;
            Text.text = ">>Do Analog?<<";
        }

        public override bool GetAny()
        {
            string name = GetAxisPressed();
            return name != "none";
        }

        public override void OnSelected()
        {
            Ref.Axis = GetAxisPressed();
            Text.text = Ref.Axis;
            Selecting = false;
        }

        public string GetAxisPressed()
        {
            for (int i = 0; i < names.Length; i++)
            {
                Debug.Log("Test Axis: " + names[i]);
                if (Input.GetAxis(names[i]) > .5f || Input.GetAxis(names[i]) < -.5f)
                {
                    return names[i];
                }
            }
            return "none";
        }
    }
}