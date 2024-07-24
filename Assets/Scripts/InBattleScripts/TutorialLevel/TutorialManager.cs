using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public Image darkOverlay;
    public GameObject highlightPrefab;
    public GameObject textBubbleObject;
    public TextMeshProUGUI textBubbleText;
    public WaveManager waveManager;
    public TutorialDeckManager tutorialDeckManager;
    public NewCardClick newCardClick;
    public Canvas mainCanvas;

    private GameObject currentHighlight;
    private int currentStep = 0;
    private bool isTutorialActive = false;

    void Start()
    {
        // Ensure the text bubble is hidden at the start
        textBubbleObject.SetActive(false);
        StartTutorial();
    }

    void Update()
    {
        if (isTutorialActive && Input.GetMouseButtonDown(0))  // Left mouse button
        {
            NextStep();
        }
    }

    void NextStep()
    {
        // Clear previous step
        RemoveHighlight();
        HideTextBubble();

        switch (currentStep)
        {
            case 0:
                HighlightEnemy();
                break;
            case 1:
                HighlightCard();
                PauseGame();
                break;
            case 2:
                Cost();
                break;
            case 3:
                Cost2();
                break;
            case 4:
                EndTutorial();
                break;
            default:
                Debug.Log("Tutorial completed");
                break;
        }

        currentStep++;
    }

    void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = 0;
        darkOverlay.gameObject.SetActive(true);
        NextStep();
    }

    void HighlightEnemy()
    {
        if (waveManager.enemiesRemainingAlive > 0)
        {
            GameObject enemy = FindFirstEnemy();
            if (enemy != null)
            {
                HighlightWorldObject(enemy.transform);
                ShowTextBubble("This is an enemy. Defeat it with your cards!");
            }
        }
        else
        {
            ShowTextBubble("No enemies found. Let's move on.");
        }
    }

    void HighlightCard()
    {
        if (tutorialDeckManager.hand.Count > 0)
        {
            GameObject card = tutorialDeckManager.hand[0];
            HighlightUIObject(card.GetComponent<RectTransform>());
            ShowTextBubble("This is a card. Click on it to select it!");
        }
        else
        {
            ShowTextBubble("No cards in hand. Let's move on.");
        }
    }

    void Cost()
    {
        ShowTextBubble("This is your mana cost which is recovered to the maximum in the next turn");
    }

    void Cost2()
    {
        ShowTextBubble("The mana cost increases by 1 each turn. You should always keep this in mind when planning on how to use your cards");
    }

    void EndTutorial()
    {
        isTutorialActive = false;
        darkOverlay.gameObject.SetActive(false);
        textBubbleObject.SetActive(false);
        ResumeGame();
    }

    private void HighlightWorldObject(Transform objectToHighlight)
    {
        RemoveHighlight();
        currentHighlight = Instantiate(highlightPrefab, mainCanvas.transform);
        RectTransform highlightRect = currentHighlight.GetComponent<RectTransform>();

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(objectToHighlight.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.GetComponent<RectTransform>(),
            screenPoint, null, out Vector2 localPoint);

        highlightRect.anchorMin = highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        highlightRect.anchoredPosition = localPoint;
        highlightRect.sizeDelta = new Vector2(100, 100); // Adjust size as needed
    }

    private void HighlightUIObject(RectTransform objectToHighlight)
    {
        RemoveHighlight();
        currentHighlight = Instantiate(highlightPrefab, mainCanvas.transform);
        RectTransform highlightRect = currentHighlight.GetComponent<RectTransform>();

        Vector2 screenPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.GetComponent<RectTransform>(),
            objectToHighlight.position, null, out screenPoint);

        highlightRect.anchorMin = highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        highlightRect.anchoredPosition = screenPoint;
        highlightRect.sizeDelta = objectToHighlight.sizeDelta + new Vector2(20, 20);
    }

    private void RemoveHighlight()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
            currentHighlight = null;
        }
    }

    private void ShowTextBubble(string text)
    {
        textBubbleObject.SetActive(true);
        textBubbleText.text = text;
    }

    private void HideTextBubble()
    {
        textBubbleObject.SetActive(false);
    }

    private GameObject FindFirstEnemy()
    {
        return GameObject.FindGameObjectWithTag("Enemy");
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        // Disable other game systems here
        if (waveManager != null) waveManager.enabled = false;
        if (tutorialDeckManager != null) tutorialDeckManager.enabled = false;
        if (newCardClick != null) newCardClick.enabled = false;
        // Add any other systems that need to be paused
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        // Re-enable other game systems here
        if (waveManager != null) waveManager.enabled = true;
        if (tutorialDeckManager != null) tutorialDeckManager.enabled = true;
        if (newCardClick != null) newCardClick.enabled = true; ;
        // Add any other systems that need to be resumed
    }
}