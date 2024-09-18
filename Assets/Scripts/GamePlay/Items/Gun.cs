using Mirror;
using System;
using UnityEngine;

public class Gun : Holdable
{
	public PoolProjectile P;
	public ParticleSystem PS;
	public Transform shooter;
	public float Power;
	public int Clip;
	public int _Clip;
	public float Reload;
	public float FireRate;
	public float _Wait;

	public override bool Directional => true;

	public override void Use() 
	{
		CmdUse();
		//if (_Wait < 0 && _Clip > 0)
		//{
		//	//PoolProjectile p = BulletPool.GetInstance();// Instantiate(P);
		//	P.NetShootNew(shooter.position, shooter.rotation, Power, H.PC);
		//	//NetworkServer.Spawn(p.gameObject);
		//	//Shoot(p.gameObject);
		//	_Wait = FireRate;
		//	_Clip--;
		//	//if (_Clip < 1)
		//	//	RpcUseOther();
		//	//else
		//	//	RpcUse();
		//}
	}
	[Command]
	public override void CmdUse()
	{
		if (_Wait < 0 && _Clip > 0)
		{
			Projectile p = BulletPool.GetInstance();// Instantiate(P);
			p.Shoot(shooter.position, shooter.rotation, Power, H.PC);
			NetworkServer.Spawn(p.gameObject);
			Shoot(p.gameObject);
			_Wait = FireRate;
			_Clip--;
			if (_Clip < 1)
				RpcUseOther();
			else
				RpcUse();
		}
	}
	[ClientRpc]
	public void Shoot(GameObject P) 
	{
		P.GetComponent<Projectile>().Shoot(shooter.position, shooter.rotation, Power, H.PC);
	}
	public override void RpcUse()
	{
		if (!isServer)
		{
			_Wait = FireRate;
			_Clip--;
		}
	}
	public override void RpcUseOtherDual()
	{
		_Wait = Reload;
		_Clip = Clip;
	}

	public override void RpcUseOther()
	{
		_Wait = Reload;
		_Clip = Clip;
	}

	private void Update()
	{
		update();
		_Wait -= Time.deltaTime;
	}

	public override void LoadFab(mapdata.savedata data)
	{

	}

	public override mapdata.savedata SaveFab()
	{
		return null;
	}
}

[RequireComponent(typeof(Rigidbody))]
public class Damageable : NetworkBehaviour 
{
	public float MaxHealth;
	public float Health { get => _Health; protected set => _Health = value; }
	[SerializeField]
	private float _Health;

	public event EventHandler<EventArgs> OnDeath;

	private void Start()
	{
		OnDeath += CmdOnDeath;
	}

	[Server]
	public void DealDamage(float Value, IDamaging DamageSource)
	{
	//	CmdDealDamage(Value,DamageSource);
	//}
	//[Command]
	//public void CmdDealDamage(float Value, GameObject DamageSource)
	//{
		Health -= Value;
		//Health = 1;
		RpcDealDamage(Health,DamageSource.netId);
		if (Health < 0)
		{
			Debug.Log($"Kill {gameObject.name} From {DamageSource.Team}");
			//CmdOnDeath(this, new EventArgs());
			RpcOnDeath(this, new EventArgs());
			//Debug.Log($"{gameObject.name} is Dead with {Health} Health");
		}

	}
	[ClientRpc]
	public void RpcDealDamage(float Value, uint DamageSourceNetID)
	{
		Health = Value;
	}

	[Command]
	public void CmdOnDeath(object sender, EventArgs e)
	{
		RpcOnDeath(sender, e);
		Debug.Log($"{gameObject.name} is Dead with {Health} Health");
	}
	[ClientRpc]
	public void RpcOnDeath(object sender, EventArgs e)
	{
		OnDeath(sender, e);
	}
}

public interface IDamaging 
{
	int Team { get; }
	uint netId { get; }
}