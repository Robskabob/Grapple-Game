using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static KeySpace.KeyBindMenu;

namespace KeySpace
{
    public partial class KeyBindMenu : UIBehaviour
    {
        public KeyMenu KM;
        public Bind Bind;
        public int ID;
        public Text Name;

        public Transform BindsPanel;
        public Key KeyFab;
        public Axis AxisFab;

        public List<MenuBind> Binds;

        public interface MenuBind
        {
            public bool IsSelected();
            public bool GetAny();
            public void OnSelected();
            public void Rebind();
        }
        public abstract class Control<T> : UIBehaviour , MenuBind where T : Bind.bind
        {
            public T Ref;
            public Button BindButton;
            public Text Text;

            public bool Selecting;

            public abstract void Create(T control);
            public abstract bool GetAny();

            public bool IsSelected()
            {
                return Selecting;
            }

            public abstract void OnSelected();
            public abstract void Rebind();
        }

        public float Create(ref int i, KeyValuePair<KeyBinds, Bind> e, KeyMenu km, float Hight)
        {
            name = $"H:{Hight} N:{e.Key}";
            KM = km;
            Bind = e.Value;
            ID = i++;
            Name.text = e.Key.ToString();

            float HightAdd = 0;
            Binds = new List<MenuBind>();
            for (int j = 0; j < Bind.Binds.Length; j++)
            {
                Bind.bind b = Bind.Binds[j];
                HightAdd += 50;
                
                if(b is Bind.KeyBind k) 
                {
                    Key K = Instantiate(KeyFab, BindsPanel);
                    (K.transform as RectTransform).anchoredPosition = new Vector2(0, -HightAdd);
                    K.Create(k);
                    Binds.Add(K);
                }
                else if(b is Bind.AxisBind a)
                {
                    Axis A = Instantiate(AxisFab, BindsPanel);
                    (A.transform as RectTransform).anchoredPosition = new Vector2(0, -HightAdd);
                    A.Create(a);
                    Binds.Add(A);
                }
                else 
                {
                    Assert.IsTrue(false,"control is nither a key or axis, this is imposible");
                }
            }
            (transform as RectTransform).anchoredPosition = new Vector2(0, -Hight);
            (transform as RectTransform).sizeDelta = new Vector2((transform as RectTransform).sizeDelta.x, HightAdd+50);

            return HightAdd + 70;
        }
        public void fliptoggle(bool v) 
        {
            Bind.Toggle = v;
        }

        private void Update()
        {
            foreach (var b in Binds)
            {
                if (b.IsSelected() && b.GetAny()) 
                {
                    b.OnSelected();
                }
            }
        }
    }
}