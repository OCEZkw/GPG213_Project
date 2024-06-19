using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public CardEffectType effectType;
    public int effectValue;
    public int cost; // Add cost property
    public CardType cardType;

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
}