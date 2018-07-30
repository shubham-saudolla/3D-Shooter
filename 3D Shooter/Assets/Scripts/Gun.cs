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

    Vector3 recoilSmoothDampVelocity;

    float recoilAngle;
    float recoilRotSmoothDampVel;

    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);

    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemaininginBurst = burstCount;
    }

    void LateUpdate()
    {
        // animate recoil

        // recoil backwards
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilRotationSettleTime);

        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVel, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
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
            transform.localPosition = Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        transform.LookAt(aimPoint);
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
