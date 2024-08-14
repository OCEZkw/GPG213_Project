using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // This line adds the Slider type
using TMPro; // This is for TextMeshPro, which we use for the damage text

public class TripartiteBossPart : BossPart
{
    public enum PartType
    {
        MainBody,
        MagicBall,
        Shield
    }

    public PartType partType;
    private TripartiteBoss parentBoss;
    private bool isCannonMode = false;

    private SpriteRenderer spriteRenderer;

    public void Initialize(int health, PartType type, TripartiteBoss boss)
    {
        base.Initialize(health, (EnemyType)type);
        partType = type;
        parentBoss = boss;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"TripartiteBossPart: SpriteRenderer not found on {gameObject.name}");
        }
    }

    // Override TakeDamage to include the shield logic
    public override void TakeDamage(int damage, bool isMagic)
    {
        int actualDamage = isMagic ?
            Mathf.Max(damage * 100 / (100 + magicDefense), 0) :
            Mathf.Max(damage * 100 / (100 + defense), 0);

        if (partType == PartType.MainBody && !parentBoss.IsShieldDead())
        {
            actualDamage = Mathf.RoundToInt(actualDamage * 0.1f); // 90% damage reduction
        }

        base.TakeDamage(actualDamage, isMagic);
    }

    // Override Die to notify the parent boss
    protected override void Die()
    {
        base.Die();
        parentBoss.OnPartDestroyed(this);
        parentBoss.CheckBossDeath();
    }

    // Override Heal to handle respawning if necessary
    public override void Heal(int amount)
    {
        bool wasDeadBefore = IsDead;
        base.Heal(amount);

        if (wasDeadBefore && !IsDead)
        {
            gameObject.SetActive(true);
            Debug.Log($"TripartiteBossPart: {partType} has been revived and respawned.");
        }
    }

    public void PerformAction()
    {
        switch (partType)
        {
            case PartType.MainBody:
                PerformMainBodyAction();
                break;
            case PartType.MagicBall:
                PerformMagicBallAction();
                break;
            case PartType.Shield:
                PerformShieldAction();
                break;
        }
    }
    private void PerformMainBodyAction()
    {
        // Attack the player
        Debug.Log($"TripartiteBossPart: Main Body attacking player for {attackDamage} damage");
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.TakeDamage(attackDamage);
        }
    }
    private void PerformMagicBallAction()
    {
        if (parentBoss.IsInRageMode())
        {
            Debug.Log("TripartiteBossPart: Magic Ball spawning card-locking flowers");
            parentBoss.SpawnCardLockingFlower();
        }
        else
        {
            int randomAction = Random.Range(0, 3);
            switch (randomAction)
            {
                case 0:
                    parentBoss.SpawnHealingFlower();
                    Debug.Log("TripartiteBossPart: Magic Ball spawning healing flowers");
                    break;
                case 1:
                    if (parentBoss.IsShieldDead())
                        parentBoss.RespawnShield();
                    Debug.Log("TripartiteBossPart: Magic Ball respawning Shield");
                    break;
                case 2:
                    Debug.Log("TripartiteBossPart: Magic Ball increasing Main Body damage by 50");
                    parentBoss.IncreaseMainBodyDamage(50);
                    break;
            }
        }
    }

    private void PerformShieldAction()
    {
        if (parentBoss.IsInRageMode())
        {
            Debug.Log("TripartiteBossPart: Shield (Cannon) charging attack");
            ChargeCannonAttack();
        }
        else
        {
            Debug.Log("TripartiteBossPart: Shield decreasing player defenses");
            DecreasePlayerDefenses();
        }
    }

    private void SpawnHealingFlowers()
    {
        // Implement spawning of healing flowers
        parentBoss.SpawnHealingFlower();
        Debug.Log("TripartiteBossPart: Spawning healing flowers (implement this functionality)");
    }

    private void SpawnCardLockingFlowers()
    {
        // Implement spawning of card-locking flowers
        parentBoss.SpawnCardLockingFlower();
        Debug.Log("TripartiteBossPart: Spawning card-locking flowers (implement this functionality)");
    }

    private void ChargeCannonAttack()
    {
        Debug.Log("TripartiteBossPart: Charging cannon attack");
        parentBoss.IncrementCannonCharge();
    }

    private void DecreasePlayerDefenses()
    {
        Debug.Log("TripartiteBossPart: Decreasing player defenses by 10");
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.DecreaseDefenses(10);
        }
        else
        {
            Debug.LogWarning("TripartiteBossPart: Player not found for decreasing defenses");
        }
    }

    public void TransformIntoCannon(Sprite cannonSprite)
    {
        isCannonMode = true;
        if (spriteRenderer != null && cannonSprite != null)
        {
            spriteRenderer.sprite = cannonSprite;
            Debug.Log($"TripartiteBossPart: Changed sprite to cannon for {gameObject.name}");
        }
        else
        {
            if (spriteRenderer == null)
                Debug.LogError($"TripartiteBossPart: SpriteRenderer is null on {gameObject.name}");
            if (cannonSprite == null)
                Debug.LogError("TripartiteBossPart: Cannon sprite is null");
        }
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        gameObject.SetActive(true);
        Debug.Log($"TripartiteBossPart: {partType} has been revived with {currentHealth} HP.");
    }
}


