using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialDeckManager : MonoBehaviour, IDeckManager
{
    public List<GameObject> tutorialDeck;  // Predefined cards for the tutorial
    public Transform[] handPositions;  // Positions where the cards will be displayed
    public List<GameObject> hand = new List<GameObject>();  // Cards currently in hand
    public Transform confirmedCardPosition;  // Position for the confirmed card
    public TutorialConfirmHandler tutorialConfirmHandler;
    public Transform canvasTransform;
    public Transform deckPosition; // Position for the deck
    public float drawDuration = 0.5f; // Duration of the draw animation
    public float flipDuration = 0.3f; // Duration of the flip animation

    public GameObject frontObject;  // The object that should be in front of the cards
    public GameObject backObject;   // The object that should be behind the cards

    public List<GameObject> GetHand()
    {
        return hand;
    }

    void Start()
    {
        if (tutorialConfirmHandler != null)
        {
            tutorialConfirmHandler.tutorialDeckManager = this;  // Assign this deck manager to the tutorial confirm handler
        }
      //  InitializeDeck();
      //  StartCoroutine(DrawHand());
    }

    public void InitializeDeck()
    {
        if (tutorialDeck == null || tutorialDeck.Count == 0)
        {
            Debug.LogError("Tutorial deck is not set!");
            return;
        }
        StartCoroutine(DrawHand());
        Debug.Log("Tutorial deck initialized with " + tutorialDeck.Count + " cards.");
    }

    public IEnumerator DrawHand()
    {
        Debug.Log("Drawing tutorial hand...");
        Debug.Log("Deck count: " + tutorialDeck.Count);

        int frontIndex = frontObject.transform.GetSiblingIndex();
        int backIndex = backObject.transform.GetSiblingIndex();
        if (frontIndex < backIndex)
        {
            int temp = frontIndex;
            frontIndex = backIndex;
            backIndex = temp;
        }
        int cardIndex = backIndex + 1;

        List<GameObject> drawnCards = new List<GameObject>();
        List<RectTransform> cardRects = new List<RectTransform>();

        for (int i = 0; i < Mathf.Min(5, tutorialDeck.Count); i++)
        {
            GameObject cardObject = Instantiate(tutorialDeck[i], deckPosition.position, Quaternion.identity, canvasTransform);
            cardObject.transform.SetSiblingIndex(cardIndex++);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();

            cardRect.localScale = Vector3.one;
            cardObject.transform.Find("CardFront").gameObject.SetActive(false);
            cardObject.transform.Find("CardBack").gameObject.SetActive(true);

            drawnCards.Add(cardObject);
            cardRects.Add(cardRect);
        }

        Sequence drawSequence = DOTween.Sequence();

        for (int i = 0; i < drawnCards.Count; i++)
        {
            RectTransform handPositionRect = handPositions[i] as RectTransform;
            drawSequence.Join(cardRects[i].DOAnchorPos(handPositionRect.anchoredPosition, drawDuration).SetEase(Ease.OutQuad));
        }

        drawSequence.AppendInterval(0.1f);

        for (int i = 0; i < drawnCards.Count; i++)
        {
            RectTransform cardRect = cardRects[i];
            GameObject cardObject = drawnCards[i];
            RectTransform handPositionRect = handPositions[i] as RectTransform;

            cardRect.anchorMin = handPositionRect.anchorMin;
            cardRect.anchorMax = handPositionRect.anchorMax;
            cardRect.pivot = handPositionRect.pivot;
            cardRect.sizeDelta = handPositionRect.sizeDelta;

            Sequence flipSequence = DOTween.Sequence();
            flipSequence.Append(cardRect.DORotate(new Vector3(0, 90, 0), flipDuration / 2).SetEase(Ease.InOutQuad));
            flipSequence.AppendCallback(() => {
                cardObject.transform.Find("CardFront").gameObject.SetActive(true);
                cardObject.transform.Find("CardBack").gameObject.SetActive(false);
            });
            flipSequence.Append(cardRect.DORotate(Vector3.zero, flipDuration / 2).SetEase(Ease.InOutQuad));

            drawSequence.Join(flipSequence);
        }

        yield return drawSequence.Play().WaitForCompletion();

        for (int i = 0; i < drawnCards.Count; i++)
        {
            GameObject cardObject = drawnCards[i];
            SetupCard(cardObject, i);
            hand.Add(cardObject);

            CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
            if (floatEffect != null)
            {
                RectTransform handPositionRect = handPositions[i] as RectTransform;
                floatEffect.Initialize(handPositionRect.anchoredPosition);
            }
        }

        Debug.Log("Tutorial hand count: " + hand.Count);
    }

    void SetupCard(GameObject card, int handIndex)
    {
        BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = card.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = card.GetComponent<RectTransform>().sizeDelta;

        var cardClickHandler = card.GetComponent<NewCardClick>();
        if (cardClickHandler != null)
        {
            cardClickHandler.deckManager = this;
            cardClickHandler.buttonManager = ButtonManager.Instance;
            cardClickHandler.roundManager = FindObjectOfType<RoundManager>();
            cardClickHandler.player = FindObjectOfType<Player>();
        }

        if (card.GetComponent<CardFloatEffect>() == null)
        {
            card.AddComponent<CardFloatEffect>();
        }

        Image image = card.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }

    public void ReplaceCard(GameObject usedCard)
    {
        int cardIndex = hand.IndexOf(usedCard);
        if (cardIndex != -1)
        {
            hand.RemoveAt(cardIndex);
            Destroy(usedCard);

            if (tutorialDeck.Count > 0)
            {
                List<GameObject> availableCards = new List<GameObject>(tutorialDeck);
                foreach (var card in hand)
                {
                    availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                }
                availableCards.RemoveAll(c => c.name == usedCard.name.Replace("(Clone)", ""));

                if (availableCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableCards.Count);
                    StartCoroutine(DrawCard(cardIndex, availableCards[randomIndex]));
                }
            }
        }
    }

    IEnumerator DrawCard(int handIndex, GameObject cardPrefab)
    {
        GameObject cardObject = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, canvasTransform);
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();

        cardRect.localScale = Vector3.one;
        cardObject.transform.Find("CardFront").gameObject.SetActive(false);
        cardObject.transform.Find("CardBack").gameObject.SetActive(true);

        RectTransform handPositionRect = handPositions[handIndex] as RectTransform;
        yield return cardRect.DOAnchorPos(handPositionRect.anchoredPosition, drawDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        cardRect.anchorMin = handPositionRect.anchorMin;
        cardRect.anchorMax = handPositionRect.anchorMax;
        cardRect.pivot = handPositionRect.pivot;
        cardRect.sizeDelta = handPositionRect.sizeDelta;

        yield return cardRect.DORotate(new Vector3(0, 90, 0), flipDuration / 2).SetEase(Ease.InOutQuad).OnComplete(() => {
            cardObject.transform.Find("CardFront").gameObject.SetActive(true);
            cardObject.transform.Find("CardBack").gameObject.SetActive(false);
        }).WaitForCompletion();

        yield return cardRect.DORotate(Vector3.zero, flipDuration / 2).SetEase(Ease.InOutQuad).WaitForCompletion();

        SetupCard(cardObject, handIndex);
        hand.Insert(handIndex, cardObject);

        CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
        if (floatEffect != null)
        {
            floatEffect.Initialize(handPositionRect.anchoredPosition);
        }
    }

    public void HideAllCards()
    {
        foreach (GameObject card in hand)
        {
            if (card != null)
            {
                card.SetActive(false);
            }
        }
    }
}