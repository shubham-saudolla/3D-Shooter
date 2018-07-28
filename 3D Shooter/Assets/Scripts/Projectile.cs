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
    public float destroyAfter = 1.5f;           // the lifetime of the projectile
    public float damage = 1f;
    public float skinWidth = 0.1f;

    void Start()
    {
        // cleaning up the clones to avoid the cluttering up of the hierarchy
        Destroy(this.gameObject, destroyAfter);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
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

        // QueryTriggerInteraction allows us to collide with triggers
        // skinWidth is used to improve the collision detection when the projectile and the enemies are moving at high speeds
        if (Physics.Raycast(_ray, out _hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(_hit.collider, _hit.point);
        }
    }



    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        // print(hit.collider.gameObject.name);
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
