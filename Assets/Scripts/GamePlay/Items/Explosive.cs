using System;
using UnityEngine;
using Mirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Explosive : Damageable, SavableFab, IPlatform , IDamaging
{
	public float Range;
	public float Force;
	public float Damage;
	public float time;
	public ParticleSystem PS;

	public Materials MaterialMask { get => _MaterialMask; set => _MaterialMask = value; }

	public int Team => -1;

	[EnumMask]
	public Materials _MaterialMask;
	private void Start()
	{
		if (isServer)
		{
			Debug.Log("Can Explode");
			OnDeath += Explode;
		}
	}
	[ClientRpc]
	public void ClientBoom()
	{
		PS.Play();
	}
	private void Update()
	{
		time -= Time.deltaTime;
	}
	public void Explode(object sender, EventArgs e)
	{
		if (time > 0)
			return;
		time = 1;

		ClientBoom();
		Collider[] Cols = Physics.OverlapSphere(transform.position,Range);

		for (int i = 0; i < Cols.Length; i++) 
		{
			Rigidbody rb = Cols[i].attachedRigidbody;
			if (rb == null)
				continue;

			float Dist = Vector3.Distance(transform.position, rb.transform.position);
			float proprtion = Dist / Range;
			Damageable D = Cols[i].GetComponent<Damageable>();
			if (D == null)
				continue;
			D.DealDamage(Damage * proprtion, this);
			if (rb != null) 
			{
				rb.AddExplosionForce(Force,transform.position,Range);
			}
		}
	}

	public void LoadFab(mapdata.savedata data)
	{
		ExplosiveData Data =((ExplosiveData)data);

		Range = Data.Range;
		Force = Data.Force;
		Damage = Data.Damage;
		MaterialMask = Data.Materials;
	}

	public mapdata.savedata SaveFab()
	{
		return new ExplosiveData(this);
	}
	[Serializable]
	public struct ExplosiveData : mapdata.savedata
	{
		public float Range;
		public float Force;
		public float Damage;
		public Materials Materials;

		public ExplosiveData(Explosive e)
		{
			Range = e.Range;
			Force = e.Force;
			Damage = e.Damage;
			Materials = e.MaterialMask;
		}

		public int BoolNum()
		{
			return 0;
		}
#if UNITY_EDITOR
		public void Draw(ref bool[] boolObs, ref int boolIndex)
		{
			EditorGUILayout.FloatField("Range ", Range);
			EditorGUILayout.FloatField("Force ", Force);
			EditorGUILayout.FloatField("Damage ", Damage);
			EditorGUILayout.EnumFlagsField("Material", Materials);
		}
#endif
		public Transform LoadObject()
		{
			Explosive e = Instantiate(PrefabList.instance.GetFab("Explosive")).GetComponent<Explosive>();
			e.Range = Range;
			e.Force = Force;
			e.Damage = Damage;
			e.MaterialMask = Materials;
			return e.transform;
		}
	}
}