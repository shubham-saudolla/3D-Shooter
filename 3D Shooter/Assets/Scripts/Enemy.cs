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
	private NavMeshAgent _pathFinder;
	private Transform _target;
	public float refreshAfter = 0.5f;

	protected override void Start()
	{
		base.Start();
		_pathFinder = GetComponent<NavMeshAgent>();
		_target = GameObject.FindGameObjectWithTag("Player").transform;

		StartCoroutine(UpdatePath());
	}
	
	void Update()
	{
		
	}

	IEnumerator UpdatePath()
	{
		while(_target != null)
		{
			Vector3 targetPosition = new Vector3(_target.position.x, 0, _target.position.z);
			if(!dead)
			{
				_pathFinder.SetDestination(targetPosition);
			}
			
			yield return new WaitForSeconds(refreshAfter);
		}
	}
}
