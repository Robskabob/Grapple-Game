using Mirror;
using UnityEngine;

public abstract class Projectile : NetworkBehaviour
{
	public Rigidbody rb;
	public IDamaging Owner;
	public abstract void Shoot(Vector3 Pos, Quaternion rot, float Power, IDamaging Owner);
	public abstract void Hit(Collision col);

	private void OnCollisionEnter(Collision col)
	{
		Hit(col);
	}
}

public abstract class Ability : NetworkBehaviour, SavableFab
{
	public PlayerController PC;

	public virtual void Use() { CmdUse(); }
	[Command]
	public virtual void CmdUse() { RpcUse(); }
	[ClientRpc(includeOwner = true)]
	public virtual void RpcUse() { }


	public abstract void LoadFab(mapdata.savedata data);

	public abstract mapdata.savedata SaveFab();
}

public class JetPack : Ability
{
	public float Fuel;
	public float Force;

	public override void RpcUse()
	{
		PC.rb.AddForce(PC.transform.up*Force);
	}

	#region Save
	public struct JetPackData : mapdata.savedata
	{
		public int BoolNum()
		{
			return 0;
		}

		#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			throw new System.NotImplementedException();
		}
		#endif

		public Transform LoadObject()
		{
			throw new System.NotImplementedException();
		}
	}
	public override void LoadFab(mapdata.savedata data)
	{

	}

	public override mapdata.savedata SaveFab()
	{
		return new JetPackData();
	}
	#endregion
}