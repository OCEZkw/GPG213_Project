using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest1
{
    public bool isActive;

    public string title;
    public string description;
    public int goldReward;
    public GachaSystem gachaSystem;

    public QuestGoal goal;

    public void Complete()
    {
        isActive = false;
        Debug.Log(title + " was completed!");
    }
}
