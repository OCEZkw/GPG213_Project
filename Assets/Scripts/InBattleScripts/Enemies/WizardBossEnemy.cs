using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WizardBossEnemy : MonoBehaviour
{
    public GameObject staff;
    public GameObject head;
    public GameObject leftHand;

    private BossPart staffPart;
    private BossPart headPart;
    private BossPart leftHandPart;

    public EnemyType staffType;
    public EnemyType headType;
    public EnemyType leftHandType;

    private int currentEnemyCode = 0;

    private bool isBossAttacking = false;
    public bool IsBossAttacking { get { return isBossAttacking; } }

    void Start()
    {
        InitializeBossParts(); // Ensure parts are initialized first
    }

    public void AssignUniqueCodes(BossPart staffPart, BossPart headPart, BossPart leftHandPart, int staffCode, int headCode, int leftHandCode)
    {
        // Assign unique codes to each boss part
        staffPart.enemyCode = staffCode;
        headPart.enemyCode = headCode;
        leftHandPart.enemyCode = leftHandCode;
    }

    void InitializeBossParts()
    {
        staffPart = staff.GetComponent<BossPart>();
        headPart = head.GetComponent<BossPart>();
        leftHandPart = leftHand.GetComponent<BossPart>();

        if (staffPart == null)
            Debug.LogError("Staff part not found or BossPart component not attached.");
        if (headPart == null)
            Debug.LogError("Head part not found or BossPart component not attached.");
        if (leftHandPart == null)
            Debug.LogError("Left hand part not found or BossPart component not attached.");

        // Initialize the health for each part if they are not null
        if (staffPart != null)
            staffPart.Initialize(3000, staffType);
        if (headPart != null)
            headPart.Initialize(5000, headType);
        if (leftHandPart != null)
            leftHandPart.Initialize(2500, leftHandType);
    }

    public void PartDamaged(GameObject part, int damage, bool isMagic)
    {
        if (part == staff)
        {
            staffPart.TakeDamage(damage, isMagic);
        }
        else if (part == head)
        {
            headPart.TakeDamage(damage, isMagic);
        }
        else if (part == leftHand)
        {
            leftHandPart.TakeDamage(damage, isMagic);
        }

        CheckBossDeath();
    }

    void CheckBossDeath()
    {
        if (staffPart.IsDead && headPart.IsDead && leftHandPart.IsDead)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Wizard Boss died!");
        // Implement additional logic for boss death, like rewards or ending the game.
    }

    // Method to handle boss attacking after player actions
    public void BossAttackAfterPlayerActions(Player player)
    {
        StartCoroutine(PerformBossActions(player));
    }

    private IEnumerator PerformBossActions(Player player)
    {
        // Example: Perform actions for each boss part
        if (headPart != null && !headPart.IsDead)
        {
            headPart.IncreaseMagicDamage(200); // Example: Increase head's magic damage
            isBossAttacking = true;
            yield return new WaitForSeconds(2f); // Wait for 1 second after this action
        }

        if (staffPart != null && !staffPart.IsDead)
        {
            int magicDamage = staffPart.magicDamage; // Example: Get staff's magic damage
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null && playerComponent.gameObject.activeInHierarchy)
            {
                playerComponent.TakeMagicDamage(magicDamage);
                isBossAttacking = true;
                Debug.Log($"Staff attacked player for {magicDamage} magical damage.");
                yield return new WaitForSeconds(2f); // Wait for 1 second after this action
            }
        }

        if (leftHandPart != null && !leftHandPart.IsDead)
        {
            int attackDamage = leftHandPart.attackDamage; // Example: Get left hand's attack damage
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null && playerComponent.gameObject.activeInHierarchy)
            {
                playerComponent.TakeDamage(attackDamage);
                isBossAttacking = true;
                Debug.Log($"Left hand attacked player for {attackDamage} physical damage.");
                yield return new WaitForSeconds(2f); // Wait for 1 second after this action
            }
        }
        isBossAttacking = false;
        // After all actions are performed, check if boss needs to do something else
        CheckBossDeath();
        // You can add additional logic here for the boss's turn after player actions
        yield return null;
    }
}

