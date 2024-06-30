using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isFull;
    public Image cardImage;
    public GameObject selectedShader;

    private InventoryManager inventoryManager;

    public bool thisItemSelected;

    public CardSO currentCardSO;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            inventoryManager.ShowInventoryMenu();
        }
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        inventoryManager.SetSelectedDeckSlot(this);
    }

    public void SetCard(string cardName, Sprite cardSprite)
    {
        // Remove the stat effect of the currently assigned card if any and add it back to the inventory
        if (currentCardSO != null)
        {
            inventoryManager.RemoveCardEffect(currentCardSO);
            inventoryManager.AddItem(currentCardSO.cardName, 1, currentCardSO.cardSprite, currentCardSO.description);
        }

        // Find the new card's CardSO
        currentCardSO = null;
        foreach (CardSO cardSO in inventoryManager.cardSOs)
        {
            if (cardSO.cardName == cardName)
            {
                currentCardSO = cardSO;
                break;
            }
        }

        // Assign the new card sprite and add its stat effect
        cardImage.sprite = cardSprite;
        if (currentCardSO != null)
        {
            inventoryManager.ApplyCardEffect(currentCardSO);
        }

        isFull = true; // Assuming this flag is set to true to indicate the slot is filled

        // Check if all deck slots are filled
        inventoryManager.CheckAllDeckSlotsFilled();
    }
}
