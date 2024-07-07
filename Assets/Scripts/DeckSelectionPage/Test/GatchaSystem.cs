using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public static GachaSystem Instance { get; private set; }

    private InventoryManager inventoryManager;

    [System.Serializable]
    public class RarityRate
    {
        public CardSO.Rarity rarity;
        public float rate;
    }

    public List<CardSO> allCards = new List<CardSO>(); // Changed from Card to CardSO
    public List<RarityRate> rarityRates = new List<RarityRate>();

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

    private void Start()
    {
        // Find the InventoryManager component on the Canvas
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found on Canvas!");
        }
    }

    public CardSO SummonSingleCard() // Changed return type from Card to CardSO
    {
        float randomValue = Random.value;
        float cumulativeRate = 0f;

        foreach (RarityRate rate in rarityRates)
        {
            cumulativeRate += rate.rate;
            if (randomValue <= cumulativeRate)
            {
                return GetRandomCardOfRarity(rate.rarity);
            }
        }

        // Fallback to common if something goes wrong
        return GetRandomCardOfRarity(CardSO.Rarity.Common);
    }

    public List<CardSO> SummonMultipleCards(int count) // Changed return type from Card to CardSO
    {
        List<CardSO> summonedCards = new List<CardSO>();
        for (int i = 0; i < count; i++)
        {
            summonedCards.Add(SummonSingleCard());
        }
        return summonedCards;
    }

    private CardSO GetRandomCardOfRarity(CardSO.Rarity rarity) // Changed return type from Card to CardSO
    {
        List<CardSO> cardsOfRarity = allCards.FindAll(card => card.rarity == rarity);
        if (cardsOfRarity.Count > 0)
        {
            return cardsOfRarity[Random.Range(0, cardsOfRarity.Count)];
        }
        return null;
    }

    public void AddCardToInventory(CardSO card)
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(card.cardName, 1, card.cardSprite, card.description);
            Debug.Log($"Added card to inventory: {card.cardName}, Rarity: {card.rarity}");
        }
        else
        {
            Debug.LogError("Cannot add card to inventory: InventoryManager is not set!");
        }
    }
}