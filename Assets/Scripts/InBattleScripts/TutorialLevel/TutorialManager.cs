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

    public List<TutorialPanel> tutorialPanels = new List<TutorialPanel>();

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

        UpdatePanels();

        switch (currentStep)
        {
            case 0:
                Cost();
                break;
            case 1:
                Cost2();
                
                break;
            case 2:
                CardCost();
                break;
            case 3:
                TypingChart();
                break;
            case 4:
                CardType();
                break;
            case 5:
                EnemyType();
                break;
            case 6:
                HighlightCard();
                break;
            case 7:
                HighlightEnemy();
                break;
            case 8:
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

        // Hide all panels at the start
        foreach (var panel in tutorialPanels)
        {
            panel.panel.SetActive(false);
        }

        NextStep();
    }

    void EndTutorial()
    {
        isTutorialActive = false;
        darkOverlay.gameObject.SetActive(false);
        textBubbleObject.SetActive(false);

        // Hide all panels at the end
        foreach (var panel in tutorialPanels)
        {
            panel.panel.SetActive(false);
        }

        ResumeGame();
    }

    void HighlightEnemy()
    {
        if (waveManager.enemiesRemainingAlive > 0)
        {
            GameObject enemy = FindFirstEnemy();
            if (enemy != null)
            {
                HighlightWorldObject(enemy.transform);
                ShowTextBubble("Good job! Now lets select the enemy and defeat it!");
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
            ShowTextBubble("Now lets select a card. Click on it to select it!");
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

    void CardCost()
    {
        ShowTextBubble("Here shows the mana cost to play each card. As long as you have enough mana cost you can play as many cards as you want");
    }

    void TypingChart()
    {
        ShowTextBubble("This is the element chart each element has their strengths and weaknesses. Each enemy and card have their own element");
    }

    void CardType()
    {
        ShowTextBubble("Here shows the cards element and the type of effect it does");
    }

    void EnemyType()
    {
        ShowTextBubble("Enemy element is displayed here");
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

    void UpdatePanels()
    {
        foreach (var panel in tutorialPanels)
        {
            if (currentStep == panel.stepToShow)
            {
                panel.panel.SetActive(true);
            }
            else
            {
                panel.panel.SetActive(false);
            }
        }
    }
}