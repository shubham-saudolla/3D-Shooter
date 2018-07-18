/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public LayerMask collisionMask;
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
		float _moveDistance = speed * Time.deltaTime;
		CheckCollsions(_moveDistance);
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

	void CheckCollsions(float moveDistance)
	{
		Ray _ray = new Ray(transform.position, transform.forward);
		RaycastHit _hit;

		//QueryTriggerInteraction allows us to collide with triggers
		if(Physics.Raycast(_ray, out _hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
		{
			OnHitObject(_hit);
		}
	}

	void OnHitObject(RaycastHit hit)
	{
		print(hit.collider.gameObject.name);
		GameObject.Destroy(gameObject);
	}
}
