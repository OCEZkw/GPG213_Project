using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal
{
    public GoalType goalType;

    public int requiredAmount;
    public int currentAmount;

    public bool IsReached()
    {
        return (currentAmount >= requiredAmount);
    }

    public void CardSummoned()
    {
        if (goalType == GoalType.Summon)
        {
            currentAmount++;
        }
    }

    public void CardEnhanced()
    {
        if (goalType == GoalType.Enhance)
        {
            currentAmount++;
        }
    }
}

public enum GoalType
{
    Summon,
    Enhance
}
