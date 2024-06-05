using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Reference to the enemy prefab
    public Transform[] enemySpawnPoint; // Reference to the enemy spawn point

    private GameObject enemyInstance;  // Instance of the enemy

    void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab != null && enemySpawnPoint != null && enemyInstance == null)
        {
            enemyInstance = Instantiate(enemyPrefab, enemySpawnPoint[0].position, Quaternion.identity);
        }
    }

    public GameObject GetEnemyInstance()
    {
        return enemyInstance;
    }
}
