using UnityEngine;

public class SpawnMarker : MonoBehaviour , SavableFab
{
    public int Team;

    public void LoadFab(mapdata.savedata data)
	{
		throw new System.NotImplementedException();
	}

	public mapdata.savedata SaveFab()
	{
		throw new System.NotImplementedException();
	}

	public struct SpawnMarkerSavedata : mapdata.savedata
	{
		public int BoolNum()
		{
			throw new System.NotImplementedException();
		}

		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			throw new System.NotImplementedException();
		}

		public Transform LoadObject()
		{
			throw new System.NotImplementedException();
		}
	}
}