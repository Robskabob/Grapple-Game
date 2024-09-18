using UnityEngine;

public class SaveFolder : MonoBehaviour, SavableFolder<SaveFolder>
{
	public SaveFolder This { get { return this; } }
}