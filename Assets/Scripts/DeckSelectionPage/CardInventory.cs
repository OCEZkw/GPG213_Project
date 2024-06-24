using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    // Singleton instance
    public static CardInventory Instance { get; private set; }

    // List of all available cards
    public List<Card> allCards = new List<Card>();

    // Dictionary to store card IDs and their counts
    private Dictionary<int, int> cardCollection = new Dictionary<int, int>();

    // Reference to the inventory slot prefab
    public GameObject inventorySlotPrefab;

    // Reference to the inventory panel parent
    public Transform inventoryPanel;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadInventory();
        }
    }

    // Add a card to the inventory
    public void AddCard(int cardId)
    {
        if (cardCollection.ContainsKey(cardId))
        {
            cardCollection[cardId]++;
        }
        else
        {
            cardCollection[cardId] = 1;
        }

        Debug.Log($"Card {GetCardById(cardId).name} added. Total: {cardCollection[cardId]}");

        // Update UI after adding card
        UpdateInventoryUI();
    }

    // Remove a card from the inventory
    public void RemoveCard(int cardId)
    {
        if (cardCollection.ContainsKey(cardId))
        {
            cardCollection[cardId]--;
            if (cardCollection[cardId] <= 0)
            {
                cardCollection.Remove(cardId);
            }

            Debug.Log($"Card {GetCardById(cardId).name} removed. Total: {(cardCollection.ContainsKey(cardId) ? cardCollection[cardId].ToString() : "0")}");

            // Update UI after removing card
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log($"Card {cardId} not found in inventory.");
        }
    }

    // Check if a card is owned
    public bool HasCard(int cardId)
    {
        return cardCollection.ContainsKey(cardId) && cardCollection[cardId] > 0;
    }

    // Get the count of a specific card
    public int GetCardCount(int cardId)
    {
        return cardCollection.ContainsKey(cardId) ? cardCollection[cardId] : 0;
    }

    // Save the inventory to PlayerPrefs
    public void SaveInventory()
    {
        List<string> keys = new List<string>();
        foreach (var card in cardCollection)
        {
            PlayerPrefs.SetInt(card.Key.ToString(), card.Value);
            keys.Add(card.Key.ToString());
        }
        PlayerPrefs.SetString("CardInventoryKeys", string.Join(",", keys));
        PlayerPrefs.Save();
        Debug.Log("Inventory saved.");
    }

    // Load the inventory from PlayerPrefs
    public void LoadInventory()
    {
        cardCollection.Clear();

        foreach (var key in PlayerPrefsKeys())
        {
            int cardId = int.Parse(key);
            int count = PlayerPrefs.GetInt(key);
            cardCollection[cardId] = count;
        }

        Debug.Log("Inventory loaded.");

        // Update UI after loading inventory
        UpdateInventoryUI();
    }

    // Update the inventory UI
    private void UpdateInventoryUI()
    {
        // Clear existing slots
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // Populate new slots
        foreach (var card in allCards)
        {
            if (cardCollection.ContainsKey(card.id) && cardCollection[card.id] > 0)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel);
                InventorySlot slotScript = slot.GetComponent<InventorySlot>();
                slotScript.SetCard(card);
            }
        }
    }

    // Get all PlayerPrefs keys that are card IDs
    private List<string> PlayerPrefsKeys()
    {
        List<string> keys = new List<string>();
        string keysString = PlayerPrefs.GetString("CardInventoryKeys", "");
        if (!string.IsNullOrEmpty(keysString))
        {
            keys.AddRange(keysString.Split(','));
        }
        return keys;
    }

    // Get a card by its ID
    public Card GetCardById(int cardId)
    {
        return allCards.Find(card => card.id == cardId);
    }

    // Example usage
    private void Start()
    {
        // Optionally, load inventory on start
        LoadInventory();
    }

    private void OnApplicationQuit()
    {
        SaveInventory();
    }
}
