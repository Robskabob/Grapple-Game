using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Platform : MonoBehaviour, IPlatform , SavableFab
{
	[EnumMask]
	public Materials _MaterialMask;
	public Materials MaterialMask { get => _MaterialMask; set =>_MaterialMask = value; }

	[Serializable]
	public struct platformdata : mapdata.savedata
	{
		public Materials MaterialMask;

		public platformdata(Materials materialMask)
		{
			MaterialMask = materialMask;
		}
		public int BoolNum()
		{
			return 0;
		}

#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			EditorGUILayout.EnumFlagsField("Material",MaterialMask);
		}
#endif

		public Transform LoadObject()
		{
			GameObject G = new GameObject();
			Platform P = G.AddComponent<Platform>();
			P._MaterialMask = MaterialMask;
			return G.transform;
		}
	}

	public void LoadFab(mapdata.savedata data)
	{
		platformdata Data = (platformdata)data;
		_MaterialMask = (Materials)Data.MaterialMask;
	}

	public mapdata.savedata SaveFab()
	{
		return new platformdata(_MaterialMask);
	}
}
[Flags]
[Serializable]
public enum Materials
{
	Wood = 1,
	Metal = 2,
	Plastic = 4,
	Energy = 8,
	glass = 16,
}
public interface IPlatform
{ 
	Materials MaterialMask { get; set; }
}
