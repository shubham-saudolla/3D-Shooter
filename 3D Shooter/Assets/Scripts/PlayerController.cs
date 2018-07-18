/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	Rigidbody rb;
	Vector3 velocity;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		rb.MovePosition(rb.position + velocity*Time.deltaTime);
	}

	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

	public void LookAt(Vector3 lookPoint)
	{
		Vector3 heightCorrected = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt(heightCorrected);
	}
}
