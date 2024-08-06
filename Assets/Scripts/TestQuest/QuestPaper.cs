using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestPaper : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    private Quest1 quest;
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
    }

    public void OnClick()
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
}