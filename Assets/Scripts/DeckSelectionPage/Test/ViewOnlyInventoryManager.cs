using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewOnlyInventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public ViewOnlyCardSlot[] cardSlots;
    public Inventory inventory;

    void Start()
    {
        inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogError("Inventory instance not found!");
        }
        else
        {
            // Subscribe to the OnInventoryChanged event
            inventory.OnInventoryChanged += UpdateCardSlots;
            UpdateCardSlots();
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
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
                    cardSlots[i].UpdateSlot(inventory.items[i]);
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

    public void DeselectAllSlots()
    {
        foreach (ViewOnlyCardSlot slot in cardSlots)
        {
            slot.selectedShader.SetActive(false);
        }
    }

    public void ShowInventoryMenu()
    {
        InventoryMenu.SetActive(true);
        UpdateCardSlots();
    }

    public void HideInventoryMenu()
    {
        InventoryMenu.SetActive(false);
    }
}
