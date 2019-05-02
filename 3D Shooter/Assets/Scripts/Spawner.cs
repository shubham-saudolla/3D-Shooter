/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public bool developerMode;

    public event System.Action<int> OnNewWave;
    public Wave[] waves;
    public Enemy enemy;

    private LivingEntity playerEntity;
    private GameObject playerT;

    private Wave _currentWave;
    private int _currentWaveNumber;
    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;
    private float _nextSpawnTime;

    MapGenerator map;

    private float timeBetweenCampingChecks = 2f;
    private float campThresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping;

    bool isDisabled;

    void Start()
    {
        isDisabled = false;

        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.gameObject;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.transform.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();

        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.transform.position, campPositionOld)) < campThresholdDistance;

                campPositionOld = playerT.transform.position;
            }


            if ((_enemiesRemainingToSpawn > 0 || _currentWave.infinite) && Time.time > _nextSpawnTime)
            {
                _enemiesRemainingToSpawn--;
                _nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }

        // if (Input.GetKeyDown(KeyCode.Return))               // developer mode
        // {
        //     StopCoroutine("SpawnEnemy");
        //     foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        //     {
        //         Destroy(enemy.gameObject);
        //     }

        //     NextWave();
        // }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4f;

        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.transform.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0f;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.GetComponent<NavMeshAgent>().speed = Random.Range(_currentWave.minSpeed, _currentWave.maxSpeed);
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(_currentWave.moveSpeed, _currentWave.hitsToKillPlayer, _currentWave.enemyHealth, _currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        // print("Enemy died");
        _enemiesRemainingAlive--;

        if (_enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.transform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    void NextWave()
    {
        if (_currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("LevelCompleted");
        }
        _currentWaveNumber++;

        if (_currentWaveNumber - 1 < waves.Length)
        {
            _currentWave = waves[_currentWaveNumber - 1];

            _enemiesRemainingToSpawn = _currentWave.enemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(_currentWaveNumber);
            }

            ResetPlayerPosition();
            playerT.GetComponent<Player>().moveSpeed += 0.5f;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public float minSpeed;
        public float maxSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
