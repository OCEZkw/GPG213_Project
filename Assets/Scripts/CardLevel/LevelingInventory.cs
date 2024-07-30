using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelingInventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    public LevelingInventorySlot[] inventorySlots;
    private Inventory inventory;
    private EnhancementCardSlot currentEnhancementSlot;
    private CardEnhancementManager cardEnhancementManager;

   // private List<int> selectedSlotIndices = new List<int>();
    private Dictionary<string, List<int>> selectedCardIndices = new Dictionary<string, List<int>>();

    void Start()
    {
        inventory = Inventory.Instance;
        cardEnhancementManager = FindObjectOfType<CardEnhancementManager>();
        if (inventory == null)
        {
            Debug.LogError("Inventory instance not found!");
        }
        inventoryPanel.SetActive(false);
    }

    public void ShowInventory(EnhancementCardSlot enhancementSlot)
    {
        currentEnhancementSlot = enhancementSlot;
        UpdateInventorySlots();
        inventoryPanel.SetActive(true);
    }

    public void HideInventory()
    {
        inventoryPanel.SetActive(false);
    }

    private void UpdateInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                string cardName = inventory.items[i].cardName;
                bool isSelected = selectedCardIndices.ContainsKey(cardName) && selectedCardIndices[cardName].Contains(i);
                inventorySlots[i].UpdateSlot(inventory.items[i], i, isSelected);
            }
            else
            {
                inventorySlots[i].EmptySlot();
            }
        }
    }

    public void OnInventoryItemSelected(Inventory.InventoryItem selectedItem, int slotIndex)
    {
        if (currentEnhancementSlot != null)
        {
            if (currentEnhancementSlot.isMainSlot)
            {
                cardEnhancementManager.SetMainCard(selectedItem);
            }
            else
            {
                cardEnhancementManager.AddEnhancementCard(selectedItem);
            }
            currentEnhancementSlot.UpdateSlot(selectedItem, currentEnhancementSlot.isMainSlot);

            // Add the selected slot index to the list
            if (!selectedCardIndices.ContainsKey(selectedItem.cardName))
            {
                selectedCardIndices[selectedItem.cardName] = new List<int>();
            }
            selectedCardIndices[selectedItem.cardName].Add(slotIndex);

            UpdateInventorySlots(); // Update all slots to reflect the new selection
            HideInventory();
        }
    }

    public void ResetSelections()
    {
        selectedCardIndices.Clear();
        UpdateInventorySlots();
    }

    public void OnInventoryItemDeselected(string cardName)
    {
        if (selectedCardIndices.ContainsKey(cardName))
        {
            selectedCardIndices.Remove(cardName);
            UpdateInventorySlots();
        }
    }
}