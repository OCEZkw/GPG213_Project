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
        // Ensure the deck has unique cards
        HashSet<int> uniqueIndices = new HashSet<int>();

        while (uniqueIndices.Count < 10)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            uniqueIndices.Add(randomIndex);
        }

        foreach (int index in uniqueIndices)
        {
            deck.Add(allCards[index]);
        }
    }

    void DrawHand()
    {
        HashSet<int> usedIndices = new HashSet<int>();

        for (int i = 0; i < 5; i++)
        {
            int randomIndex;

            // Ensure the hand has unique cards
            do
            {
                randomIndex = Random.Range(0, deck.Count);
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);

            GameObject card = Instantiate(deck[randomIndex], handPositions[i].position, Quaternion.identity);
            var cardClickHandler = card.GetComponent<CardClickHandler>();
            cardClickHandler.confirmButton = confirmButton;  // Assign the confirm button
            hand.Add(card);
        }
    }
}
