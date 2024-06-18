using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public List<GameObject> allCards;  // All possible cards
    public Transform[] handPositions;  // Positions where the cards will be displayed
    public List<GameObject> deck = new List<GameObject>();  // Player's selected deck
    public List<GameObject> hand = new List<GameObject>();  // Cards currently in hand
    public Transform confirmedCardPosition;  // Position for the confirmed card
    public ConfirmHandler confirmHandler;  // Reference to the ConfirmHandler
    public Transform canvasTransform;

    void Start()
    {
        confirmHandler.deckManager = this;  // Assign this deck manager to the confirm handler
        InitializeDeck();
        DrawHand();
    }

    void InitializeDeck()
    {
        // Shuffle allCards
        Shuffle(allCards);

        // Take the first 10 cards for the deck
        for (int i = 0; i < Mathf.Min(10, allCards.Count); i++)
        {
            deck.Add(allCards[i]);
        }
    }

    void DrawHand()
    {
        // Shuffle the deck
        Shuffle(deck);

        // Take the first 5 cards for the hand
        for (int i = 0; i < Mathf.Min(5, deck.Count); i++)
        {
            // Instantiate the card prefab and parent it to the canvas
            GameObject card = Instantiate(deck[i], canvasTransform);

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

            // Get the CardClickHandler component if it exists and add the card to the hand list
            var cardClickHandler = card.GetComponent<CardClickHandler>();
            if (cardClickHandler != null)
            {
                cardClickHandler.deckManager = this;
            }

            hand.Add(card);
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
}