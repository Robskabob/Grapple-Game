using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace Skripts
{
	public class Brain : MonoBehaviour
	{
		public List<LogicBlock> Prosseses = new List<LogicBlock>();
		public List<LogicObject> Objects = new List<LogicObject>();
		public List<LogicNode> Nodes = new List<LogicNode>();

		private void Start()
		{
			
		}
	}


	[Serializable]
	public class LogicNode : ILogic
	{
		public LogicNode(ILogic logic, Vector2 pos)
		{
			Logic = logic;
			rect = new Rect(pos,new Vector2(100,150));
		}

		public ILogic Logic;
		public Rect rect;

		public List<Wire> In { get { return Logic.In; } set { Logic.In = value; } }
		public List<Wire> Out { get { return Logic.Out; } set { Logic.Out = value; } }

		public void Think()
		{
			Logic.Think();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("In", In, typeof(List<Wire>));
			info.AddValue("Out", Out, typeof(List<Wire>)); 
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(Brain))]
	public class BrainEditor : Editor, IHasCustomMenu
	{
		Material mat;
		public bool Graph = false;
		Vector2 Zoom;
		private Vector2 Pos;
		public void MovePos(Vector2 v) { Pos += v; }
		[ContextMenu("Do Something")]
		public void ReSet(Brain B)
		{
			B.Nodes.Clear();
			int i = 0;
			while (i < B.Objects.Count)
			{
				B.Nodes.Add(new LogicNode(B.Objects[i], new Vector2(i * 5, i * 25)));
				i++;
			}
			while (i < B.Prosseses.Count)
			{
				B.Nodes.Add(new LogicNode(B.Prosseses[i], new Vector2(i * 5, i * 25)));
				i++;
			}
			EditorUtility.SetDirty(B);
		}

		private void OnEnable()
		{
			var shader = Shader.Find("Hidden/Internal-Colored");
			mat = new Material(shader);
			MovePos(Vector2.zero);
		}

		LogicNode Targ;
		Vector2 original = Vector2.zero;


		public bool Drag(Brain B, Rect rect, Event e)
		{
			Vector2 mousePos = e.mousePosition;

			switch (e.type)
			{
				case EventType.MouseDown:
					for (int i = 0; i < B.Nodes.Count; i++)
					{
						if (B.Nodes[i].rect.Contains(mousePos))
						{
							original = mousePos - B.Nodes[i].rect.position;
							Targ = B.Nodes[i];
						}
					}
					break;
				case EventType.MouseDrag:
					if (Targ != null)
					{
						Targ.rect.position = mousePos - original;
						EditorUtility.SetDirty(B);
						return true; 
					}
					break;
				case EventType.MouseUp:
					Targ = null;
					break;
			}
			return false;
		}

		void DrawNode(LogicNode Node, Rect rect)
		{
			GUI.Box(Node.rect,"Node Name");
			if (Node.Logic == null) { return; }			
			Vector2 mousePos = Event.current.mousePosition;
			//if (Node.rect.Contains(mousePos) && rect.position != Vector2.zero)
			//{
			//	Node.rect.position = mousePos;
			//	
			//}
			for (int i = 0; i < Node.In?.Count; i++)
			{
				//Handles.Button(Node.rect.center,Quaternion.identity,4,8,Handles.RectangleCap);
				//Handles.RectangleHandleCap();
				GUI.Button(new Rect(Node.rect.x-9, Node.rect.y + i * 20 + 20,10,15),"");
			}
			for (int i = 0; i < Node.Out?.Count; i++)
			{
				GUI.Button(new Rect(Pos.x, Pos.y+1, 15, 5), "Node");
			}
		}
		
		void DrawConnections(Brain B,LogicNode Node)
		{
			if(Node.Logic != null)
				for (int i = 0; i < Node.In.Count; i++)
				{
					LogicNode Other = B.Nodes.Find(x => x == Node.In[i].Parent);
					Handles.DrawBezier(Node.rect.position, Other.rect.position, Node.rect.position + Vector2.left * 50f, Other.rect.position + Vector2.right * 50f, Color.white, null, 2f);
				}
		}

		void DrawLogicGraph(Brain B, Rect rect)
		{
			//for (int i = 0; i < B.Objects.Count; i++)
			//{
			//	DrawNode(B.Objects[i], rect);
			//}
			//for (int i = 0; i < B.Prosseses.Count; i++)
			//{
			//	DrawNode(B.Prosseses[i], rect);
			//}
			for (int i = 0; i < B.Nodes.Count; i++)
			{
				DrawNode(B.Nodes[i], rect);
			}
		}
		void YourCallback()
		{
			Debug.Log("Hi there");
		}
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			Brain B = (Brain)target;
			Event E = Event.current;

			if(GUILayout.Button("Update"))
				ReSet(B);


			Graph = EditorGUILayout.Foldout(Graph, "Graph");
			if (Graph)
			{
				GUILayout.BeginHorizontal(EditorStyles.helpBox);

				Rect rect = GUILayoutUtility.GetRect(10, 10000, 200, 200);
				//EditorGUILayout.Vector2Field("",rect.position);
				
				GUI.BeginClip(rect);

				if (Drag(B, rect, E)) Repaint();
				DrawLogicGraph(B,rect);
				Rect clickArea = EditorGUILayout.GetControlRect();
				//if (rect.Contains(E.mousePosition) && E.type == EventType.ContextClick)
				//{
				//	//Do a thing, in this case a drop down menu
				//
				//	GenericMenu menu = new GenericMenu();
				//
				//	menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
				//	menu.AddItem(new GUIContent("Do a thing"), false, YourCallback);
				//	menu.ShowAsContext();
				//
				//	E.Use();
				//}

				GUI.EndClip();

				GUILayout.EndHorizontal();
				if (GUI.changed) Repaint();
			}
		}


		private GUIContent m_MenuItem1 = new GUIContent("Menu Item 1");
		private GUIContent m_MenuItem2 = new GUIContent("Menu Item 2");
		private bool m_Item2On = false;
		public void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(m_MenuItem1, false, MenuItem1Selected);
			menu.AddItem(m_MenuItem2, m_Item2On, MenuItem2Selected);
		}
		private void MenuItem1Selected()
		{
			Debug.Log("Menu Item 1 selected");
		}

		private void MenuItem2Selected()
		{
			m_Item2On = !m_Item2On;

			Debug.Log("Menu Item 2 is " + m_Item2On);
		}


		//[MenuItem("Tools/Custom Menu Window")]
		//public static void Init_ToolsMenu()
		//{
		//	EditorWindow window = EditorWindow.GetWindow<BrainEditor>();
		//	window.title = "Custom Menu";
		//	window.Show();
		//}
	}
#endif
}