using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardEnhancementManager : MonoBehaviour
{
    public EnhancementCardSlot mainCardSlot;
    public EnhancementCardSlot[] enhancementCardSlots;
    public Button enhanceButton;
    public TextMeshProUGUI mainCardLevelText;
    public TextMeshProUGUI mainCardExpText;
    public EnhancementCardSlot selectedSlot;

    private CardSO mainCard;
    private List<CardSO> enhancementCards = new List<CardSO>();

    private Inventory inventory;
    private LevelingInventory levelingInventory;

    public TextMeshProUGUI[] statChangeTexts = new TextMeshProUGUI[6];
    private int[] statChanges = new int[6];

    void Awake()
    {
        SetupCardSlots();
    }

    void SetupCardSlots()
    {
        EnhancementCardSlot[] allSlots = GetComponentsInChildren<EnhancementCardSlot>();

        // Find the main slot
        mainCardSlot = System.Array.Find(allSlots, slot => slot.isMainSlot);

        if (mainCardSlot == null)
        {
            Debug.LogError("No main card slot found. Please set isMainSlot to true for one of the EnhancementCardSlots.");
            return;
        }

        // Set up enhancement slots
        enhancementCardSlots = System.Array.FindAll(allSlots, slot => !slot.isMainSlot);

        if (enhancementCardSlots.Length != 2)
        {
            Debug.LogWarning($"Expected 2 enhancement slots, found {enhancementCardSlots.Length}.");
        }
    }

    void Start()
    {
        levelingInventory = FindObjectOfType<LevelingInventory>();
        inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogError("Inventory instance not found!");
        }

        enhanceButton.onClick.AddListener(EnhanceCard);
        UpdateCardSlots();
        UpdateStatChangeTexts();
    }

    public void OnSlotSelected(EnhancementCardSlot slot)
    {
        if (selectedSlot != null && selectedSlot != slot)
        {
            selectedSlot.isSelected = false;
            selectedSlot.selectedShader.SetActive(false);
        }
        selectedSlot = slot;
        selectedSlot.isSelected = true;
        selectedSlot.selectedShader.SetActive(true);
    }

    public void UpdateSelectedSlot(Inventory.InventoryItem item)
    {
        if (selectedSlot == null)
        {
            Debug.LogWarning("No slot selected");
            return;
        }

        // Deselect the previous card if it exists
        if (!string.IsNullOrEmpty(selectedSlot.cardName))
        {
            levelingInventory.OnInventoryItemDeselected(selectedSlot.cardName);
        }

        if (selectedSlot.isMainSlot)
        {
            SetMainCard(item);
        }
        else
        {
            int slotIndex = System.Array.IndexOf(enhancementCardSlots, selectedSlot);
            if (slotIndex != -1)
            {
                AddEnhancementCard(item, slotIndex);
            }
            else
            {
                Debug.LogWarning("Selected slot not found in enhancement slots array");
            }
        }

        selectedSlot.UpdateSlot(item, selectedSlot.isMainSlot);
        UpdateCardSlots();
    }

    // Update this method to take a slot index
    public void AddEnhancementCard(Inventory.InventoryItem item, int slotIndex)
    {
        CardSO card = System.Array.Find(inventory.cardSOs, c => c.cardName == item.cardName);
        if (card != null)
        {
            // If the slot already has a card, remove it
            if (slotIndex < enhancementCards.Count && enhancementCards[slotIndex] != null)
            {
                enhancementCards[slotIndex] = card;
            }
            else if (slotIndex >= enhancementCards.Count)
            {
                // Add null elements until we reach the desired index
                while (enhancementCards.Count <= slotIndex)
                {
                    enhancementCards.Add(null);
                }
                enhancementCards[slotIndex] = card;
            }
            else
            {
                enhancementCards[slotIndex] = card;
            }
            UpdateCardSlots();
        }
    }

    public void UpdateCardSlots()
    {
        // Update main card slot
        if (mainCard != null)
        {
            mainCardSlot.UpdateSlot(new Inventory.InventoryItem(mainCard.cardName, mainCard.cardSprite, mainCard.description, mainCard.level, mainCard.experience), true);
        }
        else
        {
            mainCardSlot.EmptySlot();
        }

        // Update enhancement card slots
        for (int i = 0; i < enhancementCardSlots.Length; i++)
        {
            if (i < enhancementCards.Count && enhancementCards[i] != null)
            {
                enhancementCardSlots[i].UpdateSlot(new Inventory.InventoryItem(enhancementCards[i].cardName, enhancementCards[i].cardSprite, enhancementCards[i].description, enhancementCards[i].level, enhancementCards[i].experience), false);
            }
            else
            {
                enhancementCardSlots[i].EmptySlot();
            }
        }

        UpdateMainCardInfo();
    }

    public void SetMainCard(Inventory.InventoryItem item)
    {
        mainCard = System.Array.Find(inventory.cardSOs, card => card.cardName == item.cardName);
        UpdateCardSlots();
        UpdateStatChangeTexts();  // Add this line
    }

    public void AddEnhancementCard(Inventory.InventoryItem item)
    {
        CardSO card = System.Array.Find(inventory.cardSOs, c => c.cardName == item.cardName);
        if (card != null && !enhancementCards.Contains(card))
        {
            enhancementCards.Add(card);
            UpdateCardSlots();
        }
    }

    public void RemoveEnhancementCard(string cardName)
    {
        enhancementCards.RemoveAll(card => card.cardName == cardName);
    }

    private void UpdateMainCardInfo()
    {
        if (mainCard != null)
        {
            mainCardLevelText.text = $"Level: {mainCard.level}";
            mainCardExpText.text = $"EXP: {mainCard.experience}/{mainCard.experienceToNextLevel}";
        }
        else
        {
            mainCardLevelText.text = "Level: -";
            mainCardExpText.text = "EXP: -/-";
        }
    }

    private void EnhanceCard()
    {
        if (mainCard == null || enhancementCards.Count == 0)
        {
            Debug.Log("Please select a main card and at least one enhancement card.");
            return;
        }

        int totalExp = 0;
        foreach (var card in enhancementCards)
        {
            totalExp += CalculateExpGain(card);
            inventory.RemoveItem(card.cardName);
        }

        int oldLevel = mainCard.level;
        mainCard.GainExperience(totalExp);

        // Update the inventory
        UpdateInventoryAfterEnhancement(mainCard, oldLevel);

        enhancementCards.Clear();

        UpdateCardSlots();
        UpdateMainCardInfo();
        UpdateStatChangeTexts();
    }

    private void UpdateInventoryAfterEnhancement(CardSO enhancedCard, int oldLevel)
    {
        if (inventory != null)
        {
            string newDescription = enhancedCard.level > oldLevel ? enhancedCard.description : null;
            inventory.UpdateItem(enhancedCard.cardName, enhancedCard.level, enhancedCard.experience, newDescription);
        }
        else
        {
            Debug.LogError("Inventory is null when trying to update after enhancement.");
        }
    }

    public void UpdateSingleSlot(EnhancementCardSlot slot, Inventory.InventoryItem item)
    {
        if (slot.isMainSlot)
        {
            mainCard = System.Array.Find(inventory.cardSOs, card => card.cardName == item.cardName);
            slot.UpdateSlot(item, true);
        }
        else
        {
            CardSO card = System.Array.Find(inventory.cardSOs, c => c.cardName == item.cardName);
            if (card != null)
            {
                // Remove any existing card in this slot
                enhancementCards.RemoveAll(c => c.cardName == slot.cardName);
                // Add the new card
                enhancementCards.Add(card);
                slot.UpdateSlot(item, false);
            }
        }
        UpdateMainCardInfo();
    }

    private int CalculateExpGain(CardSO card)
    {
        // This is a simple calculation, you might want to make it more complex
        return 100 * card.level;
    }

    private void UpdateStatChangeTexts()
    {
        // Initialize all stat changes to 0
        for (int i = 0; i < statChanges.Length; i++)
        {
            statChanges[i] = 0;
        }

        if (mainCard != null)
        {
            List<CardSO.StatChange> currentChanges = mainCard.GetCurrentStatChanges();

            foreach (var change in currentChanges)
            {
                int statIndex = (int)change.statToChange;
                if (statIndex >= 0 && statIndex < statChanges.Length)
                {
                    statChanges[statIndex] = change.amountToChangeStat;
                }
            }
        }

        // Update the text displays
        for (int i = 0; i < statChangeTexts.Length; i++)
        {
            statChangeTexts[i].text = statChanges[i].ToString();
        }
    }
}
