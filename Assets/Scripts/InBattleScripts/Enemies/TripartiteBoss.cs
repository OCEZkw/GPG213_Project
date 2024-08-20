using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripartiteBoss : MonoBehaviour
{
    public TripartiteBossPart mainBody;
    public TripartiteBossPart magicBall;
    public TripartiteBossPart shield;
    private bool isRageMode = false;
    private int cannonChargeCounter = 0;
    private const int CHARGES_NEEDED_TO_FIRE = 3;
    private WaveManager waveManager;

    public bool IsPerformingActions { get; private set; }

    // New fields for flower prefabs and spawn points
    public GameObject healingFlowerPrefab;
    public GameObject cardLockingFlowerPrefab;
    private bool[] spawnPointsOccupied;

    // Add references to the shield sprites
    public Sprite normalShieldSprite;
    public Sprite cannonShieldSprite;
    private int currentEnemyCode = 1000;

    void Start()
    {
        InitializeBossParts();
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager == null)
        {
            Debug.LogError("WaveManager not found in the scene!");
        }
        spawnPointsOccupied = new bool[waveManager.spawnPoints.Length];
    }

    public void InitializeBossParts()
    {
        mainBody.Initialize(5000, TripartiteBossPart.PartType.MainBody, this);
        magicBall.Initialize(1000, TripartiteBossPart.PartType.MagicBall, this);
        shield.Initialize(300, TripartiteBossPart.PartType.Shield, this);
    }

    public void CheckBossDeath()
    {
        if (mainBody.IsDead && magicBall.IsDead && shield.IsDead)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Tripartite Boss died!");
        if (waveManager != null)
        {
            waveManager.OnEnemyDefeated();
        }
        else
        {
            Debug.LogError("WaveManager is null when trying to notify of boss death!");
        }
        gameObject.SetActive(false);
    }

    // Implement BossAttackAfterPlayerActions similar to WizardBossEnemy
    public void BossAttackAfterPlayerActions(Player player)
    {
        StartCoroutine(PerformBossActions(player));
    }

    private IEnumerator PerformBossActions(Player player)
    {
        IsPerformingActions = true;
        Debug.Log("TripartiteBoss: Starting boss actions");

        if (!mainBody.IsDead)
        {
            Debug.Log("TripartiteBoss: Main Body performing action");
            mainBody.PerformAction();
            yield return new WaitForSeconds(2f);
        }
        else
        {
            Debug.Log("TripartiteBoss: Main Body is dead, skipping action");
        }

        if (!magicBall.IsDead)
        {
            Debug.Log("TripartiteBoss: Magic Ball performing action");
            magicBall.PerformAction();
            yield return new WaitForSeconds(2f);
        }
        else
        {
            Debug.Log("TripartiteBoss: Magic Ball is dead, skipping action");
        }

        if (!shield.IsDead)
        {
            Debug.Log("TripartiteBoss: Shield performing action");
            shield.PerformAction();
            yield return new WaitForSeconds(2f);
        }
        else
        {
            Debug.Log("TripartiteBoss: Shield is dead, skipping action");
        }

        CheckRageMode();
        CheckBossDeath();

        Debug.Log("TripartiteBoss: Finished performing actions");
        IsPerformingActions = false;
    }

    public void CheckRageMode()
    {
        if (!isRageMode && mainBody.currentHealth <= mainBody.maxHealth * 0.2f)
        {
            StartCoroutine(EnterRageModeSequence());
        }
    }

    private IEnumerator EnterRageModeSequence()
    {
        isRageMode = true;
        Debug.Log("TripartiteBoss: Entering Rage Mode!");
        NotificationManager.Instance.ShowNotification("Blossom Queen entering rage mode");

        yield return new WaitForSeconds(2f);  // Wait for 2 seconds

        // Heal Main Body
        int healAmount = Mathf.RoundToInt(mainBody.maxHealth * 0.5f);
        mainBody.Heal(healAmount);
        Debug.Log($"TripartiteBoss: Healed Main Body by {healAmount} HP");
        NotificationManager.Instance.ShowNotification($"Blossom Queen healed for {healAmount} HP");

        yield return new WaitForSeconds(2f);  // Wait for another 2 seconds

        // Revive and transform shield if it's dead, or just transform if it's alive
        if (shield.IsDead)
        {
            ReviveAndTransformShield();
        }
        else
        {
            TransformShieldToCannon();
        }
    }

    void ReviveAndTransformShield()
    {
        shield.gameObject.SetActive(true);
        shield.Revive();
        NotificationManager.Instance.ShowNotification("Shield revived");

        StartCoroutine(DelayedTransformShieldToCannon());
    }

    private IEnumerator DelayedTransformShieldToCannon()
    {
        yield return new WaitForSeconds(2f);  // Wait for 2 seconds
        TransformShieldToCannon();
    }

    void TransformShieldToCannon()
    {
        if (cannonShieldSprite == null)
        {
            Debug.LogError("TripartiteBoss: Cannon shield sprite is not assigned!");
            return;
        }

        shield.TransformIntoCannon(cannonShieldSprite);
        Debug.Log("TripartiteBoss: Shield transformed into Cannon");
        NotificationManager.Instance.ShowNotification("Shield transformed into cannon");
    }


    public void BossAttack()
    {
        mainBody.PerformAction();
        magicBall.PerformAction();
        shield.PerformAction();
    }
    public bool IsInRageMode() => isRageMode;

    public bool IsShieldDead() => shield.IsDead;

    // New method to handle part destruction
    public void OnPartDestroyed(TripartiteBossPart part)
    {
        part.gameObject.SetActive(false);
        Debug.Log($"TripartiteBoss: {part.partType} has been destroyed and despawned.");
    }

    public void RespawnShield()
    {
        shield.gameObject.SetActive(true);
        shield.Heal(shield.maxHealth);
        Debug.Log("TripartiteBoss: Shield has been respawned.");
    }

    public void IncreaseMainBodyDamage(int amount)
    {
        mainBody.attackDamage += amount;
    }

    public void IncrementCannonCharge()
    {
        cannonChargeCounter++;
        int roundsLeft = CHARGES_NEEDED_TO_FIRE - cannonChargeCounter;

        if (roundsLeft > 0)
        {
            NotificationManager.Instance.ShowNotification($"Cannon charging! {roundsLeft} {(roundsLeft == 1 ? "round" : "rounds")} until firing!");
        }

        if (cannonChargeCounter >= CHARGES_NEEDED_TO_FIRE)
        {
            NotificationManager.Instance.ShowNotification("Cannon fully charged! Firing!");
            FireCannon();
            cannonChargeCounter = 0;
        }
    }

    private void FireCannon()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            int damage = player.maxHealth;  // Or whatever damage value you want to use
            player.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("Player not found when trying to fire cannon.");
        }
    }

    // New method to spawn healing flowers
    public void SpawnHealingFlower()
    {
        SpawnFlower(healingFlowerPrefab);
        NotificationManager.Instance.ShowNotification("Magic Crystal spawned healing flower");
    }

    // New method to spawn card-locking flowers
    public void SpawnCardLockingFlower()
    {
        SpawnFlower(cardLockingFlowerPrefab);
        NotificationManager.Instance.ShowNotification("Magic Crystal spawned card-locking flower");
    }

    // Updated method to spawn flowers with enemy codes
    public void SpawnFlower(GameObject flowerPrefab)
    {
        if (waveManager == null || waveManager.spawnPoints == null || waveManager.spawnPoints.Length < 3)
        {
            Debug.LogWarning("WaveManager or spawn points not available!");
            return;
        }

        // Check only spawn points 1 and 2
        int availableSpawnPoint = -1;
        for (int i = 1; i <= 2; i++)
        {
            if (!spawnPointsOccupied[i])
            {
                availableSpawnPoint = i;
                break;
            }
        }

        if (availableSpawnPoint != -1)
        {
            Transform spawnPoint = waveManager.spawnPoints[availableSpawnPoint];
            GameObject flower = Instantiate(flowerPrefab, spawnPoint.position, Quaternion.identity);
            spawnPointsOccupied[availableSpawnPoint] = true;

            FlowerEnemy flowerEnemy = flower.GetComponent<FlowerEnemy>();
            if (flowerEnemy != null)
            {
                flowerEnemy.enemyCode = GenerateUniqueEnemyCode();
                flowerEnemy.SetSpawnPointIndex(availableSpawnPoint);
                Debug.Log($"Spawned {flowerPrefab.name} at spawn point {availableSpawnPoint} with enemy code {flowerEnemy.enemyCode}");
            }
            else
            {
                Debug.LogWarning($"FlowerEnemy component not found on {flowerPrefab.name}");
            }
        }
        else
        {
            Debug.Log("Spawn points 1 and 2 are occupied. No flower spawned.");
        }
    }

    public void OnFlowerDestroyed(int spawnPointIndex)
    {
        if (spawnPointIndex >= 0 && spawnPointIndex < spawnPointsOccupied.Length)
        {
            spawnPointsOccupied[spawnPointIndex] = false;
            Debug.Log($"Spawn point {spawnPointIndex} is now available.");
        }
        else
        {
            Debug.LogWarning($"Invalid spawn point index: {spawnPointIndex}");
        }
    }

    // New method to generate unique enemy codes
    private int GenerateUniqueEnemyCode()
    {
        return currentEnemyCode++;
    }
}
