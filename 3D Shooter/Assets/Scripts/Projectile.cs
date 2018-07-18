/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed = 1f;
	public float destroyAfter = 1.5f;

	void Start()
	{
		// cleaning up the clones to avoid the cluttering up of the hierarchy
		Destroy(this.gameObject, destroyAfter);
	}

	public void setSpeed(float newSpeed)
	{
		this.speed = newSpeed;
	}
	
	void Update()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
}
