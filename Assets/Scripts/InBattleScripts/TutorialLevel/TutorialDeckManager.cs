using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDeckManager : MonoBehaviour, IDeckManager
{
    public List<GameObject> tutorialDeck;  // Predefined cards for the tutorial
    public Transform[] handPositions;  // Positions where the cards will be displayed
    public List<GameObject> hand = new List<GameObject>();  // Cards currently in hand
    public Transform confirmedCardPosition;  // Position for the confirmed card
    public ConfirmHandler confirmHandler;  // Reference to the ConfirmHandler
    public Transform canvasTransform;
    public List<GameObject> GetHand()
    {
        return hand;
    }
    void Start()
    {
        if (confirmHandler != null)
        {
            //confirmHandler.deckManager = this;  // Assign this deck manager to the confirm handler
        }
        InitializeDeck();
        DrawHand();
    }
    void InitializeDeck()
    {
        if (tutorialDeck == null || tutorialDeck.Count == 0)
        {
            Debug.LogError("Tutorial deck is not set!");
            return;
        }
        Debug.Log("Tutorial deck initialized with " + tutorialDeck.Count + " cards.");
    }
    void DrawHand()
    {
        Debug.Log("Drawing tutorial hand...");
        Debug.Log("Deck count: " + tutorialDeck.Count);
        // Take the first 5 cards (or fewer if the deck is smaller) for the hand
        for (int i = 0; i < Mathf.Min(5, tutorialDeck.Count); i++)
        {
            // Instantiate the card prefab and parent it to the canvas
            GameObject card = Instantiate(tutorialDeck[i], canvasTransform);
            if (card == null)
            {
                Debug.LogError("Failed to instantiate card prefab from deck!");
                continue;
            }
            // Set the card's RectTransform properties to match the corresponding hand position
            RectTransform cardRectTransform = card.GetComponent<RectTransform>();
            RectTransform handPositionRectTransform = handPositions[i] as RectTransform;
            // Copy RectTransform properties
            cardRectTransform.anchorMin = handPositionRectTransform.anchorMin;
            cardRectTransform.anchorMax = handPositionRectTransform.anchorMax;
            cardRectTransform.pivot = handPositionRectTransform.pivot;
            cardRectTransform.anchoredPosition = handPositionRectTransform.anchoredPosition;
            cardRectTransform.sizeDelta = handPositionRectTransform.sizeDelta;
            // Ensure the card has a Graphic component with Raycast Target enabled
            Image image = card.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }
            // Ensure the card has a BoxCollider2D and adjust its size
            BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                boxCollider = card.AddComponent<BoxCollider2D>();
            }
            boxCollider.size = cardRectTransform.sizeDelta;
            // Get the CardClickHandler component if it exists and set the deck manager
            var cardClickHandler = card.GetComponent<CardClickHandler>();
            if (cardClickHandler != null)
            {
                // cardClickHandler.deckManager = this;
            }
            hand.Add(card);
        }
        Debug.Log("Tutorial hand count: " + hand.Count);
    }
    // Add any additional methods you need for tutorial-specific functionality
    // For example, you might want to add methods to highlight specific cards,
    // force the player to select a certain card, etc.
}