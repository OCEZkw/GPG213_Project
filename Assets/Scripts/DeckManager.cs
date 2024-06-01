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
    public Button confirmButton;  // Reference to the confirm button
    public Transform confirmedCardPosition;  // Position for the confirmed card
    public ConfirmHandler confirmHandler;  // Reference to the ConfirmHandler

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
            GameObject card = Instantiate(deck[i], handPositions[i].position, Quaternion.identity);
            var cardClickHandler = card.GetComponent<CardClickHandler>();
            cardClickHandler.confirmButton = confirmButton;  // Assign the confirm button
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