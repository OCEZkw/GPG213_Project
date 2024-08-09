using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public GameObject questWindow;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI goldText;
    public PlayerController player;
    public Button acceptButton;

    private static QuestGiver instance;
    private Quest1 currentQuest;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        acceptButton.onClick.AddListener(AcceptCurrentQuest);
    }

    public static void OpenQuestWindow(Quest1 quest)
    {
        instance.currentQuest = quest;
        instance.questWindow.SetActive(true);
        instance.titleText.text = quest.title;
        instance.descriptionText.text = quest.description;
        instance.goldText.text = quest.goldReward.ToString();
    }

    public void AcceptCurrentQuest()
    {
        if (currentQuest != null)
        {
            AcceptQuest(currentQuest);
        }
    }

    private void AcceptQuest(Quest1 quest)
    {
        questWindow.SetActive(false);
        quest.isActive = true;
        GameManager.Instance.SaveQuestData(quest);
        player.quest = quest;
        Debug.Log($"Quest accepted: {quest.title}");

        // Disable the button on the corresponding QuestPaper
        DisableQuestPaperButton(quest);
    }

    private void DisableQuestPaperButton(Quest1 quest)
    {
        QuestPaper[] questPapers = FindObjectsOfType<QuestPaper>();
        foreach (QuestPaper paper in questPapers)
        {
            if (paper.quest == quest)
            {
                paper.DisableButton();
                break;
            }
        }
    }
}
