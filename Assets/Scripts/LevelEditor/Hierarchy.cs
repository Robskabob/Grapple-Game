using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hierarchy : Window
{
    public override string Name => "Hierarchy";
    public Folder FolderFab;
    public PreFab PreFabFab;

    public Folder Root;
	private static float Height = 30;

    public RectTransform Content;

    public List<Element> elements;

    public Material selected;
    public static Material Selected;

	public void Load(Level L) 
    {
        Selected = selected;
        int i = 0;
        int l = 0;

        Root = Instantiate(FolderFab, Content);

        Root.Index = i++;
        Root.Indent = l;
        elements.Add(Root);
        LoadFolder(ref i, ref l,L.transform);
        UpdateOrder();
    }

    public void LoadFolder(ref int i, ref int l, Transform T)
    {
        l++;
        foreach (Transform child in T)
        {
            SaveFolder fold = child.GetComponent<SaveFolder>();
            SavableFab fab = child.GetComponent<SavableFab>();

            if (fold != null)
            {
                Folder Fold = Instantiate(FolderFab, Content);
                Fold.Open = true;
                Fold.button.onClick.AddListener(() => UpdateOrder());
                Fold.SaveFolder = fold;
                Fold.SetVals(i++, l,child);
                elements.Add(Fold);
                LoadFolder(ref i, ref l,child);
            }
            else if (fab != null)
            {
                PreFab Fab = Instantiate(PreFabFab, Content);
                Fab.Fab = fab;
                Fab.SetVals(i++, l, child);
                elements.Add(Fab);
            }
        }
        l--;
    }

    public void UpdateOrder() 
    {
        int c = 0;
        int Indent = 0;
        for(int i = 0; i < elements.Count; i++) 
        {
            Element E = elements[i];
            Debug.Log($"C: {c} I: {Indent} E.I: {E.Indent}");
            if (E.Indent > Indent)
            {
                E.gameObject.SetActive(false);
                continue;
            }
            else
                E.gameObject.SetActive(true);
            Folder F = E as Folder;

            RectTransform Rect = E.transform as RectTransform;
            Rect.localPosition = new Vector3(0,c * -Height, 0);
            //Debug.Log($"Pos: {E.transform.position} LocalPos: {E.transform.localPosition} PosRect: {Rect.position}");
            //Debug.Log($"SD: {Rect.sizeDelta} OM: {Rect.offsetMin}");
            Rect.sizeDelta = new Vector2(-(Indent * 20), Height);
            Rect.anchoredPosition = new Vector2(Indent * 20, -c * Height);
            //Rect.offsetMin = new Vector2(Indent * 20, Rect.offsetMin.y);
            //Debug.Log($"SD: {Rect.sizeDelta} OM: {Rect.offsetMin}");
            if (F != null && F.Open)
                Indent++;
            c++;
        }

        Content.sizeDelta = new Vector2(Content.sizeDelta.x, c * Height);
    }


	public abstract class Element : Selectable
    {
        public string Name;
        public int Index;
        public int Indent;

        public Text Text;
        public Image Back;

        public Transform T;
        //public abstract mapdata.IObject Object { get; }

        public void SetVals(int index, int indent,Transform obj) 
        {
            Index = index;
            Indent = indent;
            transform.localPosition = new Vector3(indent*20,(index*-Height),0);
            T = obj;
            Name = obj.name;
            Text.text = obj.name;
        }

		//public override void OnPointerEnter(PointerEventData eventData)
		//{
		//	base.OnPointerEnter(eventData);
        //    Back.color = Over;
		//}
		//public override void OnPointerExit(PointerEventData eventData)
		//{
		//	base.OnPointerEnter(eventData);
        //    //if(Se)
        //    Back.color = Color;
        //}
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
            Context.Selected = this;
        }
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
        }

        public void HighLight() 
        {
            MeshRenderer[] M = T.GetComponentsInChildren<MeshRenderer>(); 
            for (int i = 0; i < M.Length; i++)
            {
                Material[] mats = new Material[M[i].materials.Length + 1];
                for (int j = 0; j < mats.Length-1; j++)
                {
                    mats[j] = M[i].materials[j];
                }
                mats[mats.Length - 1] = Selected;
                M[i].materials = mats;
            }        
        }
        public void DeHighLight() 
        {
            if (T == null)
                return;
            MeshRenderer[] M = T.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < M.Length; i++)
            {
                Material[] mats = new Material[M[i].materials.Length - 1];
                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = M[i].materials[j];
                }
                M[i].materials = mats;
            }
        }
	}
}

public class Context 
{
    public Hierarchy.Element Selected
    {
        get
        {
            return _Selected; 
        } 

        set 
        { 
            if (_Selected != null) 
            { 
                _Selected.DeHighLight(); 
            } 
            _Selected = value;
            _Selected.HighLight();
        } 
    }
    private Hierarchy.Element _Selected;
}

public class Window : UIBehaviour
{
    public static Context Context = new Context();

    public virtual string Name { get; }
    public Text NamePlate;
    public float Width;

	protected override void Start()
	{
        NamePlate.text = Name;
	}
}