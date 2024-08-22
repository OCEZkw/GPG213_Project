using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckData : MonoBehaviour
{
    public static DeckData Instance { get; private set; }
    public List<CardSO> selectedCards = new List<CardSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("DeckData: Instance created and set to DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("DeckData: Duplicate instance destroyed");
        }
    }

    public void AddCard(CardSO card)
    {
        if (card == null)
        {
            Debug.LogError("DeckData: Attempted to add null card");
            return;
        }

        if (selectedCards.Count < 10)
        {
            selectedCards.Add(card);
            Debug.Log($"DeckData: Added card {card.cardName}. Total cards: {selectedCards.Count}");
        }
        else
        {
            Debug.LogWarning("DeckData: Cannot add more cards. Deck is full.");
        }
    }

    public void RemoveCard(CardSO card)
    {
        if (card == null)
        {
            Debug.LogError("DeckData: Attempted to remove null card");
            return;
        }

        if (selectedCards.Remove(card))
        {
            Debug.Log($"DeckData: Removed card {card.cardName}. Total cards: {selectedCards.Count}");
        }
        else
        {
            Debug.LogWarning($"DeckData: Card {card.cardName} not found in deck");
        }
    }

    public void ClearDeck()
    {
        int count = selectedCards.Count;
        selectedCards.Clear();
        Debug.Log($"DeckData: Cleared deck. Removed {count} cards.");
    }
}