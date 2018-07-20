/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
	public float startingHealth;
	protected float health;
	protected bool dead;

	public event System.Action onDeath;

	protected virtual void Start()
	{
		health = startingHealth;
	}

	public void TakeHit(float damage, RaycastHit hit)
	{
		health -= damage;

		if(health <= 0 && !dead)
		{
			Die();
		}
	}

	protected void Die()
	{
		dead = true;
		if(onDeath != null)
		{
			onDeath();
		}
		GameObject.Destroy(gameObject);
	}
}
