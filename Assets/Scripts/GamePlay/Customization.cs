using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Customization : NetworkBehaviour
{
	public Mesh Gun;
	public string Name;
	public Text DisplayName;


	public void ChangeName(string name)
	{
		CmdChangeName(name);
	}
	[Command]
	public void CmdChangeName(string name)
	{
		Name = name;
		DisplayName.text = name;
		RpcChangeName(name);
	}
	[ClientRpc]
	public void RpcChangeName(string name)
	{
		Name = name;
		DisplayName.text = name;
	}
}
