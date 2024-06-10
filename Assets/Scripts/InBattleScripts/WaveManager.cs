using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public float spawnInterval;
    }

    public List<Wave> waves; // List of waves
    public Transform[] spawnPoints; // Enemy spawn points
    public GameObject enemyPrefab; // Enemy prefab

    private int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    private EnemySpawner enemySpawner;

    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        StartNextWave();
    }

    void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + waves[currentWaveIndex].spawnInterval;
        }
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            enemiesRemainingToSpawn = waves[currentWaveIndex].numberOfEnemies;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            nextSpawnTime = Time.time;
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    void SpawnEnemy()
    {
        if (enemiesRemainingToSpawn > 0)
        {
            int spawnIndex = enemiesRemainingToSpawn - 1;
            if (spawnIndex < spawnPoints.Length)
            {
                Transform spawnPoint = spawnPoints[spawnIndex];
                GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                enemySpawner.AddEnemyInstance(enemyInstance);
                enemiesRemainingToSpawn--;
            }
            else
            {
                Debug.LogWarning("Not enough spawn points for the remaining enemies.");
            }
        }
    }

    public void OnEnemyDefeated()
    {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive <= 0)
        {
            currentWaveIndex++;
            StartCoroutine(StartNextWaveWithDelay(3f));
        }
    }

    IEnumerator StartNextWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }
}
