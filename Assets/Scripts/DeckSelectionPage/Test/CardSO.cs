using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardSO : ScriptableObject
{
    public string cardName;
    public StatChange[] statChanges; // Array to hold multiple stat changes
    public Sprite cardSprite;
    public string description;

    public void UseCard(PlayerStats playerStats)
    {
        foreach (StatChange change in statChanges)
        {
            playerStats.ChangeStat(change.statToChange, change.amountToChangeStat);
        }
    }
}

[System.Serializable]
public class StatChange
{
    public PlayerStats.StatToChange statToChange;
    public int amountToChangeStat;
}
