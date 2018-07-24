/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: the player can hide in a corner and kill all the enemies
// one fix could be that if the player hasn't moved in a while, the enemies will spawn at the player's location

public class Spawner : MonoBehaviour
{
	public Wave[] waves;
	public Enemy enemy;
	private Wave _currentWave;
	private int _currentWaveNumber;
	private int _enemiesRemainingToSpawn;
	private int _enemiesRemainingAlive;
	private float _nextSpawnTime;

	MapGenerator map;

	void Start()
	{
		map = FindObjectOfType<MapGenerator>();
		NextWave();
	}

	void Update()
	{
		if(_enemiesRemainingToSpawn > 0 && Time.time > _nextSpawnTime)
		{
			_enemiesRemainingToSpawn--;
			_nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

			StartCoroutine(SpawnEnemy());
		}
	}

	IEnumerator SpawnEnemy()
	{
		float spawnDelay = 1f;
		float tileFlashSpeed = 4f;

		Transform randomTile = map.getRandomOpenTile();
		Material tileMat = randomTile.GetComponent<Renderer>().material;
		Color initialColor = tileMat.color;
		Color flashColor = Color.red;
		float spawnTimer = 0f;

		while(spawnTimer < spawnDelay)
		{
			tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.onDeath += OnEnemyDeath;
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
