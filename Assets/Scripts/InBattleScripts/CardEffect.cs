using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardEffect : MonoBehaviour
{
    public CardEffectType effectType;
    public int baseEffectValue;
    public int cost;
    public CardType cardType;

    public TextMeshProUGUI strengthText;

    private void Awake()
    {
        strengthText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateStrengthText();
    }

    private void Start()
    {
        if (strengthText == null)
        {
            Transform childTransform = transform.Find("ChildName/GrandChildName");
            if (childTransform != null)
            {
                strengthText = childTransform.GetComponent<TextMeshProUGUI>();
            }
        }
        UpdateStrengthText();
        FixTextWidth();
    }

    private void FixTextWidth()
    {
        if (strengthText != null)
        {
            strengthText.enableAutoSizing = false;
            RectTransform rectTransform = strengthText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
            }
        }
    }

    public void ApplyEffect(GameObject target)
    {
        int effectValue = CalculateEffectValue();

        if (effectType == CardEffectType.Healing || effectType == CardEffectType.Defense)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                ApplyEffectToPlayer(player, effectValue);
            }
        }
        else if (target.CompareTag("BossPart"))
        {
            BossPart bossPart = target.GetComponent<BossPart>();
            if (bossPart != null)
            {
                ApplyEffectToBossPart(bossPart, effectValue);
            }
        }
        else if (target.CompareTag("FlowerEnemy"))
        {
            FlowerEnemy flowerEnemy = target.GetComponent<FlowerEnemy>();
            if (flowerEnemy != null)
            {
                ApplyEffectToFlowerEnemy(flowerEnemy, effectValue);
            }
        }
        else
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyEffectToEnemy(enemy, effectValue);
            }
        }
    }

    private int CalculateEffectValue()
    {
        PlayerStats playerStats = PlayerStats.Instance;
        int totalEffectValue = baseEffectValue;

        switch (effectType)
        {
            case CardEffectType.AttackDamage:
                totalEffectValue += playerStats.attackDamage;
                break;
            case CardEffectType.MagicAttackDamage:
                totalEffectValue += playerStats.magicDamage;
                break;
            case CardEffectType.Healing:
                totalEffectValue += playerStats.healingAmount;
                break;
            case CardEffectType.Defense:
                totalEffectValue += playerStats.defense;
                break;
            case CardEffectType.MagicDefense:
                totalEffectValue += playerStats.magicDefense;
                break;
        }

        return totalEffectValue;
    }

    private void ApplyEffectToPlayer(Player player, int effectValue)
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

    private void ApplyEffectToEnemy(Enemy enemy, int effectValue)
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
                enemy.IncreaseMagicDefense(modifiedEffectValue);
                break;
        }
    }

    private void ApplyEffectToBossPart(BossPart bossPart, int effectValue)
    {
        int modifiedEffectValue = ModifyEffectByTyping(effectValue, cardType, bossPart.bossPartType);

        switch (effectType)
        {
            case CardEffectType.AttackDamage:
                bossPart.TakeDamage(modifiedEffectValue, false);
                break;
            case CardEffectType.MagicAttackDamage:
                bossPart.TakeDamage(modifiedEffectValue, true);
                break;
        }
    }

    private void ApplyEffectToFlowerEnemy(FlowerEnemy flowerEnemy, int effectValue)
    {
        int modifiedEffectValue = ModifyEffectByTyping(effectValue, cardType, flowerEnemy.enemyType);

        switch (effectType)
        {
            case CardEffectType.AttackDamage:
            case CardEffectType.MagicAttackDamage:
                flowerEnemy.TakeDamage(modifiedEffectValue, cardType);
                break;
                // Add other effect types if necessary for FlowerEnemy
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
            int totalEffectValue = CalculateEffectValue();
            strengthText.text = totalEffectValue.ToString();
        }
    }

    // Example method to change the effect value and update the text
    public void SetEffectValue(int newValue)
    {
        baseEffectValue = newValue;
        UpdateStrengthText();
    }
}