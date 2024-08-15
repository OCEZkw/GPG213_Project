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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCard(CardSO card)
    {
        if (selectedCards.Count < 10)
        {
            selectedCards.Add(card);
        }
    }

    public void RemoveCard(CardSO card)
    {
        selectedCards.Remove(card);
    }

    public void ClearDeck()
    {
        selectedCards.Clear();
    }
}
