using Mirror;
using System.Linq;
using UnityEngine;
//update grapple length
//make other player pos more smooth
//make host not double speed
public class GrappleHook : Projectile
{
	public Rigidbody ConnectedBody;
	public SpringJoint Joint;
	public FixedJoint Contact;
	public float MaxDist;
	public float reduce = 5;

	[EnumMask] 
	public Materials _MaterialMask;
	public Materials MaterialMask { get => _MaterialMask; set => _MaterialMask = value; }

	public GrapleState Grapled;

	public enum GrapleState
	{
		Shot,			//red		red
		Retract,		//white		yellow
		Loaded,			//orange	green
		GrapledStatic,	//black		blue
		GrapledDynamic,	//green		purple
	}

	private void Start()
	{
		Joint = gameObject.AddComponent<SpringJoint>();
		Joint.connectedBody = ConnectedBody;
	}

	[ClientRpc]
	public void GetParent(GameObject O) 
	{
		HaveParent(O);
	}
	public void HaveParent(GameObject O)
	{
		GrappleGun Gun = O.GetComponent<GrappleGun>();
		Gun.Hook = this;
		transform.SetParent(Gun.shooter);
		transform.localPosition = Vector3.zero;
	}
	//[Command]
	//public void CmdRelease()
	//{
	//	RpcRelease();
	//}
	//[ClientRpc(excludeOwner = true)]
	//public void RpcRelease()
	//{
	//	//unparent platform
	//	transform.parent = null;
	//	Grapled = GrapleState.Retract;
	//	UnFreeze();
	//	//remove physical grapple connection
	//	//Destroy(Joint);//probably slow
	//	//Joint.
	//	Destroy(Contact);
	//}
	public void Release()
	{
		//CmdRelease();
		//unparent platform
		Joint.maxDistance = Vector3.Distance(ConnectedBody.position,transform.position);
		transform.parent = null;
		Grapled = GrapleState.Retract;
		UnFreeze();
		//remove physical grapple connection
		//Destroy(Joint);//probably slow
		Destroy(Contact);
	}

	[Command]
	public void CmdLatch(Vector3 pos, GameObject D)
	{
		RpcLatch(pos,D);
	}
	[ClientRpc(includeOwner = false)]
	public void RpcLatch(Vector3 pos, GameObject D)
	{
		Debug.Log("Latch");
		transform.position = pos;
		//create physical grapple connection
		Joint.tolerance = .1f;
		Joint.autoConfigureConnectedAnchor = false;
		Joint.anchor = Vector3.zero;
		Joint.connectedAnchor = Vector3.zero;
		Joint.maxDistance = Vector3.Distance(transform.position, ConnectedBody.position) - reduce;
		//get other object
		if (D != null)
		{
			Rigidbody other = D.GetComponent<Rigidbody>();
			if (other != null && !other.isKinematic) //check if dynamic
			{
				Grapled = GrapleState.GrapledDynamic;
				Contact = gameObject.AddComponent<FixedJoint>();
				Contact.connectedBody = other;
				Contact.enableCollision = false;
				Contact.autoConfigureConnectedAnchor = false;
				return;
			}
		}
		Grapled = GrapleState.GrapledStatic;
		Freeze();
	}

	public void Latch(Collider col)
	{
		//parent to hit platform
		//transform.parent = col.transform;
		//create physical grapple connection
		//get other object
		Rigidbody other = col.attachedRigidbody;
		//Debug.Log($"Hit {other != null} && {other != null && !other.isKinematic}");
		if (other != null && !other.isKinematic) //check if dynamic
		{
			PlayerController PC = other.GetComponent<PlayerController>();
			Debug.Log($"isNet:{other.GetComponent<NetworkIdentity>() != null} && (isPlayerNull:{PC == null} || isPlayerLocal:{PC == null||PC.LocalPlayer})");
			if (other.GetComponent<NetworkIdentity>() != null && (PC == null || !PC.LocalPlayer))
				CmdLatch(transform.position, col.gameObject);
			else
				return;
			CreateSpring();
			Grapled = GrapleState.GrapledDynamic;
			Contact = gameObject.AddComponent<FixedJoint>();
			Contact.connectedBody = other;
			Contact.enableCollision = false;
			Contact.autoConfigureConnectedAnchor = false;
			return;
		}
		CreateSpring();
		CmdLatch(transform.position, null);
		Grapled = GrapleState.GrapledStatic;
		Freeze();
	}

	public void CreateSpring() 
	{
		//Joint = gameObject.AddComponent<SpringJoint>();
		Joint.tolerance = .1f;
		Joint.autoConfigureConnectedAnchor = false;
		Joint.anchor = Vector3.zero;
		Joint.connectedAnchor = Vector3.zero;
		Joint.connectedBody = ConnectedBody;
		Joint.maxDistance = Vector3.Distance(transform.position,ConnectedBody.position) - reduce;	
	}

	public override void Shoot(Vector3 Pos, Quaternion Rot, float Power, IDamaging owner)
	{
		Owner = owner;
		UnFreeze();
		Util.setAllLayers(gameObject, 0);
		transform.parent = null;
		Grapled = GrapleState.Shot;
		transform.position = Pos;
		transform.rotation = Rot;
		rb.velocity = transform.forward * Power;
		Joint.maxDistance = 1000;
	}
	private void OnTriggerEnter(Collider col)
	{
		if (Grapled == GrapleState.Shot)
		{
			IPlatform Ip = col.GetComponent<IPlatform>();
			if (Ip != null && Util.BitCollison((int)Ip.MaterialMask, (int)MaterialMask))
				Latch(col);
		}
	}

	public void Freeze()
	{
		//gameObject.SetActive(false);
		rb.constraints = RigidbodyConstraints.FreezeAll;
	}
	public void UnFreeze()
	{
		//gameObject.SetActive(true);
		rb.constraints = RigidbodyConstraints.None;
		transform.localScale = Vector3.one;
	}

	public override void Hit(Collision col)
	{

	}
}
