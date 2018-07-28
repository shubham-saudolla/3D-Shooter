/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100f;
    public float muzzleVelocity = 35f;
    private float _nextShotTime;
    public Transform shell;
    public Transform shellEjector;

    public void Shoot()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = (Time.time + msBetweenShots / 1000);
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.setSpeed(muzzleVelocity);

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
        }
    }
}
