using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    public GameObject questPaperPrefab;
    public Transform questContainer;
    public List<Quest1> availableQuests;

    public int columns = 3;
    public float spacingX = 100f;
    public float spacingY = 150f;
    public Vector2 startPosition = new Vector2(-100f, 100f);

    private void Start()
    {
        DisplayQuests();
    }

    private void DisplayQuests()
    {
        for (int i = 0; i < availableQuests.Count; i++)
        {
            Quest1 quest = availableQuests[i];
            GameObject questPaper = Instantiate(questPaperPrefab, questContainer);
            QuestPaper paperScript = questPaper.GetComponent<QuestPaper>();
            paperScript.SetQuest(quest);

            // Calculate position
            int row = i / columns;
            int col = i % columns;
            float posX = startPosition.x + (col * spacingX);
            float posY = startPosition.y - (row * spacingY);

            // Set position
            RectTransform rectTransform = questPaper.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(posX, posY);
        }
    }
}
