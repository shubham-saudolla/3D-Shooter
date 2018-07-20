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

	public enum State{Idle, Chasing, Attacking};
	private State _currentState;

	private NavMeshAgent _pathFinder;
	private Transform _target;
	Material skinMaterial;
	Color originalColor;
	public float refreshAfter = 0.5f;

	public float attackDistanceThreshold = .5f;
	float timeBetweenAttacks = 1f;
	float nextAttackTime = 0f;
	float myCollisionRadius;
	float targetCollisionRadius;

	protected override void Start()
	{
		base.Start();
		_pathFinder = GetComponent<NavMeshAgent>();
		skinMaterial = GetComponent<Renderer>().material;
		originalColor = skinMaterial.color;

		_currentState = State.Chasing;
		_target = GameObject.FindGameObjectWithTag("Player").transform;

		myCollisionRadius = GetComponent<CapsuleCollider>().radius;
		targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

		StartCoroutine(UpdatePath());
	}
	
	void Update()
	{
		if(Time.time > nextAttackTime)
		{
			float sqDstToTarget = (_target.position - transform.position).sqrMagnitude;

			if(sqDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
			{
				nextAttackTime = Time.time + timeBetweenAttacks;
				StartCoroutine(Attack());
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
		Vector3 attackPosition = _target.position - dirToTarget * (myCollisionRadius);

		float percent = 0;
		float attackSpeed = 3;

		skinMaterial.color = Color.red;

		while(percent <= 1)
		{
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
			transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

			yield return null;
		}

		skinMaterial.color = originalColor;
		_currentState = State.Chasing;
		// re-enabling the pathfinder
		_pathFinder.enabled = true;
	}

	IEnumerator UpdatePath()
	{
		while(_target != null)
		{
			if(_currentState == State.Chasing)
			{
				Vector3 dirToTarget = (_target.position - transform.position).normalized;

				Vector3 targetPosition = _target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
				if(!dead)
				{
					_pathFinder.SetDestination(targetPosition);
				}
				
			}

			yield return new WaitForSeconds(refreshAfter);
		}
	}
}
