using KeySpace;
using Mirror;
using UnityEngine;

public class ItemController : NetworkBehaviour
{
	public Hand Right;
	public Hand Left;
	public Transform Head;

	public Transform Back;
	public Ability Ability;



	[Command]
	public void CmdPickup(bool isRight)
	{
		Hand Hand;
		if (isRight)
			Hand = Right;
		else
			Hand = Left;
		if (Hand.Held != null)
			return;
		//Collider[] col = Physics.OverlapSphere(Hand.transform.position, Hand.PickUpDistance);//mask to holdables
		//for (int i = 0; i < col.Length; i++)
		//Debug.DrawRay(Head.position, Head.forward, Color.blue);
		if (Physics.Raycast(Head.position,Head.forward,out RaycastHit h,Hand.PickUpDistance, 1 << 15))//add a holdable layer
		{
			//Debug.Log($"Hit: {h.transform.name}");
			Holdable H = h.transform.GetComponent<Holdable>();//col[i].GetComponent<Holdable>();
			if (H != null && H.H == null)
			{
				H.netIdentity.AssignClientAuthority(netIdentity.connectionToClient);
				H.H = Hand;
				H.PickUp(gameObject,isRight);
				Hand.Held = H;
				rpcPickUp(H.gameObject,isRight);

				//flip model for left hand
				//Vector3 scale = Held.transform.localScale;
				//if (Left ^ scale.x < 0)
				//	scale.x *= -1;
				//Held.transform.localScale = scale;
				//break;
			}
		}
	}

	[ClientRpc]
	public void rpcPickUp(GameObject holdable,bool isRight)
	{
		Hand Hand;
		if (isRight)
			Hand = Right;
		else
			Hand = Left;
		//Debug.Log($"held{Hand.Held} hand{Hand} netid{holdable.GetComponent<NetworkIdentity>().netId}");
		Hand.Held = holdable.GetComponent<Holdable>();
	}

	private void Update()
	{
		if (!Right.PC.LocalPlayer)
			return;
		if (Right.Held != null)
		{
			if (Left.Held != null)
			{
				if (KeySystem.GetBind(KeyBinds.ShootLeft)){
					Left.Held.Use();
					//Right.Held.UseOtherDual();
				}
				if (KeySystem.GetBind(KeyBinds.ShootRight))
				{
					Right.Held.Use();
					//Left.Held.UseOtherDual();
				}
				if (KeySystem.GetBind(KeyBinds.ShootMid))
				{
					Right.Held.UseOther();
					Left.Held.UseOther();
				}
				if (KeySystem.GetBind(KeyBinds.DropLeft))
				{
					Left.Held.CmdDrop();
				}
				if (KeySystem.GetBind(KeyBinds.DropRight))
				{
					Right.Held.CmdDrop();
				}
				if (KeySystem.GetBind(KeyBinds.UseLeft))
				{
					Left.Held.UseOtherDual();
				}
				if (KeySystem.GetBind(KeyBinds.UseRight))
				{
					Right.Held.UseOtherDual();
				}
			}
			else
			{
				if (KeySystem.GetBind(KeyBinds.ShootRight))
				{
					Right.Held.Use();
				}
				if (KeySystem.GetBind(KeyBinds.UseLeft))
				{
					CmdPickup(false);
				}
				if (KeySystem.GetBind(KeyBinds.DropRight))
				{
					Right.Held.CmdDrop();
				}
				if (KeySystem.GetBind(KeyBinds.UseRight))
				{
					Right.Held.UseOther();
				}
				if (KeySystem.GetBind(KeyBinds.ShootRight))
				{
					Right.Held.UseOtherDual();
				}
			}
		}
		else if (Left.Held != null)
		{
			if (KeySystem.GetBind(KeyBinds.ShootLeft))
			{
				Left.Held.Use();
			}
			if (KeySystem.GetBind(KeyBinds.UseRight))
			{
				CmdPickup(true);
			}
			if (KeySystem.GetBind(KeyBinds.DropLeft))
			{
				Left.Held.CmdDrop();
			}
			if (KeySystem.GetBind(KeyBinds.UseLeft))
			{
				Left.Held.UseOther();
			}
			if (KeySystem.GetBind(KeyBinds.ShootMid))
			{
				Left.Held.UseOtherDual();
			}
		}
		else
		{
			if (KeySystem.GetBind(KeyBinds.UseLeft))
			{
				CmdPickup(false);
			}
			if (KeySystem.GetBind(KeyBinds.UseRight))
			{
				CmdPickup(true);
			}
		}

		if(Ability != null) 
		{
		
		}
	}
}
