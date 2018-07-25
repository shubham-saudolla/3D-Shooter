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
    private Rigidbody _rb;
    private Vector3 _velocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _velocity * Time.deltaTime);
    }

    public void Move(Vector3 _velocity)
    {
        this._velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrected = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrected);
    }
}
