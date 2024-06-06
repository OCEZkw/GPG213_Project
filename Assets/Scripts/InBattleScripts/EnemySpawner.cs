using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;  // Reference to the enemy prefab
    public Transform[] enemySpawnPoints; // Reference to the enemy spawn points

    private List<GameObject> enemyInstances = new List<GameObject>();  // List of enemy instances

    void Start()
    {
        // SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab != null && enemySpawnPoints != null )
        {
            for (int i = 0; i < enemySpawnPoints.Length; i++)
            {
                GameObject enemyInstance = Instantiate(enemyPrefab, enemySpawnPoints[i].position, Quaternion.identity);
                enemyInstances.Add(enemyInstance);
            }
        }
    }

    public List<GameObject> GetEnemyInstances()
    {
        return enemyInstances;
    }
}
