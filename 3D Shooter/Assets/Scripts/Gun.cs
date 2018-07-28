/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100f;
    public float muzzleVelocity = 35f;
    private float _nextShotTime;
    public Transform shell;
    public Transform shellEjector;
    private MuzzleFlash muzzleFlash;

    public int burstCount;

    bool triggerReleasedSinceLastShot;
    int shotsRemaininginBurst;

    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemaininginBurst = burstCount;
    }

    void Shoot()
    {
        if (Time.time > _nextShotTime)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemaininginBurst == 0)
                {
                    return;
                }
                else
                {
                    shotsRemaininginBurst--;
                }
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                _nextShotTime = (Time.time + msBetweenShots / 1000);
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.setSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemaininginBurst = burstCount;

    }
}
