using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject DeckMenu;
    private bool menuActivated;
    public CardSlot[] cardSlot;
    public DeckSlot[] deckSlot;

    public CardSO[] cardSOs;
    private PlayerStats playerStats;

    private DeckSlot selectedDeckSlot;

    // Reference to the UIManager to enable the button
    private UIManager uiManager;

    public Inventory inventory;

    private Dictionary<(string, int), bool> cardSelectability = new Dictionary<(string, int), bool>();

    // Start is called before the first frame update
    void Start()
    {
        // Use the PlayerStats instance
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance not found!");
        }

        uiManager = FindObjectOfType<UIManager>();

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
        }

        if (DeckData.Instance == null)
        {
            new GameObject("DeckData").AddComponent<DeckData>();
            Debug.Log("InventoryManager: Created DeckData instance");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseCard(string cardName)
    {
        for (int i = 0; i < cardSOs.Length; i++)
        {
            if(cardSOs[i].cardName == cardName)
            {
                cardSOs[i].UseCard(playerStats);
            }
        }
    }

    public void UpdateCardSelectability(string cardName, int cardIndex, bool isSelectable)
    {
        cardSelectability[(cardName, cardIndex)] = isSelectable;
        UpdateCardSlots();
    }

    public void UpdateCardSlots()
    {
        if (inventory != null)
        {
            for (int i = 0; i < cardSlot.Length; i++)
            {
                if (i < inventory.items.Count)
                {
                    CardSO cardSO = GetCardSOByName(inventory.items[i].cardName);
                    bool isSelectable = !cardSelectability.ContainsKey((inventory.items[i].cardName, i)) || cardSelectability[(inventory.items[i].cardName, i)];
                    cardSlot[i].UpdateSlot(inventory.items[i], i, isSelectable, cardSO);
                }
                else
                {
                    cardSlot[i].EmptySlot();
                }
            }
        }
        else
        {
            Debug.LogError("Cannot update card slots: Inventory is null");
        }
    }

    private CardSO GetCardSOByName(string cardName)
    {
        return cardSOs.FirstOrDefault(card => card.cardName == cardName);
    }


    public void OnInventoryChanged()
    {
        UpdateCardSlots();
    }

    public void AddItem(string cardName, Sprite cardSprite, string itemDescription)
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.AddItem(cardName, cardSprite, itemDescription);
            UpdateCardSlots();
        }
        else
        {
            Debug.LogError("Cannot add item: Inventory instance is null");
        }
    }


    public void DeselectAllSlots()
    {
        DeselectAllCardSlots();
        DeselectAllDeckSlots();
    }

    public void DeselectAllCardSlots()
    {
        foreach (var slot in cardSlot)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }
    }

    public void DeselectAllDeckSlots()
    {
        foreach (var slot in deckSlot)
        {
            slot.Deselect();
        }
        selectedDeckSlot = null;
    }

    public void ShowInventoryMenu()
    {
        InventoryMenu.SetActive(true);
        Debug.Log("InventoryManager: Showing Inventory Menu");
    }

    public void HideInventoryMenu()
    {
        InventoryMenu.SetActive(false);
        DeckMenu.SetActive(true);
        Debug.Log("InventoryManager: Hiding Inventory Menu, Showing Deck Menu");
    }

    public void SetSelectedDeckSlot(DeckSlot deckSlot)
    {
        if (selectedDeckSlot != null)
        {
            selectedDeckSlot.Deselect();
        }
        selectedDeckSlot = deckSlot;
        Debug.Log($"InventoryManager: Set selected DeckSlot to {(deckSlot != null ? deckSlot.gameObject.name : "null")}");
    }


    public void PlaceCardOnSelectedDeckSlot(CardSO cardSO, Sprite cardSprite, int cardIndex)
    {
        Debug.Log($"InventoryManager: Attempting to place card on selected DeckSlot. SelectedDeckSlot is {(selectedDeckSlot != null ? "not null" : "null")}");
        if (selectedDeckSlot != null)
        {
            selectedDeckSlot.SetCard(cardSO, cardSprite, cardIndex);
            Debug.Log($"InventoryManager: Placed card {cardSO.cardName} on DeckSlot {selectedDeckSlot.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("InventoryManager: Cannot place card. No DeckSlot selected.");
        }
    }

    public void RemoveCardEffect(CardSO cardSO)
    {
        if (cardSO != null)
        {
            List<CardSO.StatChange> currentChanges = cardSO.GetCurrentStatChanges();
            foreach (CardSO.StatChange change in currentChanges)
            {
                playerStats.ReverseStatChange(change.statToChange, change.amountToChangeStat);
            }
        }
    }

    public void ApplyCardEffect(CardSO cardSO)
    {
        if (cardSO != null)
        {
            cardSO.UseCard(playerStats);
        }
    }

    // Method to check if all deck slots are filled
    public void CheckAllDeckSlotsFilled()
    {
        foreach (DeckSlot slot in deckSlot)
        {
            if (!slot.isFull)
            {
                return;
            }
        }
        // If all slots are filled, enable the button
        uiManager.EnableLevelLoadButton(true);
    }

    public bool IsDeckSlotSelected()
    {
        return selectedDeckSlot != null;
    }
}
