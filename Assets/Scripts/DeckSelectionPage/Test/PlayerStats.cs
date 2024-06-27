using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 1000;
    public int attackDamage = 50;
    public int magicDamage = 50;
    public int defense = 50;
    public int magicDefense = 50;
    public int healingAmount = 50;

    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI attackDamageText;
    public TextMeshProUGUI magicDamageText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI magicDefenseText;
    public TextMeshProUGUI healingAmountText;


    void Start()
    {
        UpdateStatTexts();
    }

    public void UpdateStatTexts()
    {
        maxHealthText.text = "Max Health: " + maxHealth.ToString();
        attackDamageText.text = "Attack Damage: " + attackDamage.ToString();
        magicDamageText.text = "Magic Damage: " + magicDamage.ToString();
        defenseText.text = "Defense: " + defense.ToString();
        magicDefenseText.text = "Magic Defense: " + magicDefense.ToString();
        healingAmountText.text = "Healing Amount: " + healingAmount.ToString();
    }

    public void ChangeStat(StatToChange stat, int amount)
    {
        switch (stat)
        {
            case StatToChange.health:
                maxHealth += amount;
                break;
            case StatToChange.attackDamage:
                attackDamage += amount;
                break;
            case StatToChange.magicDamage:
                magicDamage += amount;
                break;
            case StatToChange.defense:
                defense += amount;
                break;
            case StatToChange.magicDefense:
                magicDefense += amount;
                break;
            case StatToChange.healingAmount:
                healingAmount += amount;
                break;
        }
        UpdateStatTexts();
    }
    public void ReverseStatChange(StatToChange stat, int amount)
    {
        ChangeStat(stat, -amount);
        UpdateStatTexts();
    }

    public enum StatToChange
    {
        health,
        attackDamage,
        magicDamage,
        defense,
        magicDefense,
        healingAmount
    }
}
