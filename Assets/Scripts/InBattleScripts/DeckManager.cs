using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class DeckManager : MonoBehaviour, IDeckManager
{
    public List<GameObject> allCards;  // All possible cards
    public Transform[] handPositions;  // Positions where the cards will be displayed
    public List<GameObject> deck = new List<GameObject>();  // Player's selected deck
    public List<GameObject> hand = new List<GameObject>();  // Cards currently in hand
    public Transform confirmedCardPosition;  // Position for the confirmed card
    public ConfirmHandler confirmHandler;  // Reference to the ConfirmHandler
    public Transform canvasTransform;

    public Transform deckPosition; // Add this for the deck position
    public float drawDuration = 0.5f; // Duration of the draw animation
    public float flipDuration = 0.3f; // Duration of the flip animation

    public List<GameObject> lockedCards = new List<GameObject>();

    private List<GameObject> temporaryObjects = new List<GameObject>();

    public List<GameObject> GetHand()
    {
        return hand;
    }

    void Start()
    {
        confirmHandler.deckManager = this;  // Assign this deck manager to the confirm handler
        InitializeDeck();
        DrawHand();
    }

    void InitializeDeck()
    {
        // Clear the existing deck
        deck.Clear();

        // Get the selected cards from DeckData
        List<CardSO> selectedCards = DeckData.Instance.selectedCards;

        // Create GameObjects for each selected card and add them to the deck
        foreach (CardSO cardSO in selectedCards)
        {
            GameObject cardObject = cardSO.CreateCardInstance();
            if (cardObject != null)
            {
                // Set the card's parent to this DeckManager or another appropriate transform
                cardObject.transform.SetParent(transform);

                // Set the initial position (you might want to adjust this)
                cardObject.transform.position = deckPosition.position;

                // Add the card to the deck
                deck.Add(cardObject);
            }
        }

        // Shuffle the deck
        Shuffle(deck);
    }

    void DrawHand()
    {
        Debug.Log("Drawing hand...");
        Debug.Log("Deck count: " + deck.Count);
        Shuffle(deck);

        StartCoroutine(DrawHandCoroutine());
    }

    IEnumerator DrawHandCoroutine()
    {
        int cardsToDraw = Mathf.Min(5, deck.Count);
        List<GameObject> drawnCards = new List<GameObject>();
        List<RectTransform> cardRects = new List<RectTransform>();

        // Instantiate all cards at once
        for (int i = 0; i < cardsToDraw; i++)
        {
            GameObject cardObject = Instantiate(deck[i], deckPosition.position, Quaternion.identity, canvasTransform);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();

            // Set up initial state
            cardRect.localScale = Vector3.one;
            cardObject.transform.Find("CardFront").gameObject.SetActive(false);
            cardObject.transform.Find("CardBack").gameObject.SetActive(true);

            drawnCards.Add(cardObject);
            cardRects.Add(cardRect);
        }

        // Create a sequence for all animations
        Sequence drawSequence = DOTween.Sequence();

        // Move all cards to hand positions simultaneously
        for (int i = 0; i < cardsToDraw; i++)
        {
            RectTransform handPositionRect = handPositions[i] as RectTransform;
            drawSequence.Join(cardRects[i].DOAnchorPos(handPositionRect.anchoredPosition, drawDuration).SetEase(Ease.OutQuad));
        }

        // Wait for move animations to complete before starting flip animations
        drawSequence.AppendInterval(0.1f); // Small delay before flipping

        // Flip all cards simultaneously
        for (int i = 0; i < cardsToDraw; i++)
        {
            RectTransform cardRect = cardRects[i];
            GameObject cardObject = drawnCards[i];
            RectTransform handPositionRect = handPositions[i] as RectTransform;

            // Set anchor and pivot before flipping
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

        // Wait for the entire sequence to complete
        yield return drawSequence.Play().WaitForCompletion();

        // Setup all cards and add to hand
        for (int i = 0; i < cardsToDraw; i++)
        {
            GameObject cardObject = drawnCards[i];
            SetupCard(cardObject, i);
            hand.Add(cardObject);

            // Initialize the CardFloatEffect after all animations are complete
            CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
            if (floatEffect != null)
            {
                RectTransform handPositionRect = handPositions[i] as RectTransform;
                floatEffect.Initialize(handPositionRect.anchoredPosition);
            }
        }
    }

    IEnumerator DrawCard(int handIndex)
    {
        GameObject cardObject = Instantiate(deck[handIndex], deckPosition.position, Quaternion.identity, canvasTransform);
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();

        // Find the card front and back
        Image cardFront = cardObject.transform.Find("CardFront").GetComponent<Image>();
        Image cardBack = cardObject.transform.Find("CardBack").GetComponent<Image>();

        if (cardFront == null || cardBack == null)
        {
            Debug.LogError("Card front or back not found in prefab: " + cardObject.name);
            yield break;
        }

        // Set up initial state
        cardRect.localScale = Vector3.one;
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(true);

        // Move to hand position
        RectTransform handPositionRect = handPositions[handIndex] as RectTransform;
        yield return cardRect.DOAnchorPos(handPositionRect.anchoredPosition, drawDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        // Set anchor and pivot before flipping
        cardRect.anchorMin = handPositionRect.anchorMin;
        cardRect.anchorMax = handPositionRect.anchorMax;
        cardRect.pivot = handPositionRect.pivot;
        cardRect.sizeDelta = handPositionRect.sizeDelta;

        // Flip card at its current position
        yield return cardRect.DORotate(new Vector3(0, 90, 0), flipDuration / 2).SetEase(Ease.InOutQuad).OnComplete(() => {
            cardFront.gameObject.SetActive(true);
            cardBack.gameObject.SetActive(false);
        }).WaitForCompletion();

        yield return cardRect.DORotate(Vector3.zero, flipDuration / 2).SetEase(Ease.InOutQuad).WaitForCompletion();

        // Setup card components
        SetupCard(cardObject, handIndex);

        hand.Add(cardObject);

        // Initialize the CardFloatEffect after all animations are complete
        CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
        if (floatEffect != null)
        {
            floatEffect.Initialize(handPositionRect.anchoredPosition);
        }
    }

    void SetupCard(GameObject card, int handIndex)
    {
        // Ensure the card has a BoxCollider2D and adjust its size
        BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = card.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = card.GetComponent<RectTransform>().sizeDelta;

        // Get the CardClickHandler component if it exists and add the card to the hand list
        var cardClickHandler = card.GetComponent<CardClickHandler>();
        if (cardClickHandler != null)
        {
            cardClickHandler.deckManager = this;
        }

        // Ensure the CardFloatEffect is present (but don't initialize it yet)
        if (card.GetComponent<CardFloatEffect>() == null)
        {
            card.AddComponent<CardFloatEffect>();
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            GameObject temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }

    public GameObject LockRandomCard()
    {
        List<GameObject> unlockedCards = hand.Where(card => !lockedCards.Contains(card)).ToList();

        if (unlockedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedCards.Count);
            GameObject cardToLock = unlockedCards[randomIndex];
            LockCard(cardToLock);
            Debug.Log($"Locked card: {cardToLock.name}");
            return cardToLock;
        }
        else
        {
            Debug.Log("No unlocked cards available to lock.");
            return null;
        }
    }

    public void LockCard(GameObject card)
    {
        if (!lockedCards.Contains(card))
        {
            lockedCards.Add(card);
            NewCardClick cardClick = card.GetComponent<NewCardClick>();
            if (cardClick != null)
            {
                cardClick.SetLocked(true);
            }
            Debug.Log($"Card locked: {card.name}");
        }
    }

    public void UnlockCard(GameObject card)
    {
        if (lockedCards.Contains(card))
        {
            lockedCards.Remove(card);
            NewCardClick cardClick = card.GetComponent<NewCardClick>();
            if (cardClick != null)
            {
                cardClick.SetLocked(false);
            }
            Debug.Log($"Card unlocked: {card.name}");
        }
    }
}