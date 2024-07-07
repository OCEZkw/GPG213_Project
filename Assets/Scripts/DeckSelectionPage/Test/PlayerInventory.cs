using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{
    public string cardName;
    public int quantity;
    public string itemDescription;
    public string spritePath;

    public InventoryItem(string cardName, int quantity, string itemDescription, string spritePath)
    {
        this.cardName = cardName;
        this.quantity = quantity;
        this.itemDescription = itemDescription;
        this.spritePath = spritePath;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public List<InventoryItem> items = new List<InventoryItem>();

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

    public void AddItem(string cardName, int quantity, string itemDescription, string spritePath)
    {
        InventoryItem existingItem = items.Find(item => item.cardName == cardName);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
            Debug.Log($"Updated quantity of {cardName} to {existingItem.quantity}");
        }
        else
        {
            items.Add(new InventoryItem(cardName, quantity, itemDescription, spritePath));
            Debug.Log($"Added new item: {cardName}, Quantity: {quantity}, SpritePath: {spritePath}");
        }
        InventoryManager.Instance.UpdateInventoryDisplay();
    }

    public void RemoveItem(string cardName, int quantity)
    {
        InventoryItem existingItem = items.Find(item => item.cardName == cardName);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
            // Notify InventoryManager to update display
            InventoryManager.Instance.UpdateInventoryDisplay();
        }
    }

    public InventoryItem GetItem(string cardName)
    {
        return items.Find(item => item.cardName == cardName);
    }
}
