using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHealth = 1000;
    public int attackDamage = 50;
    public int magicDamage = 50;
    public int defense = 50;
    public int magicDefense = 50;
    public int healingAmount = 50;

    public delegate void StatChangedDelegate();
    public event StatChangedDelegate OnStatsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        OnStatsChanged?.Invoke();
    }

    public void ReverseStatChange(StatToChange stat, int amount)
    {
        ChangeStat(stat, -amount);
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
