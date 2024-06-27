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

    // Start is called before the first frame update
    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
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

    public void AddItem(string cardName, int quantity, Sprite cardSprite, string itemDescription)
    {
        for (int i = 0; i < cardSlot.Length; i++)
        {
            if(cardSlot[i].isFull == false)
            {
                cardSlot[i].AddItem(cardName, quantity, cardSprite, itemDescription);
                return;
            }
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

}
