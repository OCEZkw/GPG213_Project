using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestPaper : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;
    public Quest1 quest { get; private set; }
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError("No Button component found on QuestPaper!");
        }
    }

    public void SetQuest(Quest1 newQuest)
    {
        quest = newQuest;
        if (titleText != null)
        {
            titleText.text = quest.title;
        }
        else
        {
            Debug.LogError("TitleText is not assigned in QuestPaper!");
        }

        if (statusText != null)
        {
            statusText.text = quest.isCompleted ? "Completed" : "Available";
        }
    }

    public void OnClick()
    {
        if (quest.isCompleted)
        {
            OpenQuestCompleteWindow();
        }
        else
        {
            OpenQuestWindow();
        }
    }

    private void OpenQuestWindow()
    {
        Debug.Log("QuestPaper clicked: " + quest.title);
        if (quest != null)
        {
            QuestGiver.OpenQuestWindow(quest);
        }
        else
        {
            Debug.LogError("No quest assigned to this QuestPaper!");
        }
    }

    private void CollectReward()
    {
        GameManager.Instance.CollectQuestReward(quest);
        // You might want to update the UI or remove this quest paper after collecting the reward
        Destroy(gameObject);
    }

    public void DisableButton()
    {
        if (button != null)
        {
            button.interactable = false;
        }
    }

    private void OpenQuestCompleteWindow()
    {
        QuestCompleteWindow.Instance.OpenWindow(quest);
    }
}