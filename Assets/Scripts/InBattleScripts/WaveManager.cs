using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemies;
        public float spawnInterval;
        public bool isBossWave; // Add a flag to indicate a boss wave
        public GameObject bossPrefab;
    }

    public List<Wave> waves; // List of waves
    public Transform[] spawnPoints; // Enemy spawn points
    public GameObject enemyPrefab; // Enemy prefab

    private int currentWaveIndex = 0;
    private int enemiesRemainingToSpawn;
    public int enemiesRemainingAlive;
    private float nextSpawnTime;
    private EnemySpawner enemySpawner;

    private int currentEnemyCode = 0;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

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
            if (waves[currentWaveIndex].isBossWave)
            {
                // Spawn boss only if it's a boss wave
                Debug.Log("Spawning boss for wave " + currentWaveIndex);
                SpawnBoss(waves[currentWaveIndex].bossPrefab);
                enemiesRemainingAlive = 1; // Set enemiesRemainingAlive for boss wave
            }
            else
            {
                enemiesRemainingToSpawn = waves[currentWaveIndex].numberOfEnemies;
                enemiesRemainingAlive = enemiesRemainingToSpawn;
                nextSpawnTime = Time.time;
            }
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

                // Generate a unique enemy code (you can use a counter or random number generator)
                int uniqueCode = GenerateUniqueEnemyCode();

                // Access the Enemy script and set the enemyCode
                Enemy enemy = enemyInstance.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.enemyCode = uniqueCode;
                }

                // Add the enemy instance to your management systems
                enemySpawner.AddEnemyInstance(enemyInstance);

                enemiesRemainingToSpawn--;
            }
            else
            {
                Debug.LogWarning("Not enough spawn points for the remaining enemies.");
            }
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        if (spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[0]; // Use the first spawn point for the boss
            GameObject bossInstance = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

            // Get the WizardBossEnemy component from the instantiated bossInstance
            WizardBossEnemy boss = bossInstance.GetComponent<WizardBossEnemy>();
            if (boss != null)
            {
                // Get the BossPart components from the instantiated boss instance
                BossPart[] bossParts = bossInstance.GetComponentsInChildren<BossPart>();

                if (bossParts.Length < 3)
                {
                    Debug.LogError("Not enough BossPart components found on the WizardBossEnemy prefab.");
                    return;
                }

                // Initialize and assign unique codes to each part
                int staffCode = GenerateUniqueEnemyCode();
                int headCode = GenerateUniqueEnemyCode();
                int leftHandCode = GenerateUniqueEnemyCode();

                // Assign codes to each part
                boss.AssignUniqueCodes(bossParts[0], bossParts[1], bossParts[2], staffCode, headCode, leftHandCode);
            }

            enemiesRemainingAlive = 1; // Assume the boss is the only enemy in the wave
        }
    }

    int GenerateUniqueEnemyCode()
    {
        // Example: You can use a simple counter for generating unique codes
        return ++currentEnemyCode;
    }

    public void OnEnemyDefeated()
    {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive <= 0)
        {
            if (currentWaveIndex + 1 < waves.Count)
            {
                currentWaveIndex++;
                StartCoroutine(StartNextWaveWithDelay(1f));
            }
            else
            {
                StartCoroutine(ReturnToMainMenuWithDelay(3f));
            }
        }
    }

    IEnumerator StartNextWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNextWave();
    }

    IEnumerator ReturnToMainMenuWithDelay(float delay)
    {
        Debug.Log("All waves completed! Returning to main menu...");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
