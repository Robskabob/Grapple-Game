using System;
using UnityEngine;

public class DamageablePlayer : Damageable
{
	public PlayerController PC;
	private void Start()
	{
		OnDeath += Respawn;
	}
	public void Respawn(object sender, EventArgs e)
	{
		Health = MaxHealth;
		PC.rb.velocity = Vector3.zero;
		transform.position = PC.SpawnPoint;
		if (PC.HC.Left.Held)
			PC.HC.Left.Held.ReSet();
		if (PC.HC.Right.Held)
			PC.HC.Right.Held.ReSet();
	}
}
