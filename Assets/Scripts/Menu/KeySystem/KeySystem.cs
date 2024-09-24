using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using c = KeySpace.Bind.bind;
using k = KeySpace.Bind.KeyBind;
using a = KeySpace.Bind.AxisBind;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KeySpace
{
	[Serializable]
    public struct keydata
    {
        public control[] Control;
        public bool Gameplay;

        public keydata(control[] C, bool gameplay)
        {
            Control = C;
            Gameplay = gameplay;
        }

        public interface control{ }

        [Serializable]
        public struct key : control
        {
            public KeyCode Key;
            public bool Toggle;
            public key(KeyCode key, bool toggle = false) 
            {
                Key = key;
                Toggle = toggle;
            }
        }

        [Serializable]
        public struct axis : control
        {
            public string Axis;
            public float Threshold;
            public float Mult;

            public axis(string axis, float mult = 1, float threshold = .5f) 
            {
                Axis = axis;
                Mult = mult;
                Threshold = threshold;
            }
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
            KeyBindsInstance.Add(KeyBinds.LookUp, new Bind(new c[] { new k(KeyCode.UpArrow), new a("Mouse Y"), new a("Right Y", -1) }));
            KeyBindsInstance.Add(KeyBinds.LookLeft, new Bind(new c[] { new k(KeyCode.LeftArrow), new a("Mouse X",-1), new a("Right X", -1) }));
            KeyBindsInstance.Add(KeyBinds.LookDown, new Bind(new c[] { new k(KeyCode.DownArrow), new a("Mouse Y",-1), new a("Right Y") }));
            KeyBindsInstance.Add(KeyBinds.LookRight, new Bind(new c[] { new k(KeyCode.RightArrow), new a("Mouse X"), new a("Right X") }));

            KeyBindsInstance.Add(KeyBinds.Forward, new Bind(new c[] { new k(KeyCode.W), new a("Left Y", -1) }));
            KeyBindsInstance.Add(KeyBinds.Left, new Bind(new c[] { new k(KeyCode.A), new a("Left X", -1) }));
            KeyBindsInstance.Add(KeyBinds.Backward, new Bind(new c[] { new k(KeyCode.S), new a("Left Y") }));
            KeyBindsInstance.Add(KeyBinds.Right, new Bind(new c[] { new k(KeyCode.D), new a("Left X") }));

            KeyBindsInstance.Add(KeyBinds.Jump, new Bind(new c[] { new k(KeyCode.Space), new k(KeyCode.JoystickButton0) }));
            KeyBindsInstance.Add(KeyBinds.Sprint, new Bind(new c[] { new k(KeyCode.LeftShift), new k(KeyCode.JoystickButton9) }));
            KeyBindsInstance.Add(KeyBinds.Crouch, new Bind(new c[] { new k(KeyCode.LeftControl), new k(KeyCode.JoystickButton1) }));

            KeyBindsInstance.Add(KeyBinds.ShootLeft, new Bind(new c[] { new k(KeyCode.Mouse0), new a("LT") }));
            KeyBindsInstance.Add(KeyBinds.ShootRight, new Bind(new c[] { new k(KeyCode.Mouse1), new a("RT") }));
            KeyBindsInstance.Add(KeyBinds.ShootMid, new Bind(new c[] { new k(KeyCode.Mouse2) }));
            KeyBindsInstance.Add(KeyBinds.UseLeft, new Bind(new c[] { new k(KeyCode.Q), new k(KeyCode.JoystickButton4), new a("LB") }));
            KeyBindsInstance.Add(KeyBinds.UseRight, new Bind(new c[] { new k(KeyCode.E), new k(KeyCode.JoystickButton5), new a("RB") }));
            KeyBindsInstance.Add(KeyBinds.DropLeft, new Bind(new c[] { new k(KeyCode.Z), new a("Dpad Horizontal", -1) }));
            KeyBindsInstance.Add(KeyBinds.DropRight, new Bind(new c[] { new k(KeyCode.X), new a("Dpad Horizontal") }));

            KeyBindsInstance.Add(KeyBinds.UseAbility, new Bind(new c[] { new k(KeyCode.V), new a("Dpad Vertical") }));
            KeyBindsInstance.Add(KeyBinds.DropAbility, new Bind(new c[] { new k(KeyCode.C), new a("Dpad Vertical", -1) }));

            KeyBindsInstance.Add(KeyBinds.Reset, new Bind(new c[] { new k(KeyCode.R), new k(KeyCode.JoystickButton6) }));
            KeyBindsInstance.Add(KeyBinds.MainMenu, new Bind(new c[] { new k(KeyCode.Escape), new k(KeyCode.JoystickButton7) }, false));
        }

        private KeySystem()
        {
        }

        public Dictionary<KeyBinds, Bind> KeyBindsInstance = new Dictionary<KeyBinds, Bind>();
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
			List<keydata> data = new List<keydata>(KeyBindsInstance.Count);
            string str = "Ver: 0.0.2.3\n";
            int i = 0;
            foreach (KeyValuePair<KeyBinds, Bind> e in KeyBindsInstance)
            {
                str += $"{(char)e.Key}:{(e.Value.Toggle ? 'T' : 'F')},{(e.Value.GamePlay ? 'T' : 'F')}|";

                keydata.control[] binds = new keydata.control[e.Value.Binds.Length];

                for (int j = 0; j < e.Value.Binds.Length; j++)
                {
                    Bind.bind b = e.Value.Binds[j];
                    if (b is Bind.KeyBind k)
                    {
                        binds[j] = new keydata.key(k.Key,k.Toggle);
                        str += $"k:{(byte)k.Key},{(k.Toggle ? 'T' : 'F')};";
                    }
                    else if (b is Bind.AxisBind a)
                    {
                        binds[j] = new keydata.axis(a.Axis,a.Mult,a.Threshold);
                        for (int x = 0; x < Axis.names.Length; x++)
                        {
                            if(Axis.names[x] == a.Axis)
                                str += $"a:{(byte)x},{a.Mult},{a.Threshold};";
                        }
                    }
                    else
                    {
                        Assert.IsTrue(false, $"control is nither a key or axis, this is imposible {b}");
                    }
                }
                str += "\n";


                data.Add( new keydata(binds, e.Value.GamePlay));
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
                KeyBindsInstance.Add((KeyBinds)i, new Bind(data[i]));
            }
            Assert.AreEqual(KeyBindsInstance.Count, 22,$"Incorrect numbre of keybinds {KeyBindsInstance.Count} should be 22");
            KeyBinds k = (KeyBinds)UnityEngine.Random.Range(0, 22);
            Assert.IsTrue(KeyBindsInstance[k].Binds.Length > 0,$"KeyBind {k} has no Binds {KeyBindsInstance[k].Binds.Length}");
        }

        public void Update() 
        {
            //make event based
            foreach (KeyValuePair<KeyBinds, Bind> e in KeyBindsInstance)
            {
                if (isPaused && e.Value.GamePlay)
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

    public class Bind
    {
        public Bind(bind[] binds, bool gamePlay = true)
        {
            Binds = binds;
            GamePlay = gamePlay;
            isEnabled = true;
        }

        public Bind(keydata keydata)
        {
            bind[] binds = new bind[keydata.Control.Length];

            for (int i = 0; i < binds.Length; i++)
            {
                keydata.control b = keydata.Control[i];
                if (b is keydata.key k)
                {
                    binds[i] = new k(k.Key, k.Toggle);
                }
                else if (b is keydata.axis a)
                {
                    binds[i] = new a(a.Axis, a.Mult, a.Threshold);
                }
                else
                {
                    Assert.IsTrue(false, $"control is nither a key or axis, this is imposible {b}");
                }
            }

            Binds = binds;
            GamePlay = keydata.Gameplay;
            isEnabled = true;
        }

        public bind[] Binds;
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
                {
                    foreach (var C in Binds)
                    {
                        if (C.GetBool())
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            } }
        public bool Down { get
            {
                if (isEnabled)
                {
                    foreach (var C in Binds)
                    {
                        if (C.GetDown())
                            return true;
                    }
                    return false;
                }
                else
                    return false;
            }
        }
        public float GetValue
        {
            get
            {
                if (isEnabled) 
                {
                    float v = 0;
                    foreach (var C in Binds)
                    {
                        v += C.GetValue();
                    }
                    return v;
                }
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



        public interface bind
        {
            public float GetValue();
            public bool GetBool();
            public bool GetDown();
            public string GetName();
        }

        public class KeyBind : bind
        {
            public KeyCode Key;
            public bool Toggle;
            public static Dictionary<KeyCode, string> NameMap = new Dictionary<KeyCode, string>() {
            {KeyCode.JoystickButton0,"A / ╳" },
            {KeyCode.JoystickButton1,"B / ◯" },
            {KeyCode.JoystickButton2,"X / □" },
            {KeyCode.JoystickButton3,"Y / △" },
            {KeyCode.JoystickButton4,"LB" },
            {KeyCode.JoystickButton5,"RB" },
            {KeyCode.JoystickButton6,"View" },
            {KeyCode.JoystickButton7,"Menu ≡" },
            {KeyCode.JoystickButton8,"L stick" },
            {KeyCode.JoystickButton9,"R stick" },
            //{KeyCode.JoystickButton10,"" },
            //{KeyCode.JoystickButton11,"" },
            {KeyCode.JoystickButton12,"Home" },
        };
            public KeyBind(KeyCode key, bool toggle = false)
            {
                Key = key;
                Toggle = toggle;
            }
            public float GetValue()
            {
                return Input.GetKey(Key) ? 1 : 0;
            }
            public bool GetBool()
            {
                return Input.GetKey(Key);
            }

            public bool GetDown()
            {
                return Input.GetKeyDown(Key);
            }

            public string GetName()
            {
                if (!NameMap.TryGetValue(Key, out string label))
                    label = Key.ToString();
                return label;
            }
        }

        public class AxisBind : bind
        {
            public string Axis;
            public float Threshold;
            public float Mult;

            public AxisBind(string axis, float mult = 1, float threshold = .5f)
            {
                Axis = axis;
                Mult = mult;
                Threshold = threshold;

                down = false;
            }

            private bool down;

            public float GetValue()
            {
                return Input.GetAxis(Axis) * Mult;
            }
            public bool GetBool()
            {
                return Input.GetAxis(Axis) * Mult > Threshold;
            }

            public bool GetDown()
            {
                bool now = GetBool();
                if (!down && now)
                {
                    down = now;
                    return true;
                }
                down = now;
                return false;
            }

            public string GetName()
            {
                return Axis;
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
        ///Move Modifiers
        Jump,
        Sprint,
        Crouch,
        ///Arms
        ShootLeft,
        ShootRight,
        ShootMid,
        UseLeft,
        UseRight,
        DropLeft,
        DropRight,
        ///Abilities
        UseAbility,
        DropAbility,
        ///Utility
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
        }
        bool autoUpdate;
        int rate;
        int count;
        bool keybool;
        bool[] keybools = new bool[0];
        bool[][] keybindbools = new bool[0][];


        public override bool RequiresConstantRepaint()
        {
            return autoUpdate;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            KeySystem K = (KeySystem)target;

            autoUpdate = EditorGUILayout.Toggle("Auto Update Inspector",autoUpdate);
            rate = EditorGUILayout.IntField("Rate",rate);

            EditorGUILayout.Vector2Field("Look", KeySystem.GetLook());
            EditorGUILayout.Vector3Field("Move", KeySystem.GetMove());

            if (GUILayout.Button("Save"))
                K.Save();

            keybool = EditorGUILayout.Foldout(keybool,"KeyBinds");
            if (keybool)
            {
                EditorGUI.indentLevel++;
                if (keybools.Length != K.KeyBindsInstance.Count)
                    keybools = new bool[K.KeyBindsInstance.Count];
                if (keybindbools.Length != K.KeyBindsInstance.Count) 
                {
                    keybindbools = new bool[K.KeyBindsInstance.Count][];
                    for (int j = 0; j < keybindbools.Length; j++)
                    {
                        keybindbools[j] = new bool[0];
                    }
                }
                int i = 0;
                foreach (KeyValuePair<KeyBinds, Bind> k in K.KeyBindsInstance)
                {
                    keybools[i] = EditorGUILayout.Foldout(keybools[i],k.Key.ToString());
                    if (keybools[i])
                    {
                        EditorGUI.indentLevel++;
                        Bind b = k.Value;
                        b.Toggle = EditorGUILayout.Toggle("Toggle", b.Toggle);
                        b.GamePlay = EditorGUILayout.Toggle("Game Play", b.GamePlay);
                        EditorGUILayout.FloatField("Value", KeySystem.GetBindValue(k.Key));
                        EditorGUILayout.FloatField("Value",b.GetValue);
                        EditorGUILayout.Toggle("State", b.State);
                        EditorGUILayout.Toggle("Hold", b.Hold);
                        EditorGUILayout.Toggle("Down", b.Down);


                        if (keybindbools[i].Length != b.Binds.Length)
                            keybindbools[i] = new bool[b.Binds.Length];
                        for (int j = 0; j < b.Binds.Length; j++)
                        {
                            Bind.bind c = b.Binds[j];
                            keybindbools[i][j] = EditorGUILayout.Foldout(keybindbools[i][j], c.GetName());
                            if (keybindbools[i][j])
                            {
                                EditorGUI.indentLevel++;
                                if (c is Bind.KeyBind kb)
                                {
                                    kb.Key = (KeyCode)EditorGUILayout.EnumPopup("Key", kb.Key);
                                    kb.Toggle = EditorGUILayout.Toggle("Toggle", kb.Toggle);
                                }
                                else if (c is Bind.AxisBind ab)
                                {
                                    ab.Axis = EditorGUILayout.TextField("Axis", ab.Axis);
                                    EditorGUILayout.LabelField("Axis Name", ab.GetName());
                                    ab.Mult = EditorGUILayout.FloatField("Mult", ab.Mult);
                                    ab.Threshold = EditorGUILayout.FloatField("Threshold", ab.Threshold);
                                }
                                EditorGUILayout.FloatField("Value",c.GetValue());
                                EditorGUILayout.Toggle("Bool",c.GetBool());
                                EditorGUILayout.Toggle("Down",c.GetDown());
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    i++;
                }
                EditorGUI.indentLevel--;
            }
        }
    }
    #endif
}