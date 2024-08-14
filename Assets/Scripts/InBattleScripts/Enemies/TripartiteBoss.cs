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
    private WaveManager waveManager;

    public bool IsPerformingActions { get; private set; }

    // New fields for flower prefabs and spawn points
    public GameObject healingFlowerPrefab;
    public GameObject cardLockingFlowerPrefab;
    private bool[] spawnPointsOccupied;

    // Add references to the shield sprites
    public Sprite normalShieldSprite;
    public Sprite cannonShieldSprite;

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
            Debug.Log("TripartiteBoss: Entering Rage Mode!");
            EnterRageMode();
        }
    }

    void EnterRageMode()
    {
        isRageMode = true;
        Debug.Log("TripartiteBoss: Entering Rage Mode!");

        // Heal Main Body
        int healAmount = Mathf.RoundToInt(mainBody.maxHealth * 0.5f);
        mainBody.Heal(healAmount);
        Debug.Log($"TripartiteBoss: Healed Main Body by {healAmount} HP");

        // Revive and transform shield if it's dead
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
        Debug.Log("TripartiteBoss: Attempting to transform Shield into Cannon");
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
        if (cannonChargeCounter >= 3)
        {
            FireCannon();
            cannonChargeCounter = 0;
        }
    }
    private void FireCannon()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.TakeDamage(player.maxHealth);
        }
    }

    // New method to spawn healing flowers
    public void SpawnHealingFlower()
    {
        SpawnFlower(healingFlowerPrefab);
    }

    // New method to spawn card-locking flowers
    public void SpawnCardLockingFlower()
    {
        SpawnFlower(cardLockingFlowerPrefab);
    }

    // Updated helper method to spawn flowers using WaveManager's spawn points
    private void SpawnFlower(GameObject flowerPrefab)
    {
        if (waveManager == null || waveManager.spawnPoints == null || waveManager.spawnPoints.Length == 0)
        {
            Debug.LogWarning("WaveManager or spawn points not available!");
            return;
        }

        // Find an unoccupied spawn point
        int availableSpawnPoint = -1;
        for (int i = 0; i < spawnPointsOccupied.Length; i++)
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
            Debug.Log($"Spawned {flowerPrefab.name} at spawn point {availableSpawnPoint}");
        }
        else
        {
            Debug.Log("All spawn points are occupied. Using alternative ability.");
        }
    }
}
