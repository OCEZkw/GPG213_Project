using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestCompleteWindow : MonoBehaviour
{
    public static QuestCompleteWindow Instance { get; private set; }

    public GameObject windowObject;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI rewardText;
    public Button collectButton;

    private Quest1 currentQuest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        collectButton.onClick.AddListener(CollectReward);
        windowObject.SetActive(false);
    }

    public void OpenWindow(Quest1 quest)
    {
        currentQuest = quest;
        questTitleText.text = quest.title;
        rewardText.text = $"Reward: {quest.goldReward} Gold";
        windowObject.SetActive(true);
    }

    private void CollectReward()
    {
        if (currentQuest != null)
        {
            GameManager.Instance.CollectQuestReward(currentQuest);
            windowObject.SetActive(false);
            currentQuest = null;
        }
    }
}