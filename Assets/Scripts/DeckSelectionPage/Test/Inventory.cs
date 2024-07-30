using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    public event System.Action OnInventoryChanged;

    public CardSO[] cardSOs;


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

    public void UpdateItem(string cardName, int newLevel, int newExperience, string newDescription = null)
    {
        InventoryItem itemToUpdate = items.Find(item => item.cardName == cardName);
        if (itemToUpdate != null)
        {
            itemToUpdate.level = newLevel;
            itemToUpdate.experience = newExperience;
            if (newDescription != null)
            {
                itemToUpdate.description = newDescription;
            }
            UpdateHierarchy();
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Card {cardName} not found in inventory during update attempt.");
        }
    }


    [System.Serializable]
    public class InventoryItem
    {
        public string cardName;
        public Sprite cardSprite;
        public string description;
        public int level;
        public int experience;
        public InventoryItem(string name, Sprite sprite, string desc, int lvl = 1, int exp = 0)
        {
            cardName = name;
            cardSprite = sprite;
            description = desc;
            level = lvl;
            experience = exp;
        }
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string cardName, Sprite cardSprite, string itemDescription, int level = 1, int experience = 0)
    {
        items.Add(new InventoryItem(cardName, cardSprite, itemDescription, level, experience));
        UpdateHierarchy();
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(string cardName)
    {
        int index = items.FindIndex(item => item.cardName == cardName);
        if (index != -1)
        {
            items.RemoveAt(index);
            UpdateHierarchy();
            OnInventoryChanged?.Invoke();
        }
    }

    private void UpdateHierarchy()
    {
        // Clear existing child objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Create new child objects for each item
        foreach (InventoryItem item in items)
        {
            GameObject itemObject = new GameObject(item.cardName);
            itemObject.transform.SetParent(transform);
            itemObject.AddComponent<InventoryItemVisualizer>().Initialize(item);
        }
    }
}
