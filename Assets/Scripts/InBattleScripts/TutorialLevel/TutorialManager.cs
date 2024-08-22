using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject highlightPrefab;
    public GameObject textBubbleObject;
    public GameObject textBubbleObject2;
    public TextMeshProUGUI textBubbleText;
    public TextMeshProUGUI textBubbleText2;
    public TutorialWaveManager tutorialWaveManager;
    public TutorialDeckManager tutorialDeckManager;
    public NewCardClick newCardClick;
    public Canvas mainCanvas;

    private GameObject currentHighlight;
    private int currentStep = 0;
    private bool isTutorialActive = false;

    public List<TutorialPanel> tutorialPanels = new List<TutorialPanel>();
    // Replace Image with GloveAnimation
    public GloveAnimation glove;
    public GloveAnimation glove1;
    public GloveAnimation glove2;
    public GloveAnimation glove3;

    public Image NPC;

    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullText = "";

    void Start()
    {
        // Ensure the text bubble is hidden at the start
        textBubbleObject.SetActive(false);
        textBubbleObject2.SetActive(false);
    }

    void Update()
    {
        if (isTutorialActive && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                NextStep();
            }
        }
    }

    public void NextStep()
    {
        // Clear previous step
        RemoveHighlight();
        HideTextBubble();

        UpdatePanels();

        switch (currentStep)
        {
            case 0:
                Welcome();
                NPC.gameObject.SetActive(true);
                glove.gameObject.SetActive(true);
                ShowAndAnimateGlove(glove);
                break;
            case 1:
                Welcome2();
                break;
            case 2:
                Cost();
                break;
            case 3:
                Cost2(); 
                break;
            case 4:
                CardCost();
                break;
            case 5:
                TypingChart();
                break;
            case 6:
                CardType();
                break;
            case 7:
                CardType1();
                break;
            case 8:
                CardType2();
                break;
            case 9:
                EnemyType();
                break;
            case 10:
                glove.gameObject.SetActive(false);
                glove1.gameObject.SetActive(true);
                ShowAndAnimateGlove(glove1);
                HighlightCard();
                break;
            case 11:
                glove1.gameObject.SetActive(false);
                glove2.gameObject.SetActive(true);
                ShowAndAnimateGlove(glove2);
                HighlightEnemy();
                break;
            case 12:
                glove2.gameObject.SetActive(false);
                glove3.gameObject.SetActive(true);
                ShowAndAnimateGlove(glove3);
                ConfirmButton();
                break;
            case 13:
                glove3.gameObject.SetActive(false);
                NPC.gameObject.SetActive(false);
                EndTutorial();
                break;
            default:
                Debug.Log("Tutorial completed");
                break;
        }

        currentStep++;
    }

    void ShowAndAnimateGlove(GloveAnimation gloveAnim)
    {
        gloveAnim.gameObject.SetActive(true);
        gloveAnim.StartTapAnimation();
    }

    public void StartTutorial()
    {
        isTutorialActive = true;
        currentStep = 0;


        NextStep();
    }

    void EndTutorial()
    {
        isTutorialActive = false;
        textBubbleObject.SetActive(false);
        textBubbleObject2.SetActive(false);

        // Hide all panels at the end
        foreach (var panel in tutorialPanels)
        {
            panel.panel.SetActive(false);
        }
    }

    void Welcome()
    {
        ShowTextBubble("Hey you, I've never seen you around before you must be new to this village");
    }

    void Welcome2()
    {
        ShowTextBubble("My names Alisa, let me show you how things are done here");
    }

    void HighlightEnemy()
    {
        ShowTextBubble2("Good job! Now lets select the enemy and defeat it!");
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
        ShowTextBubble("Here shows the cards element and the type of effect it does. For example this is an attack card that is fire element");
    }

    void CardType1()
    {
        ShowTextBubble("This is a water element heal card which you can select yourself to heal yourself");
    }

    void CardType2()
    {
        ShowTextBubble("This is a water element shield card which you can select yourself to increase defense");
    }

    void EnemyType()
    {
        ShowTextBubble("The enemy element is displayed here make sure to refer to the chart and use effective cards to do more damage!");
    }

    void ConfirmButton()
    {
        ShowTextBubble2("Great job! Now press confirm to use the cards");
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

    void ShowTextBubble(string text)
    {
        textBubbleObject.SetActive(true);
        StartTyping(textBubbleText, text);
    }

    void ShowTextBubble2(string text)
    {
        textBubbleObject2.SetActive(true);
        StartTyping(textBubbleText2, text);
    }

    private void HideTextBubble()
    {
        textBubbleObject.SetActive(false);
        textBubbleObject2.SetActive(false);
        StopTyping();
    }

    private void StartTyping(TextMeshProUGUI textComponent, string text)
    {
        StopTyping();
        fullText = text;
        isTyping = true;
        typingCoroutine = StartCoroutine(TypeText(textComponent, text));
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        isTyping = false;
    }

    private void CompleteTyping()
    {
        StopTyping();
        TextMeshProUGUI activeText = textBubbleObject.activeSelf ? textBubbleText : textBubbleText2;
        activeText.text = fullText;
    }

    private IEnumerator TypeText(TextMeshProUGUI textComponent, string text)
    {
        textComponent.text = "";
        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private GameObject FindFirstEnemy()
    {
        return GameObject.FindGameObjectWithTag("Enemy");
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