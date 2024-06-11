using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public CardEffectType effectType;
    public int effectValue;
    public int cost; // Add cost property

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
                player.IncreaseDefense(effectValue);
                break;
        }
    }

    private void ApplyEffectToEnemy(Enemy enemy)
    {
        switch (effectType)
        {
            case CardEffectType.AttackDamage:
                enemy.TakeDamage(effectValue);
                break;
            case CardEffectType.MagicAttackDamage:
                enemy.TakeMagicDamage(effectValue);
                break;
            case CardEffectType.MagicDefense:
                enemy.IncreaseMagicDefense(effectValue);
                break;
        }
    }
}