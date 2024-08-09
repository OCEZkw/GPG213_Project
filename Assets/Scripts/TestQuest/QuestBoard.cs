using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    public GameObject questPaperPrefab;
    public GameObject completedQuestPaperPrefab;
    public Transform questContainer;
    public List<Quest1> availableQuests;

    public int columns = 3;
    public float spacingX = 100f;
    public float spacingY = 150f;
    public Vector2 startPosition = new Vector2(-100f, 100f);

    public static QuestBoard Instance { get; private set; }

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
        DisplayQuests();
    }

    private void DisplayQuests()
    {
        // Clear existing quest papers
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }

        List<Quest1> availableQuests = GameManager.Instance.availableQuests;
        List<Quest1> completedQuests = GameManager.Instance.completedQuests;

        int index = 0;

        // Display available quests
        foreach (Quest1 quest in availableQuests)
        {
            CreateQuestPaper(quest, questPaperPrefab, ref index);
        }

        // Display completed quests
        foreach (Quest1 quest in completedQuests)
        {
            CreateQuestPaper(quest, completedQuestPaperPrefab, ref index);
        }
    }

    private void CreateQuestPaper(Quest1 quest, GameObject prefab, ref int index)
    {
        GameObject questPaper = Instantiate(prefab, questContainer);
        QuestPaper paperScript = questPaper.GetComponent<QuestPaper>();
        paperScript.SetQuest(quest);

        // Calculate position
        int row = index / columns;
        int col = index % columns;
        float posX = startPosition.x + (col * spacingX);
        float posY = startPosition.y - (row * spacingY);

        // Set position
        RectTransform rectTransform = questPaper.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(posX, posY);

        index++;
    }

    public void UpdateQuestDisplay()
    {
        // Clear existing quest papers
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }

        int index = 0;

        // Display available quests
        foreach (Quest1 quest in GameManager.Instance.availableQuests)
        {
            CreateQuestPaper(quest, questPaperPrefab, ref index);
        }

        // Display completed quests
        foreach (Quest1 quest in GameManager.Instance.completedQuests)
        {
            CreateQuestPaper(quest, completedQuestPaperPrefab, ref index);
        }
    }
}
