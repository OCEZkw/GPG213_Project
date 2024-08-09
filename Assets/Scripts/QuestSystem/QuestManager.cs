using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;
    private Inventory inventory;

    private void Awake()
    {
        questMap = CreateQuestMap();

    }

    private void OnEnable()
    {
      //  GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
      //  GameEventsManager.instance.questEvents.onStartQuest += AdvanceQuest;
      //  GameEventsManager.instance.questEvents.onStartQuest += FinishQuest;
    }

    private void OnDisable()
    {
      //  GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
      //  GameEventsManager.instance.questEvents.onStartQuest -= AdvanceQuest;
      //  GameEventsManager.instance.questEvents.onStartQuest -= FinishQuest;
    }

    private void Start()
    {
        inventory = Inventory.Instance;
        // broadcast the initial state of all quests onstartup
        foreach (Quest quest in questMap.Values)
        {
          //  GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void StartQuest(string id)
    {
        // TODO - start the quest
        Debug.Log("Start Quest: " + id);
    }

    private void AdvanceQuest(string id)
    {
        // TODO - advance the quest
        Debug.Log("Advance Quest: " + id);
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        if (quest != null)
        {
            inventory.AddGold(quest.info.goldReward);
            // TODO: Add gems reward
            Debug.Log("Finish Quest: " + id);
        }
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // Loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if(idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map " + id);
        }
        return quest;
    }
}
