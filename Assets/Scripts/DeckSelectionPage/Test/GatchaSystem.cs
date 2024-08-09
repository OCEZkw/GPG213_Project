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
                // Update the quest progress
                UpdateQuestProgress(1);
                summonedCards.Add(card);
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

        // Update the quest progress
        UpdateQuestProgress(summonedCards.Count);

        // Add the completed quests to the active quests list in the GameManager
        foreach (Quest1 quest in GameManager.Instance.availableQuests)
        {
            if (quest.goal.IsReached())
            {
                GameManager.Instance.activeQuests.Add(quest);
                GameManager.Instance.availableQuests.Remove(quest);
                quest.Complete();
            }
        }

        // Clear the summoned cards list
        summonedCards.Clear();
    }
    public List<CardSO> GetSummonedCards()
    {
        return summonedCards;
    }

    public void UpdateQuestProgress(int amount = 1)
    {
        List<Quest1> completedQuests = new List<Quest1>();

        foreach (Quest1 quest in GameManager.Instance.activeQuests)
        {
            if (quest.goal.goalType == GoalType.Summon)
            {
                quest.goal.CardSummoned(amount);
                if (quest.goal.IsReached())
                {
                    quest.Complete();
                    completedQuests.Add(quest);
                }
            }
        }

        // Now, after the loop, we can safely modify the activeQuests list
        foreach (Quest1 completedQuest in completedQuests)
        {
            GameManager.Instance.CompleteQuest(completedQuest);
        }
    }

}