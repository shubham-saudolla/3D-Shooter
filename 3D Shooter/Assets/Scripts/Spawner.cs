/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public Wave[] waves;
	public Enemy enemy;
	private Wave _currentWave;
	private int _currentWaveNumber;
	private int _enemiesRemainingToSpawn;
	private int _enemiesRemainingAlive;
	private float _nextSpawnTime;

	void Start()
	{
		NextWave();
	}

	void Update()
	{
		if(_enemiesRemainingToSpawn > 0 && Time.time > _nextSpawnTime)
		{
			_enemiesRemainingToSpawn--;
			_nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

			Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
			spawnedEnemy.onDeath += OnEnemyDeath;
		}
	}

	void OnEnemyDeath()
	{
		// print("Enemy died");
		_enemiesRemainingAlive--;

		if(_enemiesRemainingAlive == 0)
		{
			NextWave();
		}
	}

	void NextWave()
	{
		_currentWaveNumber++;
		
		if(_currentWaveNumber - 1 < waves.Length)
		{
			print("Wave " + _currentWaveNumber);
			_currentWave = waves[_currentWaveNumber - 1];

			_enemiesRemainingToSpawn = _currentWave.enemyCount;
			_enemiesRemainingAlive = _enemiesRemainingToSpawn;
		}
	}

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float timeBetweenSpawns;
	}
}
