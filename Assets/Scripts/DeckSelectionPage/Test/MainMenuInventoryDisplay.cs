using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuInventoryDisplay : MonoBehaviour
{
    public MainMenuCardSlot[] cardSlots;
    private Inventory inventory;

    void Start()
    {
        // Find the Inventory instance
        inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogError("Inventory instance not found!");
        }
        else
        {
            // Initial update of card slots
            UpdateCardSlots();
            // Subscribe to inventory changes
            inventory.OnInventoryChanged += UpdateCardSlots;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from inventory changes when this object is destroyed
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateCardSlots;
        }
    }

    public void UpdateCardSlots()
    {
        if (inventory != null)
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (i < inventory.items.Count)
                {
                    cardSlots[i].UpdateSlot(inventory.items[i], i);
                }
                else
                {
                    cardSlots[i].EmptySlot();
                }
            }
        }
        else
        {
            Debug.LogError("Cannot update card slots: Inventory is null");
        }
    }
}
