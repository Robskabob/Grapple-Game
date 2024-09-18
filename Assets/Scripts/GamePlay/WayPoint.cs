using System.Collections;
using UnityEngine;
using Skripts;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class WayPoint : LogicObject , SavableFab, SavableObject
{
	public Renderer Rend;

	public WayPoint[] Last;
	public WayPoint[] Next;

	public Race R;

	[System.Serializable]
	public struct waypointsavedata : mapdata.savedata
	{
		public mapdata.vec3 Pos;
		public mapdata.vec3 Rot;

		public int[] Last;
		public int[] Next;

		public waypointsavedata(int[] last, int[] next, Vector3 pos, Quaternion rot)
		{
			Pos = pos;
			Rot = rot.eulerAngles;
			Last = last;
			Next = next;
		}

		public int BoolNum()
		{
			return 2;
		}

#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Last Waypoints");
			if (boolObs[boolIndex++])
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < Last.Length; i++)
				{
					EditorGUILayout.IntField("Waypoint " + i, Last[i]);
				}
				EditorGUI.indentLevel--;
			}
			boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Next Waypoints");
			if (boolObs[boolIndex++])
			{
				EditorGUI.indentLevel++;
				for (int i = 0; i < Next.Length; i++)
				{
					EditorGUILayout.IntField("Waypoints " + i, Next[i]);
				}
				EditorGUI.indentLevel--;
			}
		}
#endif
		public Transform LoadObject()
		{
			 throw new System.NotImplementedException();
		}
	}
	public void LoadFab(mapdata.savedata data)
	{
		waypointsavedata Data = (waypointsavedata)data;
		transform.parent.position = Data.Pos;
		transform.parent.rotation = Quaternion.Euler(Data.Rot);
		Last = new WayPoint[Data.Last.Length];
		for (int i = 0; i < Data.Last.Length; i++)
		{
			Last[i] = R.Waypoints[Data.Last[i]];
		}

		Next = new WayPoint[Data.Next.Length];
		for (int i = 0; i < Data.Next.Length; i++)
		{
			Debug.Log(R.Waypoints[Data.Next[i]]);
			Next[i] = R.Waypoints[Data.Next[i]];
		}
	}
	public mapdata.savedata SaveFab()
	{
		return new waypointsavedata();
	}
	public WayPoint()
	{
		Out = new System.Collections.Generic.List<Wire>() {new Wire(this)};
	}
	void Start()
    {
		Rend = GetComponent<Renderer>();
    }
	private void OnTriggerEnter(Collider other)
	{
		PlayerController PC = other.GetComponent<PlayerController>();
		if (PC != null)
		{
			Race.RaceMember RM = R.GetMember(PC);
			if (RM != null)
			{
				for (int i = 0; i < RM.NextPoints.Length; i++)
				{
					if (this == RM.NextPoints[i])
					{
						RM.NextPoints = Next;
						if (this == R.Finish)
						{
							RM.PC.HUD.TopText.text = "Final Time: " + RM.GetTime();
							R.Members.Remove(RM);
						}
						return;
					}
				}
			}
			else if(R.Start == this)
			{
				R.Members.Add(new Race.RaceMember(PC,R,this,Next));
			}
		}
	}

	public Transform LoadObject(mapdata.savedata data)
	{
		GameObject G = new GameObject();
		WayPoint W = G.AddComponent<WayPoint>();
		return G.transform;
	}

	public mapdata.savedata SaveObject()
	{
		return SaveFab();
	}
}

public class Store : MonoBehaviour
{
	public int Points;
}

public class PlayerCustomizerUI : MonoBehaviour 
{

}

//public class Menu : MonoBehaviour
//{
//	public float Spacing;
//	public List<MenuItem> MenuButtons;
//	public List<MenuItem> MenuContent;
//	public class MenuItem : MonoBehaviour
//	{
//
//	}
//}