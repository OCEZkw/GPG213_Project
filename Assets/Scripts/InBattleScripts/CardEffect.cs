using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardEffect : MonoBehaviour
{
    public CardEffectType effectType;
    public int effectValue;
    public int cost; // Add cost property
    public CardType cardType;

    public TextMeshProUGUI strengthText;

    private void Awake()
    {
        // Initialize the TextMeshProUGUI component (assumes it's attached to the same GameObject or can be found in children)
        strengthText = GetComponentInChildren<TextMeshProUGUI>();

        // Update the strength text at the start
        UpdateStrengthText();
    }

    private void Start()
    {
        // If the TextMeshProUGUI component is not directly a child, navigate through the hierarchy to find it
        if (strengthText == null)
        {
            Transform childTransform = transform.Find("ChildName/GrandChildName"); // Adjust the path as necessary
            if (childTransform != null)
            {
                strengthText = childTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        // Update the strength text if found
        UpdateStrengthText();

        // Ensure the text component's width does not change unexpectedly
        FixTextWidth();
    }

    private void FixTextWidth()
    {
        if (strengthText != null)
        {
            // Ensure Auto Size is disabled
            strengthText.enableAutoSizing = false;

            // Set the RectTransform width to a fixed value
            RectTransform rectTransform = strengthText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150); // Set to your desired width
            }
        }
    }

    public void ApplyEffect(GameObject target)
    {
        if (effectType == CardEffectType.Healing || effectType == CardEffectType.Defense)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                ApplyEffectToPlayer(player);
            }
        }
        else
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyEffectToEnemy(enemy);
            }
        }
    }

    private void ApplyEffectToPlayer(Player player)
    {
        switch (effectType)
        {
            case CardEffectType.Healing:
                player.Heal(effectValue);
                break;
            case CardEffectType.Defense:
                player.IncreasePhysicalDefense(effectValue);
                break;
        }
    }

    private void ApplyEffectToEnemy(Enemy enemy)
    {
        int modifiedEffectValue = ModifyEffectByTyping(effectValue, cardType, enemy.enemyType);

        switch (effectType)
        {
            case CardEffectType.AttackDamage:
                enemy.TakeDamage(modifiedEffectValue);
                break;
            case CardEffectType.MagicAttackDamage:
                enemy.TakeMagicDamage(modifiedEffectValue);
                break;
            case CardEffectType.MagicDefense:
                enemy.IncreaseMagicDefense(effectValue);
                break;
        }
    }

    private int ModifyEffectByTyping(int baseEffectValue, CardType cardType, EnemyType enemyType)
    {
        float modifier = 1.0f;

        if (cardType == CardType.Fire && enemyType == EnemyType.Grass ||
            cardType == CardType.Grass && enemyType == EnemyType.Water ||
            cardType == CardType.Water && enemyType == EnemyType.Fire)
        {
            modifier = 2.0f; // Effective damage
        }
        else if (cardType == CardType.Fire && enemyType == EnemyType.Water ||
                 cardType == CardType.Grass && enemyType == EnemyType.Fire ||
                 cardType == CardType.Water && enemyType == EnemyType.Grass)
        {
            modifier = 0.5f; // Not effective damage
        }
        else if ((cardType == CardType.Light && enemyType == EnemyType.Dark) ||
                 (cardType == CardType.Dark && enemyType == EnemyType.Light))
        {
            modifier = 2.0f; // Light and Dark counter each other
        }

        return Mathf.RoundToInt(baseEffectValue * modifier);
    }

    public bool IsDefenseOrHealCard()
    {
        return effectType == CardEffectType.Healing || effectType == CardEffectType.Defense;
    }

    private void UpdateStrengthText()
    {
        if (strengthText != null)
        {
            strengthText.text = effectValue.ToString();
        }
    }

    // Example method to change the effect value and update the text
    public void SetEffectValue(int newValue)
    {
        effectValue = newValue;
        UpdateStrengthText();
    }
}