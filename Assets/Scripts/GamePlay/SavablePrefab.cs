using UnityEngine;

public class SavablePrefab : MonoBehaviour, SavableFab
{
	public void LoadFab(mapdata.savedata data)
	{
		
	}

	public mapdata.savedata SaveFab()
	{
		return new Nulldata();
	}

	public struct Nulldata : mapdata.savedata
	{
		public int BoolNum()
		{
			return 0;
		}

		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			
		}

		public Transform LoadObject()
		{
			Debug.LogError("insttiaite null");
			return default;
		}
	}
}