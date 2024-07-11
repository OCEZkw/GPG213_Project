using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject DeckMenu;
    private bool menuActivated;
    public CardSlot[] cardSlot;
    public DeckSlot[] deckSlot;

    public CardSO[] cardSOs;
    public PlayerStats playerStats;

    private DeckSlot selectedDeckSlot;

    // Reference to the UIManager to enable the button
    private UIManager uiManager;

    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
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

    public void UpdateCardSlots()
    {
        if (inventory != null)
        {
            for (int i = 0; i < cardSlot.Length; i++)
            {
                if (i < inventory.items.Count)
                {
                    cardSlot[i].UpdateSlot(inventory.items[i], i);  // Pass the index
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
        for (int i = 0; i < cardSlot.Length; i++)
        {
            cardSlot[i].selectedShader.SetActive(false);
            cardSlot[i].thisItemSelected = false;
        }
        for (int i = 0; i < deckSlot.Length; i++)
        {
            deckSlot[i].selectedShader.SetActive(false);
            deckSlot[i].thisItemSelected = false;
        }
    }

    public void ShowInventoryMenu()
    {
        InventoryMenu.SetActive(true);
        DeckMenu.SetActive(false);
    }

    public void HideInventoryMenu()
    {
        InventoryMenu.SetActive(false);
        DeckMenu.SetActive(true);
    }

    public void SetSelectedDeckSlot(DeckSlot deckSlot)
    {
        selectedDeckSlot = deckSlot;
    }

    public void PlaceCardOnSelectedDeckSlot(string cardName, Sprite cardSprite)
    {
        if (selectedDeckSlot != null)
        {
            selectedDeckSlot.SetCard(cardName, cardSprite);
        }
    }

    public void RemoveCardEffect(CardSO cardSO)
    {
        if (cardSO != null && cardSO.statChanges != null)
        {
            foreach (StatChange change in cardSO.statChanges)
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
}
