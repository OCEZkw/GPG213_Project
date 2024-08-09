using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Quest1> availableQuests = new List<Quest1>();
    public List<Quest1> activeQuests = new List<Quest1>();
    public List<Quest1> completedQuests = new List<Quest1>();

    // Reference to your GachaSystem
    public GachaSystem gachaSystem;

    private bool questsInitialized = false;

    public void SaveQuestData(Quest1 quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
        }
        availableQuests.Remove(quest);
    }

    public List<Quest1> GetAvailableQuests()
    {
        return availableQuests;
    }

    public List<Quest1> GetActiveQuests()
    {
        return activeQuests;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeQuests();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeQuests()
    {
        // Only initialize quests if the list is empty
        if (availableQuests.Count == 0)
        {
            // Create quests with all necessary data
            Quest1 quest1 = new Quest1
            {
                title = "Summon",
                description = "Take the gems and summon 10 cards from shrine down the village",
                goldReward = 100,
                gachaSystem = gachaSystem,
                goal = new QuestGoal
                {
                    goalType = GoalType.Summon,
                    requiredAmount = 10,
                    currentAmount = 0
                }
            };

            Quest1 quest2 = new Quest1
            {
                title = "Enhance 3 different Cards",
                description = "Enhance 3 different cards to improve their power.",
                goldReward = 200,
                gachaSystem = gachaSystem,
                goal = new QuestGoal
                {
                    goalType = GoalType.Enhance,
                    requiredAmount = 3,
                    currentAmount = 0
                }
            };

            // Add quests to the available quests list
            availableQuests.Add(quest1);
            availableQuests.Add(quest2);
            // Add more quests as needed

            questsInitialized = true;
        }
    }

    public void SavePlayerPosition(Vector3 position, string sceneName)
    {
        PlayerPrefs.SetFloat(sceneName + "_PosX", position.x);
        PlayerPrefs.SetFloat(sceneName + "_PosY", position.y);
        PlayerPrefs.SetFloat(sceneName + "_PosZ", position.z);
        PlayerPrefs.Save();
    }

    public Vector3 GetSavedPlayerPosition(string sceneName)
    {
        float x = PlayerPrefs.GetFloat(sceneName + "_PosX", 0f);
        float y = PlayerPrefs.GetFloat(sceneName + "_PosY", 0f);
        float z = PlayerPrefs.GetFloat(sceneName + "_PosZ", 0f);
        return new Vector3(x, y, z);
    }

    public bool HasSavedPosition(string sceneName)
    {
        return PlayerPrefs.HasKey(sceneName + "_PosX");
    }

    public void CompleteQuest(Quest1 quest)
    {
        quest.Complete();
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        Debug.Log($"Quest completed and moved to completed quests: {quest.title}");
    }

    public void CollectQuestReward(Quest1 quest)
    {
        if (completedQuests.Contains(quest))
        {
            // Add rewards to inventory
            Inventory.Instance.AddGold(quest.goldReward);
            // If you have gem rewards, add them here too
            // Inventory.Instance.AddGems(quest.gemReward);

            completedQuests.Remove(quest);
            Debug.Log($"Rewards collected for quest: {quest.title}. Added {quest.goldReward} gold.");

            // Trigger any necessary UI updates
            // For example: QuestBoard.Instance.UpdateQuestDisplay();
            // Trigger UI updates
            QuestBoard.Instance.UpdateQuestDisplay();
        }
        else
        {
            Debug.LogWarning($"Attempted to collect rewards for a quest that wasn't completed: {quest.title}");
        }
    }

    public void UpdateQuestProgress(GoalType goalType, int amount)
    {
        List<Quest1> questsToComplete = new List<Quest1>();

        foreach (Quest1 quest in activeQuests)
        {
            if (quest.goal.goalType == goalType)
            {
                quest.goal.currentAmount += amount;
                if (quest.goal.IsReached())
                {
                    questsToComplete.Add(quest);
                }
            }
        }

        foreach (Quest1 quest in questsToComplete)
        {
            CompleteQuest(quest);
        }
    }
}
