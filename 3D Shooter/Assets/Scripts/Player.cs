/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
	public float moveSpeed = 5f;
	PlayerController controller;
	Camera viewCamera;

	void Start()
	{
		controller = GetComponent<PlayerController>();
		viewCamera = Camera.main;
	}
	
	void Update()
	{
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move(moveVelocity);

		// passing a ray through the main camera so that it hits the ground at the mouse position
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		// generating a plane by passing the normal to the plane and the in-position
		// the normal is perpendicular to the plane
		// and since the plane lies flat, it's perpendicular would be Vector3.Up
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

		float rayDistance;

		//this will return the ray distance from the camera to the ground
		if(groundPlane.Raycast(ray, out rayDistance))
		{
			//this would return the intersection point by adding the distance to the ray origin
			Vector3 point = ray.GetPoint(rayDistance);
			// Debug.DrawLine(ray.origin, point, Color.red);
			controller.LookAt(point);
		}
	}
}
