/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using UnityEngine;

// an interface
public interface IDamageable
{
	void TakeHit(float damage, RaycastHit hit);
}