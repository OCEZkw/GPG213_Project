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
    private EnemyAnimator enemyAnimator;

    public void Initialize(int health, PartType type, TripartiteBoss boss)
    {
        base.Initialize(health, (EnemyType)type);
        partType = type;
        parentBoss = boss;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<EnemyAnimator>();
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
        enemyAnimator?.PlayAttackAnimation();

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
            player.TakeMagicDamage(magicDamage);
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
            List<int> availableActions = new List<int> { 0, 1, 2 };

            while (availableActions.Count > 0)
            {
                int randomIndex = Random.Range(0, availableActions.Count);
                int chosenAction = availableActions[randomIndex];

                switch (chosenAction)
                {
                    case 0:
                        parentBoss.SpawnHealingFlower();
                        Debug.Log("TripartiteBossPart: Magic Ball spawning healing flowers");
                        return;
                    case 1:
                        if (parentBoss.IsShieldDead())
                        {
                            parentBoss.RespawnShield();
                            NotificationManager.Instance.ShowNotification("Magic Crystal revived shield");
                            Debug.Log("TripartiteBossPart: Magic Ball respawning Shield");
                            return;
                        }
                        else
                        {
                            availableActions.Remove(1);
                            continue;
                        }
                    case 2:
                        DealMagicDamageToPlayer();
                        return;
                }
            }

            // If we've exhausted all options (which shouldn't happen), default to dealing damage
            Debug.LogWarning("TripartiteBossPart: Exhausted all action options, defaulting to magic damage");
            DealMagicDamageToPlayer();
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
            NotificationManager.Instance.ShowNotification("Shield decreasing player defenses");
            DecreasePlayerDefenses();
        }
    }

    private void DealMagicDamageToPlayer()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.TakeMagicDamage(magicDamage);
            Debug.Log($"TripartiteBossPart: Magic Ball dealing {magicDamage} magic damage to player");
        }
        else
        {
            Debug.LogWarning("TripartiteBossPart: Player not found for dealing magic damage");
        }
    }

    private void SpawnHealingFlowers()
    {
        // Implement spawning of healing flowers
        parentBoss.SpawnHealingFlower();
        NotificationManager.Instance.ShowNotification("Magic Crystal spawned healing flower");
        Debug.Log("TripartiteBossPart: Spawning healing flowers (implement this functionality)");
    }

    private void SpawnCardLockingFlowers()
    {
        // Implement spawning of card-locking flowers
        parentBoss.SpawnCardLockingFlower();
        NotificationManager.Instance.ShowNotification("Magic Crystal spawned card-locking flower");
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
            NotificationManager.Instance.ShowNotification("Shield transformed into cannon");
            Debug.Log($"TripartiteBossPart: Changed sprite to cannon for {gameObject.name}");
        }
        else
        {
            if (spriteRenderer == null)
                Debug.LogError($"TripartiteBossPart: SpriteRenderer is null on {gameObject.name}");
            if (cannonSprite == null)
                Debug.LogError("TripartiteBossPart: Cannon sprite is null");
        }

        StartCoroutine(UpdateHealthSlider(currentHealth, currentHealth));
    }

    public void Revive()
    {
        currentHealth = maxHealth;
        gameObject.SetActive(true);
        Debug.Log($"TripartiteBossPart: {partType} has been revived with {currentHealth} HP.");
    }
}


