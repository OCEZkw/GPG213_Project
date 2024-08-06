using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    public static GachaSystem Instance { get; private set; }
    private Inventory inventory;

    [System.Serializable]
    public class RarityRate
    {
        public CardSO.Rarity rarity;
        public float rate;
    }

    public List<CardSO> allCards = new List<CardSO>();
    public List<RarityRate> rarityRates = new List<RarityRate>();
    private List<CardSO> summonedCards = new List<CardSO>();

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
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory not found. Please assign it in the Inspector.");
            }
        }
    }

    public CardSO SummonSingleCard()
    {
        float randomValue = Random.value;
        float cumulativeRate = 0f;
        foreach (RarityRate rate in rarityRates)
        {
            cumulativeRate += rate.rate;
            if (randomValue <= cumulativeRate)
            {
                CardSO card = GetRandomCardOfRarity(rate.rarity);
                UpdateQuestProgress(1);
                return card;
            }
        }
        // Fallback to common if something goes wrong
        CardSO commonCard = GetRandomCardOfRarity(CardSO.Rarity.Common);
        UpdateQuestProgress(1);
        return commonCard;
    }

    public List<CardSO> SummonMultipleCards(int count)
    {
        List<CardSO> newSummonedCards = new List<CardSO>();
        for (int i = 0; i < count; i++)
        {
            newSummonedCards.Add(SummonSingleCard());
        }
        summonedCards = newSummonedCards;
        UpdateQuestProgress(count);
        return summonedCards;
    }

    private CardSO GetRandomCardOfRarity(CardSO.Rarity rarity)
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
        if (inventory != null)
        {
            inventory.AddItem(card.cardName, card.cardSprite, card.description);
            Debug.Log($"Added card to inventory: {card.cardName}, Rarity: {card.rarity}");
        }
        else
        {
            Debug.LogError("Cannot add card to inventory: Inventory is not set!");
        }
    }

    public void AddSummonedCardsToInventory()
    {
        foreach (CardSO card in summonedCards)
        {
            AddCardToInventory(card);
        }
        summonedCards.Clear();
    }

    public List<CardSO> GetSummonedCards()
    {
        return summonedCards;
    }



    public void UpdateQuestProgress(int summonCount)
    {
        Quest1 currentQuest = GameManager.Instance.LoadQuestData();
        if (currentQuest != null && currentQuest.isActive)
        {
            currentQuest.goal.CardSummoned(summonCount);
            if (currentQuest.goal.IsReached())
            {
                currentQuest.Complete();
            }
            GameManager.Instance.SaveQuestData(currentQuest);
        }
    }
}