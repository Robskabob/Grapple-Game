using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Skripts
{
	public class BrainOld : MonoBehaviour
	{
		public List<LogicBlock> Prosseses;
		public List<LogicObject> Objects;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(BrainOld))]
	public class BrainOldEditor : Editor
	{
		Material mat;
		public bool Graph;
		Vector2 Zoom;
		Vector2 Pos;

		private void OnEnable()
		{
			var shader = Shader.Find("Hidden/Internal-Colored");
			mat = new Material(shader);
		}

		void DrawNode(ILogic Node)
		{
			Rect R = new Rect(5, 5, 5, 5);
			for (int i = 0; i < Node.In.Count; i++)
			{
				//Node.In
				GUI.Button(R, "Node");
			}
			for (int i = 0; i < Node.Out.Count; i++)
			{
				GUI.Button(R, "Node");
			}
			GUI.Box(R, "Name");
		}
		void DrawConnections(Wire Node) { }

		void DrawLogicGraph(Brain B)
		{
			for (int i = 0; i < B.Objects.Count; i++)
			{
				DrawNode(B.Objects[i]);
			}
			for (int i = 0; i < B.Prosseses.Count; i++)
			{
				DrawNode(B.Prosseses[i]);
			}
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			Brain B = (Brain)target;

			Graph = EditorGUILayout.Foldout(Graph, "Graph");
			if (Graph)
			{
				GUILayout.BeginHorizontal(EditorStyles.helpBox);

				Rect rect = GUILayoutUtility.GetRect(10, 10000, 200, 200);

				if (Event.current.type == EventType.Repaint)
				{
					GUI.BeginClip(rect);
					GL.PushMatrix();
					GL.Clear(true, false, Color.black);
					mat.SetPass(0);

					GL.Begin(GL.QUADS);

					Vector2 size = rect.size;
					GL.Color(new Color(.2f, .2f, .2f));
					GL.Vertex3(0, 0, 0);
					GL.Vertex3(0, size.y, 0);
					GL.Vertex3(size.x, size.y, 0);
					GL.Vertex3(size.x, 0, 0);


					GL.Color(Color.green);



					GL.Color(Color.yellow);


					GL.Color(Color.red);


					GL.End();

					//

					GL.Begin(GL.LINES);

					GL.Color(Color.green);



					GL.Color(Color.yellow);


					GL.Color(Color.red);


					GL.End();

					GL.PopMatrix();
					GUI.EndClip();
				}
				GUILayout.EndHorizontal();
			}
		}
	}
#endif
}