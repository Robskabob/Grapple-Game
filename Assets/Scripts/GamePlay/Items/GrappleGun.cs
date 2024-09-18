using Mirror;
using UnityEngine;

public class GrappleNetworker : NetworkBehaviour
{
	public GrappleGun GrappleGun;

	private void Start()
	{

	}
}

public class GrappleGun : Holdable
{
	public override bool Directional => Hook.Grapled == GrappleHook.GrapleState.Loaded;
	//public NetworkTransformChild NTC;
	public GrappleHook HookFab;
	public GrappleHook Hook;
	public Transform shooter;
	public LineRenderer lr;
	public Renderer LED;
	public MaterialPropertyBlock Block;
	public float Power;
	public float RetractSpeed;
	public float ClimbSpeed;

	public override void OnDrop()
	{
		Hook.Joint.connectedBody = null;
	}
	public override void OnPickUp(Hand h)
	{
		Hook.ConnectedBody = h.PC.rb;
	}
	public override void Use() 
	{
		if (Hook.Grapled == GrappleHook.GrapleState.Loaded)
		{
			if (H != null)
			{
				Hook.Shoot(shooter.position, H.transform.rotation, Power,H.PC);
				CmdUse();
			}
		}
		else if (Hook.Grapled == GrappleHook.GrapleState.GrapledStatic || Hook.Grapled == GrappleHook.GrapleState.GrapledDynamic)
		{
			if (Input.GetKey(KeyCode.LeftControl))//need better control
				Hook.Joint.maxDistance += Time.deltaTime * ClimbSpeed;
			else
				Hook.Joint.maxDistance -= Time.deltaTime * ClimbSpeed;
			if (isOwned)
				CmdUpdateRope(Hook.Joint.maxDistance);
			else
				Debug.Log("Hmm really i thought this was impossible");
		}	
	}
	public override void UseOtherDual()
	{
		UseOther();
	}
	public override void UseOther()
	{
		if (Hook.Grapled == GrappleHook.GrapleState.GrapledStatic || Hook.Grapled == GrappleHook.GrapleState.GrapledDynamic || Hook.Grapled == GrappleHook.GrapleState.Shot)
		{
			Hook.Release();
			CmdUseOther();
		}
	}
	[Command]
	public override void CmdUse()
	{
		Hook.Shoot(shooter.position, H.transform.rotation, Power, H.PC);
		RpcUse();
	}
	[Command]
	public override void CmdUseOther()
	{
		Hook.Release();
		RpcUseOther();
	}
	[ClientRpc(includeOwner = false)]
	public override void RpcUse()
	{
		Hook.Shoot(shooter.position, H.transform.rotation, Power, H.PC);
	}
	[Command]
	public void CmdUpdateRope(float Dist) 
	{
		RpcUpdateRope(Dist);
		Hook.Joint.maxDistance = Dist;
	}
	[ClientRpc(includeOwner = false)]
	public void RpcUpdateRope(float Dist)
	{
		Hook.Joint.maxDistance = Dist;
	}
	[ClientRpc(includeOwner = false)]
	public override void RpcUseOther()
	{
		Hook.Release();
	}
	public override void RpcUseOtherDual()
	{
		if (Hook.Grapled == GrappleHook.GrapleState.GrapledStatic || Hook.Grapled == GrappleHook.GrapleState.GrapledDynamic || Hook.Grapled == GrappleHook.GrapleState.Shot)
		{
			Hook.Release();
		}
	}
	public override void RpcReSet()
	{
		Hook.Release();
		Load();
	}
	public void Load()
	{
		Hook.Freeze();
		Util.setAllLayers(gameObject, 16);
		Hook.transform.parent = shooter;
		Hook.transform.position = shooter.position;
		Hook.transform.rotation = shooter.rotation;

		Hook.Joint.maxDistance = 10;

		Hook.Grapled = GrappleHook.GrapleState.Loaded;
		//NTC.target = Hook.transform;
	}

	private void Update()
	{
		if (H != null)
			//rb.angularVelocity = H.PC.rb.angularVelocity;
		update();
		if (!Directional)
		{
			Vector3 dir = Hook.transform.position - transform.position;
			Quaternion look = Quaternion.LookRotation(dir);
			if (Hook.Grapled == GrappleHook.GrapleState.Retract && H != null)
			{
				look = Quaternion.Slerp(H.transform.rotation, look, Vector2.Distance(transform.position,Hook.transform.position)/10);
			}
			transform.rotation = Quaternion.Slerp(transform.rotation,look,Time.deltaTime * SnapSpeed);
		}

		switch (Hook.Grapled)
		{
			case GrappleHook.GrapleState.Shot:
				Block.SetColor("_EmissionColor", Color.red);
				LED.SetPropertyBlock(Block);
				//LED.material.color = Color.red;
				//LED.material.SetColor("_EmissionColor", Color.red);
				break;
			case GrappleHook.GrapleState.Retract:
				Block.SetColor("_EmissionColor", Color.yellow);
				LED.SetPropertyBlock(Block);
				//LED.material.color = Color.yellow;
				//LED.material.SetColor("_EmissionColor", Color.yellow);
				Hook.rb.velocity = (shooter.position - Hook.transform.position).normalized * RetractSpeed;// * Time.deltaTime;
				Hook.Joint.maxDistance -= Time.deltaTime * RetractSpeed;
				if (Vector3.Distance(shooter.position,Hook.transform.position)<1 || Hook.Joint.maxDistance == 0)
				{
					Load();
				}
				break;
			case GrappleHook.GrapleState.Loaded:
				Block.SetColor("_EmissionColor", Color.green);
				LED.SetPropertyBlock(Block);
				//LED.material.color = Color.green;
				//LED.material.SetColor("_EmissionColor", Color.green);
				break;
			case GrappleHook.GrapleState.GrapledStatic:
				Block.SetColor("_EmissionColor", Color.blue);
				LED.SetPropertyBlock(Block);
				//LED.material.color = Color.blue;
				//LED.material.SetColor("_EmissionColor", Color.blue);
				break;
			case GrappleHook.GrapleState.GrapledDynamic:
				Block.SetColor("_EmissionColor", Color.magenta);
				LED.SetPropertyBlock(Block);
				//LED.material.color = Color.magenta;
				//LED.material.SetColor("_EmissionColor", Color.magenta);
				break;
		}

		if (Hook.Grapled != GrappleHook.GrapleState.Loaded)
		{
			lr.enabled = true;
			lr.SetPosition(0, shooter.position);
			lr.SetPosition(1, Hook.transform.position);
		}
		else
		{
			lr.enabled = false;
		}
	}

	public override void OnStartAuthority()
	{
		RequestHookAuthority();
		//ReSet();
	}

	[Command]
	public void RequestHookAuthority() 
	{
		Hook.netIdentity.AssignClientAuthority(netIdentity.connectionToClient);	
	}

	public void register()
	{
		NetworkClient.RegisterPrefab(HookFab.gameObject, SpawnDelegate, UnspawnHandler);
	}

	GameObject SpawnDelegate(SpawnMessage msg)
	{

		return Instantiate(HookFab).gameObject;
	}

	

	void UnspawnHandler(GameObject spawned)
	{
		Destroy(spawned);
	}

	static bool Registerd = false;
	private void OnEnable()
	{
		Debug.Log("Try reg");
		if (!Registerd)
		{
			Debug.Log("Do reg");
			Registerd = true;
			register();
		}
	}

	private void Start()
	{
		Block = new MaterialPropertyBlock();
		if (NetworkServer.active) {
			Hook = Instantiate(HookFab);
			NetworkServer.Spawn(Hook.gameObject);
			Hook.GetParent(gameObject);
		}
		else if (!NetworkClient.active)
		{
			Hook = Instantiate(HookFab);
			Hook.HaveParent(gameObject);
		}
		start();
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 2;
	}

	public override void LoadFab(mapdata.savedata data)
	{
		if(data is saveData D)
		{
			Power = D.power;
			RetractSpeed = D.retractSpeed;
			ClimbSpeed = D.climbSpeed;

			Hook.MaxDist = D.maxDist;
			Hook.reduce = D.reduce;
			Hook.MaterialMask = D.materialMask;
		}
	}

	public override mapdata.savedata SaveFab()
	{
		return new saveData()
		{
			power = Power,
			retractSpeed = RetractSpeed,
			climbSpeed = ClimbSpeed,

			maxDist = Hook.MaxDist,
			reduce = Hook.reduce,
			materialMask = Hook.MaterialMask,
		};
	}

	public struct saveData : mapdata.savedata
	{
		public float power;
		public float retractSpeed;
		public float climbSpeed;
		//hook
		public float maxDist;
		public float reduce;
		public Materials materialMask;

		public int BoolNum()
		{
			return 0;
		}

		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{

		}

		public Transform LoadObject()
		{
			throw new System.NotImplementedException();
		}
	}
}
