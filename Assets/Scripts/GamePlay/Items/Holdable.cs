using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Holdable : NetworkBehaviour, SavableFab
{
	public Rigidbody rb;

	public Hand H;

	public float SnapSpeed = 5;

	


	[ClientRpc]
	public void PickUp(GameObject p,bool isRight)
	{
		ItemController HC = p.GetComponent<ItemController>();
		Hand h;
		if (isRight)
			h = HC.Right;
		else
			h = HC.Left;
		//Debug.Log(h+"|"+p.GetComponent<NetworkIdentity>().netId);
		H = h;
		Util.setAllLayers(gameObject, 16); 
		OnPickUp(h); 
		transform.parent = h.transform;
		rb.useGravity = false;
	}
	[Command]
	public void CmdDrop()
	{
		if (H.Held == null) 
		{
			return;
		}
		H.Held = null;
		H = null;
		Util.setAllLayers(gameObject, 15);
		transform.parent = null;
		rb.useGravity = true;

		netIdentity.RemoveClientAuthority();

		RpcDrop();
		RpcReSet();
	}
	[ClientRpc]
	public void RpcDrop() 
	{
		if(H != null)
		H.Held = null; 
		H = null;
		Util.setAllLayers(gameObject, 15);
		OnDrop();
		transform.parent = null; 
		rb.useGravity = true;
	}
	public virtual void OnPickUp(Hand h) { }
	public virtual void OnDrop() { }


	public virtual void Use() {CmdUse(); }
	public virtual void UseOther() {CmdUseOther(); }
	public virtual void UseOtherDual() {CmdUseOtherDual(); }
	public virtual void ReSet() {CmdReSet(); }

	[Command]
	public virtual void CmdUse() { RpcUse(); }
	[Command]
	public virtual void CmdUseOther() { RpcUseOther(); }
	[Command]
	public virtual void CmdUseOtherDual() { RpcUseOtherDual(); }
	[Command]
	public virtual void CmdReSet() { RpcReSet(); }
	[ClientRpc(includeOwner = true)]
	public virtual void RpcUse() { }
	[ClientRpc(includeOwner = true)]
	public virtual void RpcUseOther() { }
	[ClientRpc(includeOwner = true)]
	public virtual void RpcUseOtherDual() { }
	[ClientRpc(includeOwner = true)]
	public virtual void RpcReSet() { }

	public abstract bool Directional {get;}
	
	public void start()
	{
		rb = GetComponent<Rigidbody>();
		//if (isServer)
		//	NetworkServer.Spawn(gameObject);
	}

	public void update()
	{
		if (H != null)
		{
			transform.position = H.transform.position;
			//rb.velocity = (H.transform.position - transform.position) * SnapSpeed;// + H.PC.rb.velocity;
			if (Directional)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation,H.transform.rotation,Time.deltaTime * SnapSpeed * 100);
			}
		}
	}

	private void Update()
	{
		update();
	}

	private void Start() 
	{
		start();
	}

	public abstract void LoadFab(mapdata.savedata data);

	public abstract mapdata.savedata SaveFab();
}
