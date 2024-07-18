using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [System.Serializable]
    public class StatTextPair
    {
        public PlayerStats.StatToChange statType;
        public TextMeshProUGUI textComponent;
        public string prefix = "";
    }

    public StatTextPair[] statTexts;

    void Start()
    {
        UpdateAllStats();
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged += UpdateAllStats;
        }
    }
    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnStatsChanged -= UpdateAllStats;
        }
    }

    public void UpdateAllStats()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats instance not found!");
            return;
        }

        foreach (var statText in statTexts)
        {
            UpdateStatText(statText);
        }
    }

    void UpdateStatText(StatTextPair statText)
    {
        if (statText.textComponent == null)
        {
            Debug.LogWarning($"Text component for {statText.statType} is not assigned!");
            return;
        }

        int statValue = GetStatValue(statText.statType);
        statText.textComponent.text = $"{statText.prefix}{statValue}";
    }

    int GetStatValue(PlayerStats.StatToChange statType)
    {
        switch (statType)
        {
            case PlayerStats.StatToChange.health:
                return PlayerStats.Instance.maxHealth;
            case PlayerStats.StatToChange.attackDamage:
                return PlayerStats.Instance.attackDamage;
            case PlayerStats.StatToChange.magicDamage:
                return PlayerStats.Instance.magicDamage;
            case PlayerStats.StatToChange.defense:
                return PlayerStats.Instance.defense;
            case PlayerStats.StatToChange.magicDefense:
                return PlayerStats.Instance.magicDefense;
            case PlayerStats.StatToChange.healingAmount:
                return PlayerStats.Instance.healingAmount;
            default:
                Debug.LogError($"Unknown stat type: {statType}");
                return 0;
        }
    }
}
