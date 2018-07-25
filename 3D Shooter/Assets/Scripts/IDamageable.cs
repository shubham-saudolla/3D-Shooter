/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using UnityEngine;

// an interface
public interface IDamageable
{
    // a method that requires an amount of damage as well as a raycast hit, for damaging enemies
    void TakeHit(float damage, RaycastHit hit);

    // a method that does not require a raycast hit, used to damage the player
    void TakeDamage(float damage);
}
