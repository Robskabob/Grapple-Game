using Mirror;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static mapdata;

public class Race : NetworkBehaviour , SavableFab , SavableObject
{
	public List<WayPoint> Waypoints;
	public WayPoint Start;
	public WayPoint Finish;

	public List<RaceMember> Members = new List<RaceMember>();

	[System.Serializable]
	public struct racesavedata : savedata
	{
		public WayPoint.waypointsavedata[] Waypoints;
		public int Start;
		public int Finish;

		public racesavedata(Race R)
		{
			Waypoints = new WayPoint.waypointsavedata[R.Waypoints.Count];
			Start = R.Waypoints.FindIndex(a => a == R.Start);
			Finish = R.Waypoints.FindIndex(a => a == R.Finish);
			for (int i = 0; i < Waypoints.Length; i++) 
			{
				WayPoint W = R.Waypoints[i];
				Waypoints[i] = new WayPoint.waypointsavedata(getConections(W.Last,R.Waypoints), getConections(W.Next, R.Waypoints),W.transform.position,W.transform.rotation);
			}
		}

		int[] getConections(WayPoint[] Conlist, List<WayPoint> Mainlist) 
		{
			int[] con = new int[Conlist.Length];
			for (int i = 0; i < Conlist.Length; i++)
			{
				con[i] = Mainlist.FindIndex(a => a == Conlist[i]);
			}
			return con;
		}

#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			EditorGUI.indentLevel++; 
			boolObs[boolIndex] = EditorGUILayout.Foldout(boolObs[boolIndex], "Waypoints");
			if (boolObs[boolIndex++] && Waypoints != null)
				for (int i = 0; i < Waypoints.Length; i++)
				{
					Waypoints[i].Draw(ref boolObs, ref boolIndex);
				}
			else
				boolIndex += BoolNum() - 1;
			EditorGUI.indentLevel--;
			EditorGUILayout.IntField("Start", Start);
			EditorGUILayout.IntField("Finish", Finish);
		}
#endif

		public int BoolNum()
		{
			int n = 1;
			for (int i = 0; i < Waypoints.Length; i++) 
			{
				n += Waypoints[i].BoolNum();
			}
			return n;
		}

		public Transform LoadObject()
		{
			GameObject G = new GameObject();
			Race R = G.AddComponent<Race>();
			R.Waypoints = new List<WayPoint>();
			for (int i = 0; i < Waypoints.Length; i++) 
			{
				Transform T = Instantiate(PrefabList.instance.GetFab("Waypoint"),R.transform);//if broken change name to wataver waypoint is called
				WayPoint w = T.GetComponentInChildren<WayPoint>();
				w.R = R;
				R.Waypoints.Add(w);
			}
			for (int i = 0; i < Waypoints.Length; i++)
			{
				R.Waypoints[i].LoadFab(Waypoints[i]);
			}
			return G.transform;
		}
	}
	public void LoadFab(savedata data)
	{
		racesavedata Data = (racesavedata)data;

		Start = Waypoints[Data.Start];
		Finish = Waypoints[Data.Finish];
	}
	public savedata SaveFab()
	{
		return new racesavedata(this);
	}
	[Serializable]
	public class RaceMember
	{
		public Race R;
		public PlayerController PC;
		public WayPoint LastPoint;
		public WayPoint[] NextPoints;
		public float Time;

		public RaceMember(PlayerController pC,Race R, WayPoint lastPoint, WayPoint[] nextPoints)
		{
			this.R = R;
			PC = pC;
			LastPoint = lastPoint;
			NextPoints = nextPoints;
			Time = 0;

			PC.DP.OnDeath += OnMemberDeath;
		}

		public string GetTime() 
		{
			int Min = (int)(Time / 60 % 60);
			int Sec = (int)((Time - Min * 60));
			int Mill = (int)((Time - Sec - Min * 60) * 100);
			return string.Format("{0:D2}:{1:D2}:{2:D2}",Min,Sec,Mill);//Min + ":" + Sec.ToString() + ":" + Mill;
		}
		void OnMemberDeath(object sender, EventArgs e) 
		{
			PC.HUD.TopText.text = "Failed at: " + GetTime();
			PC.DP.OnDeath -= OnMemberDeath;
			R.Members.Remove(this);
		}
	}
	void Update()
	{
		for (int i = 0; i < Members.Count; i++)
		{
			RaceMember RM = Members[i];
			RM.Time += Time.deltaTime;
			RM.PC.HUD.TopText.text = RM.GetTime();
			for (int j = 0; j < Waypoints.Count; j++)
			{
				WayPoint W = Waypoints[j];
				if (IsNextWaypoint(RM, W))
				{
					W.Rend.material.SetColor("_Base_Color", RM.PC.color);
				}
				else
				{
					W.Rend.material.SetColor("_Base_Color", Color.white);
				}
			}
		}
	}
	bool IsNextWaypoint(RaceMember RM, WayPoint WP)
	{
		for (int i = 0; i < RM.NextPoints.Length; i++)
		{
			if (RM.NextPoints[i] == WP)
				return true;
		}
		return false;
	}
	public RaceMember GetMember(PlayerController PC)
	{
		for (int i = 0; i < Members.Count; i++)
		{
			if (Members[i].PC = PC)
			{
				return Members[i];
			}
		}
		return null;
	}
	public Transform LoadObject(savedata data)
	{
		return ((racesavedata)data).LoadObject();
	}
	public savedata SaveObject()
	{
		return SaveFab();
	}
}
