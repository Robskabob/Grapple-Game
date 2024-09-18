using Mirror;
using UnityEngine;

public abstract class PoolProjectile : Projectile// where T : PoolProjectile<T>//,P> where P : NetPool<T> 
{
	//public P Pool;
	private void Start()
	{
		//NetworkServer.reg
	}
	public void NetShootNew(Vector3 Pos, Quaternion rot, float Power, IDamaging owner)
	{
		CmdNetShootNew(Pos,rot,Power,owner.netId);
	}
	//public abstract NetPool<T> GetPool(); 
	[Command(requiresAuthority = false)]
	public void CmdNetShootNew(Vector3 Pos, Quaternion rot, float Power, uint owner)
	{
		Projectile p = BulletPool.GetInstance(); //typeof(P).GetMethod("GetInstance",System.Reflection.BindingFlags.Static).Invoke(null,null) as Projectile;// Instantiate(P);
		//might need NetworkClient.spawned
		if (NetworkServer.spawned.TryGetValue(owner, out NetworkIdentity NI)) {
			p.Shoot(Pos, rot, Power, NI.GetComponent<IDamaging>());
			NetworkServer.Spawn(p.gameObject);
			Rpc(Pos, rot, Power, owner, p.GetComponent<NetworkIdentity>().netId);
		}
	}
	[ClientRpc]
	public void Rpc(Vector3 Pos, Quaternion rot, float Power, uint owner, uint Bullet)
	{//might need NetworkClient.spawned
		if (NetworkServer.spawned.TryGetValue(owner, out NetworkIdentity NI))
		{//might need NetworkClient.spawned
			if (NetworkServer.spawned.TryGetValue(Bullet, out NetworkIdentity BI))
			{
				BI.GetComponent<Bullet>().Shoot(Pos,rot,Power,NI.GetComponent<IDamaging>());
			}
		}
	}
}

public class Bullet : PoolProjectile
{
	public float LifeTime;
	public Renderer Ren;
	public ParticleSystem PS;
	public MaterialPropertyBlock Block;

	//public override NetPool<Bullet> Pool => BulletPool.Instance;

	public override void Hit(Collision col)
	{
		//Debug.Log("hit: " + col.gameObject.name);
		if (col.rigidbody == null)
			return;
		Damageable D = col.rigidbody.GetComponent<Damageable>();
		if (isServer && D != null) 
		{
			D.DealDamage(10,Owner);
			LifeTime --;
		}
	}

	private void Update()
	{
		LifeTime -= Time.deltaTime;
		if (LifeTime < 0) 
		{
			if (isServer) 
			{
				BulletPool.PutBackInPool(this);
				NetworkServer.UnSpawn(gameObject);
			}
		}
	}

	public override void Shoot(Vector3 Pos, Quaternion rot, float Power, IDamaging owner)
	{
		//Block.SetColor("_EmissionColor", GameManager.GM.GameMode.Teams[Owner.Team].TeamColor);
		//Block.SetColor("_Color", );
		//Ren.SetPropertyBlock(Block);
		Owner = owner;
		var v = PS.main;
			v.startColor = GameManager.GM.GameMode.Teams[Owner.Team].TeamColor*2;
		transform.position = Pos;
		transform.rotation = rot;
		rb.velocity = transform.forward * Power;
		LifeTime = 1;
	}
}
