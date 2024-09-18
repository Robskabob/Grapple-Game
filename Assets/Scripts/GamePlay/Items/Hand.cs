using KeySpace;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
	public PlayerController PC;
    public Holdable Held;
    public float PickUpDistance = 15;

	// Start is called before the first frame update
	void Start()
    {
        
    }
	/*
	[Command]
	public void CmdPickup() 
	{
		Collider[] col = Physics.OverlapSphere(transform.position, PickUpDistance);//mask to holdables
		for (int i = 0; i < col.Length; i++)
		{
			Holdable H = col[i].GetComponent<Holdable>();
			if (H != null && H.H == null)
			{
				H.netIdentity.AssignClientAuthority(netIdentity.connectionToClient);
				H.H = this;
				H.PickUp(gameObject);
				Held = H;
				rpcPickUp(H.gameObject);

				//flip model for left hand
				//Vector3 scale = Held.transform.localScale;
				//if (Left ^ scale.x < 0)
				//	scale.x *= -1;
				//Held.transform.localScale = scale;
				break;
			}
		}
	}

	[ClientRpc]
	public void rpcPickUp(GameObject holdable)
	{
		Held = holdable.GetComponent<Holdable>();
	}

    // Update is called once per frame
	/*
    void UpdateNot()
    {        
        if (Held == null) 
        {
			if (KeySystem.GetBind(KeyBinds.UseRight))
			{
				Collider[] col = Physics.OverlapSphere(transform.position, PickUpDistance);//mask to holdables
				for (int i = 0; i < col.Length; i++)
				{
					Holdable H = col[i].GetComponent<Holdable>();
					if (H != null)
					{
						H.PickUp(this);
						Held = H;
						break;
					}
				}
			}
        }
        else
		{
			if (Input.GetMouseButton(0))
			{
				Held.Use();
			}

			if (Input.GetMouseButton(1))
			{
				Held.UseOther();
			}

			if (KeySystem.GetBind(KeyBinds.UseLeft))
			{
				Held.Drop();
			}
		}
    }
	//*/
}
