using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardSO : ScriptableObject
{
    public string cardName;
    public List<StatChange> baseStatChanges;
    public Sprite cardSprite;
    public string description;
    public Rarity rarity;

    public int level = 1;
    public int maxLevel = 10;
    public int experience = 0;
    public int experienceToNextLevel = 100; // Base XP needed for first level up
    public float experienceScalingFactor = 1.5f; // How much more XP is needed for each level

    public List<LevelUpBonus> levelUpBonuses;
    // New field for the card prefab
    public GameObject cardPrefab;

    [System.Serializable]
    public class StatChange
    {
        public PlayerStats.StatToChange statToChange;
        public int amountToChangeStat;
    }

    [System.Serializable]
    public class LevelUpBonus
    {
        public PlayerStats.StatToChange statToChange;
        public float bonusPerLevel;
    }

    public GameObject CreateCardInstance()
    {
        if (cardPrefab == null)
        {
            Debug.LogError($"Card prefab is missing for {cardName}");
            return null;
        }

        GameObject cardInstance = Instantiate(cardPrefab);

        // You can add any additional setup here if needed
        // For example, you might want to set the card's name or add components

        return cardInstance;
    }

    public void UseCard(PlayerStats playerStats)
    {
        foreach (StatChange change in GetCurrentStatChanges())
        {
            playerStats.ChangeStat(change.statToChange, change.amountToChangeStat);
        }
    }

    public List<StatChange> GetCurrentStatChanges()
    {
        List<StatChange> currentChanges = new List<StatChange>();

        foreach (StatChange baseChange in baseStatChanges)
        {
            StatChange currentChange = new StatChange
            {
                statToChange = baseChange.statToChange,
                amountToChangeStat = baseChange.amountToChangeStat
            };

            LevelUpBonus bonus = levelUpBonuses.Find(b => b.statToChange == baseChange.statToChange);
            if (bonus != null)
            {
                float multiplier = 1 + (bonus.bonusPerLevel * (level - 1));
                currentChange.amountToChangeStat = Mathf.RoundToInt(currentChange.amountToChangeStat + multiplier);
            }

            currentChanges.Add(currentChange);
        }

        return currentChanges;
    }

    public void GainExperience(int amount)
    {
        if (level >= maxLevel) return;

        experience += amount;
        while (experience >= experienceToNextLevel && level < maxLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * experienceScalingFactor);

        // You could add more effects here, like updating the card's description
        UpdateDescription();
    }

    private void UpdateDescription()
    {
        description = $"{cardName} (Level {level})\n{GetBaseDescription()}";
    }

    private string GetBaseDescription()
    {
        // Implement this method to return the base description of the card
        // This could be stored as a separate field if it doesn't change
        return "Base description of the card";
    }

    public enum Rarity
    {
        Common,
        Rare,
        SuperRare,
        UltraRare,
        Legendary
    }
}


