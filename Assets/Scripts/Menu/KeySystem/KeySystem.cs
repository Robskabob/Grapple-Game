using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KeySpace
{
	[Serializable]
    public struct keydata
    {
        public KeyCode Key1;
        public KeyCode Key2;
        public string Axis1;
        public string Axis2;
        public float Threshold;
        public float Mult;
        public bool Toggle;
        public bool Gameplay;

        public keydata(KeyCode key1, KeyCode key2, string axis1, string axis2,float mult, float threshold, bool toggle, bool gameplay)
        {
            Key1 = key1;
            Key2 = key2;
            Axis1 = axis1;
            Axis2 = axis2;
            Mult = mult;
            Threshold = threshold;
            Toggle = toggle;
            Gameplay = gameplay;
        }
    }
    public class KeySystem : MonoBehaviour
    {
        public bool isPaused;
        public static KeySystem instance { get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                GameObject G = new GameObject();
                _instance = G.AddComponent<KeySystem>();
                return _instance;
            } }
        [SerializeField]
        private static KeySystem _instance;
		
        void Awake()
		{
			DontDestroyOnLoad(this);
			if (_instance == null)
                _instance = this;
            else if(_instance != this)
            {
                Destroy(this);
                Debug.Log("Removed Extra KeySystem");
            }
            Load();
        }
        public void ResetKeys() 
        {
            KeyBindsInstance.Clear();
            KeyBindsInstance.Add(KeyBinds.LookUp, new KeyBind(KeyCode.UpArrow, KeyCode.None, "Mouse Y", "Right Y"));
            KeyBindsInstance.Add(KeyBinds.LookLeft, new KeyBind(KeyCode.LeftArrow, KeyCode.None, "Mouse X", "Right X", -1));
            KeyBindsInstance.Add(KeyBinds.LookDown, new KeyBind(KeyCode.DownArrow, KeyCode.None, "Mouse Y", "Right Y", -1));
            KeyBindsInstance.Add(KeyBinds.LookRight, new KeyBind(KeyCode.RightArrow, KeyCode.None, "Mouse X", "Right X"));

            KeyBindsInstance.Add(KeyBinds.Forward, new KeyBind(KeyCode.W, KeyCode.None, "", "Left Y",-1));
            KeyBindsInstance.Add(KeyBinds.Left, new KeyBind(KeyCode.A, KeyCode.None, "", "Left X", -1));
            KeyBindsInstance.Add(KeyBinds.Backward, new KeyBind(KeyCode.S, KeyCode.None, "", "Left Y"));
            KeyBindsInstance.Add(KeyBinds.Right, new KeyBind(KeyCode.D, KeyCode.None, "", "Left X"));

            KeyBindsInstance.Add(KeyBinds.Jump, new KeyBind(KeyCode.Space, KeyCode.None));
            KeyBindsInstance.Add(KeyBinds.Sprint, new KeyBind(KeyCode.LeftShift, KeyCode.None));
            KeyBindsInstance.Add(KeyBinds.Crouch, new KeyBind(KeyCode.LeftControl, KeyCode.None));

            KeyBindsInstance.Add(KeyBinds.ShootLeft, new KeyBind(KeyCode.Mouse0, KeyCode.None, "", "LT"));
            KeyBindsInstance.Add(KeyBinds.ShootRight, new KeyBind(KeyCode.Mouse1, KeyCode.None, "", "RT"));
            KeyBindsInstance.Add(KeyBinds.ShootMid, new KeyBind(KeyCode.Mouse2, KeyCode.None));
            KeyBindsInstance.Add(KeyBinds.UseLeft, new KeyBind(KeyCode.Q, KeyCode.JoystickButton4, "", "LB"));
            KeyBindsInstance.Add(KeyBinds.UseRight, new KeyBind(KeyCode.E, KeyCode.JoystickButton5, "", "RB"));
            KeyBindsInstance.Add(KeyBinds.DropLeft, new KeyBind(KeyCode.Z, KeyCode.None, "", "Dpad Horizontal",-1));
            KeyBindsInstance.Add(KeyBinds.DropRight, new KeyBind(KeyCode.X, KeyCode.None, "", "Dpad Horizontal"));

            KeyBindsInstance.Add(KeyBinds.UseAbility, new KeyBind(KeyCode.V, KeyCode.None));
            KeyBindsInstance.Add(KeyBinds.DropAbility, new KeyBind(KeyCode.C, KeyCode.None));

            KeyBindsInstance.Add(KeyBinds.Reset, new KeyBind(KeyCode.R, KeyCode.None));
            KeyBindsInstance.Add(KeyBinds.MainMenu, new KeyBind(KeyCode.Escape, KeyCode.None, "", "",1,.5f, false));
        }

        private KeySystem()
        {
        }

        public Dictionary<KeyBinds, KeyBind> KeyBindsInstance = new Dictionary<KeyBinds, KeyBind>();
        /// <summary>
        /// Returns the value of the key bind
        /// </summary>
        /// <param name="K">Key bind identifier</param>
        /// <returns>when dose this show up?</returns>
        public static bool GetBind(KeyBinds K)
        {
            return instance.KeyBindsInstance[K].State;
        }
        /// <summary>
        /// returns if the key bind was just pressed
        /// </summary>
        /// <param name="K"></param>
        /// <returns>when dose this show up?</returns>
        public static bool GetBindDown(KeyBinds K)
        {
            return instance.KeyBindsInstance[K].Down;
        }
        //unneeded
        ///// <summary>
        ///// returns when the key is being held down
        ///// </summary>
        ///// <param name="K"></param>
        ///// <returns></returns>
        //public static bool GetBindHold(KeyBinds K)
        //{
        //    return instance.KeyBindsInstance[K].Hold;
        //}
        public static float GetBindValue(KeyBinds K) 
        {
            return instance.KeyBindsInstance[K].GetValue;
        }
        public static Vector2 GetLook()
        {
            return new Vector2(GetBindValue(KeyBinds.LookRight) - GetBindValue(KeyBinds.LookLeft), GetBindValue(KeyBinds.LookUp) - GetBindValue(KeyBinds.LookDown));
        }
        public static float GetLookX()
        {
            return GetBindValue(KeyBinds.LookRight) - GetBindValue(KeyBinds.LookLeft);
        }
        public static float GetLookY()
        {
            return GetBindValue(KeyBinds.LookUp) - GetBindValue(KeyBinds.LookDown);
        }
        public static Vector3 GetMove()
        {                
            return new Vector3(GetBindValue(KeyBinds.Right) - GetBindValue(KeyBinds.Left),0, GetBindValue(KeyBinds.Forward) - GetBindValue(KeyBinds.Backward));;
        }

        public void Save()
        {
			List<keydata> data =/*//*/ new List<keydata>(KeyBindsInstance.Count);//*/ new keydata[KeyBindsInstance.Count];
            string str = "Ver: 0.0.1.3\n";
            int i = 0;
            foreach (KeyValuePair<KeyBinds, KeyBind> e in KeyBindsInstance)
            {
                str += $"{e.Key}:{e.Value.Key1},{e.Value.Key2},{e.Value.Axis1},{e.Value.Axis2},{e.Value.Mult},{e.Value.Threshold},{e.Value.Toggle},{e.Value.GamePlay} \n";
                data.Add( new keydata(e.Value.Key1, e.Value.Key2, e.Value.Axis1, e.Value.Axis2,e.Value.Mult, e.Value.Threshold, e.Value.Toggle, e.Value.GamePlay));
                i++;
            }

            IO.SaveSystem.Save(str, "SaveData/Settings", "KeyBinds.txt");
            IO.SaveSystem.Save(data, "SaveData/Settings", "KeyBinds.kbs");
            IO.SaveSystem.Save(Newtonsoft.Json.JsonConvert.SerializeObject(data), "SaveData/Settings", "KeyBinds.json");
        }
        public void Load()
        {
			List<keydata> data = IO.SaveSystem.LoadFile<List<keydata>>("SaveData/Settings", "KeyBinds.kbs",out bool fail);
            if (fail || data == null) 
            {
                ResetKeys();
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                KeyBindsInstance.Add((KeyBinds)i, new KeyBind(data[i]));
            }
        }

        public void Update() 
        {
            foreach (KeyValuePair<KeyBinds, KeyBind> e in KeyBindsInstance)
            {
                if (isPaused && e.Value.GamePlay)//posible ineficant 
                {
                    e.Value.isEnabled = false;
                }
                else 
                {
                    e.Value.isEnabled = true;
                }
                e.Value.Update();
            }
        }
    }

    public class KeyBind
    {
        public KeyBind(KeyCode k1, KeyCode k2, string a1 = "", string a2 = "", float m = 1, float t =.5f, bool gamePlay = true)
        {
            Key1 = k1;
            Key2 = k2;
            Axis1 = a1;
            Axis2 = a2;
            Mult = m;
            Threshold = t;
            GamePlay = gamePlay; 
            isEnabled = true; 
        }

        public KeyBind(keydata keydata)
        {
            Key1 = keydata.Key1;
            Key2 = keydata.Key2;
            Axis1 = keydata.Axis1;
            Axis2 = keydata.Axis2;
            Mult = keydata.Mult;
            Threshold = keydata.Threshold;
            Toggle = keydata.Toggle;
            GamePlay = keydata.Gameplay;
            isEnabled = true;
        }

        public KeyCode Key1;
        public KeyCode Key2;
        public string Axis1;
        public string Axis2;
        public float Threshold;
        public float Mult;
        private bool _State; 
        public bool State {
            get
            {
                if (isEnabled)
                    return _State;
                else
                    return false;
            }
            set { _State = value; } }
        public bool Hold { get
            {
                if (isEnabled)
                    return Input.GetKey(Key1) || Input.GetKey(Key2) || GetAxis;
                else
                    return false;
            } }
        public bool Down { get 
            {
                if (isEnabled)
                    return Input.GetKeyDown(Key1) || Input.GetKeyDown(Key2);
                else
                    return false;
            } }

        public bool GetAxis
        {
            get
            {
                if (isEnabled)
                    return (Axis1 != "" && Input.GetAxis(Axis1) * Mult > Threshold) || 
                        (Axis2 != "" && Input.GetAxis(Axis2) * Mult > Threshold);
                else
                    return false;
            }
        }
        public float GetValue
        {
            get
            {
                if (isEnabled)
                    return (Axis1 == "" ? 0 : Input.GetAxis(Axis1) * Mult) + (Axis2 == "" ? 0 : Input.GetAxis(Axis2) * Mult) + (Input.GetKey(Key1) ? 1 : 0) + (Input.GetKey(Key2) ? 1 : 0);
                else
                    return 0;
            }
        }
        public bool Toggle;

        public bool GamePlay { get; internal set; }
        public bool isEnabled { get; set; }
        public void Update() 
        {
            if (Toggle)
            {
                if (Down)
                {
                    State = !State;
                }
            }
            else 
            {
                State = Hold;
            }
        }
    }

    public enum KeyType 
    {
        Normal,
        Toggle,
        Hold,
    }

    public enum KeyBinds
    {
        ///Look
        LookUp,
        LookLeft,
        LookDown,
        LookRight,
        ///Move
        Forward,
        Left,
        Backward,
        Right,
        //Move Modifiers
        Jump,
        Sprint,
        Crouch,
        //Abilities
        ShootLeft,
        ShootRight,
        ShootMid,
        UseLeft,
        UseRight,
        DropLeft,
        DropRight,

        UseAbility,
        DropAbility,

        Reset,
        MainMenu,
    }



    #if UNITY_EDITOR
    [CustomEditor(typeof(KeySystem))]
    public class KeyEditor : Editor
    {
        private void OnEnable()
        {
            KeySystem K = (KeySystem)target;
            //int n = L.map.Objects.Count;
            //for (int i = 0; i < L.map.Objects.Count; i++)
            //{
            //    n += L.map.Objects[i].BoolNum();
            //}
            //boolObs = new bool[n];
        }
        bool keybool;
        bool[] keybools = new bool[0];
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            KeySystem K = (KeySystem)target;
            keybool = EditorGUILayout.Foldout(keybool,"KeyBinds");
            if (keybool)
            {
                if (keybools.Length != K.KeyBindsInstance.Count)
                    keybools = new bool[K.KeyBindsInstance.Count];
                int i = 0;
                foreach (KeyValuePair<KeyBinds, KeyBind> k in K.KeyBindsInstance)
                {
                    keybools[i] = EditorGUILayout.Foldout(keybools[i],k.Key.ToString());
                    if (keybools[i]) 
                    {
                        //EditorGUILayout.EnumPopup("Binding", );
                        KeyBind b = k.Value;
                        EditorGUILayout.EnumPopup("Key", b.Key1);
                        EditorGUILayout.EnumPopup("Key", b.Key2);
                        EditorGUILayout.TextField("Axis", b.Axis1);
                        EditorGUILayout.TextField("Axis", b.Axis2);
                        EditorGUILayout.FloatField("Threshold", b.Threshold);
                        EditorGUILayout.Toggle("Toggle", b.Toggle);
                        EditorGUILayout.Toggle("Game Play", b.GamePlay);
                        EditorGUILayout.FloatField(KeySystem.GetBindValue(k.Key));
                    }
                    i++;
                }
            }
            //if (GUILayout.Button("Save Level")) { PrefabList.instance.Repopulate(); L.Save(); }
            //if (GUILayout.Button("Load File")) L.LoadFile(L.ld.Name);
            //if (Application.isPlaying && GUILayout.Button("Load Level")) L.Load();
            //Objects = EditorGUILayout.Foldout(Objects, "Map Objects");
            //EditorGUI.indentLevel++;
            //if (Objects && L.map.Objects != null)
            //{
            //    List<mapdata.IObject> Objects = L.map.Objects;
            //    int boolIndex = 0;
            //    int n = Objects.Count;
            //    for (int i = 0; i < Objects.Count; i++)
            //    {
            //        n += Objects[i].BoolNum();
            //    }
            //    if (boolObs == null || n != boolObs.Length)
            //    {

            //        boolObs = new bool[n];
            //    }
            //    for (int i = 0; i < Objects.Count; i++)
            //    {
            //        boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], Objects[i].Name());
            //        if (boolObs[boolIndex++])
            //        {
            //            EditorGUI.indentLevel++;
            //            Objects[i].Draw(ref boolObs, ref boolIndex);
            //            EditorGUI.indentLevel--;
            //        }
            //        else
            //        {
            //            boolIndex += Objects[i].BoolNum();
            //        }
            //    }
            //}
            //EditorGUI.indentLevel--;
        }
    }
    #endif
}