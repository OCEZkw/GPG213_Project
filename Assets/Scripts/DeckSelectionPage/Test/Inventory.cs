using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    public event System.Action OnInventoryChanged;


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


    [System.Serializable]
    public class InventoryItem
    {
        public string cardName;
        public Sprite cardSprite;
        public string description;

        public InventoryItem(string name, Sprite sprite, string desc)
        {
            cardName = name;
            cardSprite = sprite;
            description = desc;
        }
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string cardName, Sprite cardSprite, string itemDescription)
    {
        items.Add(new InventoryItem(cardName, cardSprite, itemDescription));
        UpdateHierarchy();
        // Trigger the event when an item is added
        OnInventoryChanged?.Invoke();
    }


    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            UpdateHierarchy();
            // Trigger the event when an item is removed
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
