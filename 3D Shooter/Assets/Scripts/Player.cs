/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5f;
    private PlayerController _controller;
    private GunController _GunController;
    private Camera _viewCamera;

    protected override void Start()
    {
        base.Start();

        _controller = GetComponent<PlayerController>();
        _GunController = GetComponent<GunController>();
        _viewCamera = Camera.main;
    }

    void Update()
    {
        // movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        _controller.Move(moveVelocity);

        // look input
        // passing a ray through the main camera so that it hits the ground at the mouse position
        Ray ray = _viewCamera.ScreenPointToRay(Input.mousePosition);
        // generating a plane by passing the normal to the plane and the in-position
        // the normal is perpendicular to the plane
        // and since the plane lies flat, it's perpendicular would be Vector3.Up
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        //this will return the ray distance from the camera to the ground
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            //this would return the intersection point by adding the distance to the ray origin
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            _controller.LookAt(point);
        }

        // weapon input, shoots on left mouse button down and space key
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            _GunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            _GunController.OnTriggerRelease();
        }
    }
}
