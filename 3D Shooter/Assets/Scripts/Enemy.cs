/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    // need to include the UnityEngine.AI namespace for NavMeshAgent
    public enum State { Idle, Chasing, Attacking };
    private State _currentState;

    public ParticleSystem deathEffect;

    private NavMeshAgent _pathFinder;
    private Transform _target;
    private LivingEntity _targetEntity;
    private Material _skinMaterial;
    private Color _originalColor;
    public float refreshAfter = 0.5f;
    public float attackDistanceThreshold = .5f;
    private float _timeBetweenAttacks = 1f;
    private float _nextAttackTime = 0f;
    private float _myCollisionRadius;
    private float _targetCollisionRadius;
    private bool _hasTarget;
    public float damage = 1f;

    void Awake()
    {
        _pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            _hasTarget = true;

            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _targetEntity = _target.GetComponent<LivingEntity>();

            _hasTarget = true;

            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (_hasTarget)
        {
            _currentState = State.Chasing;
            _targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        _pathFinder.speed = moveSpeed;

        if (_hasTarget)
        {
            damage = Mathf.Ceil(_targetEntity.startingHealth / hitsToKillPlayer);
        }

        startingHealth = enemyHealth;
        _skinMaterial = GetComponent<Renderer>().material;
        _skinMaterial.color = skinColor;
        _originalColor = _skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDir)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);

        if (damage >= health)
        {
            AudioManager.instance.PlaySound("EnemyDeath", transform.position);
            GameObject deathParticles = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDir)) as GameObject;
            Destroy(deathParticles.gameObject, deathEffect.main.startLifetimeMultiplier);
        }

        base.TakeDamage(damage);
    }

    void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }

    void Update()
    {
        if (_hasTarget)
        {
            if (Time.time > _nextAttackTime)
            {
                float sqDstToTarget = (_target.position - transform.position).sqrMagnitude;

                if (sqDstToTarget < Mathf.Pow(attackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + _timeBetweenAttacks;
                    AudioManager.instance.PlaySound("EnemyAttack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        _currentState = State.Attacking;
        // pathfinder is disabled so the it does not change the target position while leaping
        _pathFinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (_target.position - transform.position).normalized;
        Vector3 attackPosition = _target.position - dirToTarget * (_myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        _skinMaterial.color = Color.red;
        bool hasAppliedDamaged = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamaged)
            {
                hasAppliedDamaged = true;
                _targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        _skinMaterial.color = _originalColor;
        _currentState = State.Chasing;
        // re-enabling the pathfinder
        _pathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        while (_hasTarget)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 dirToTarget = (_target.position - transform.position).normalized;

                Vector3 targetPosition = _target.position - dirToTarget * (_myCollisionRadius + _targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    _pathFinder.SetDestination(targetPosition);
                }
            }

            yield return new WaitForSeconds(refreshAfter);
        }
    }
}
