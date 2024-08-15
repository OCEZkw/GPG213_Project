using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        if (DeckData.Instance == null)
        {
            new GameObject("DeckData").AddComponent<DeckData>();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    private void OnHoverEnter()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        inventoryManager.SetSelectedDeckSlot(this);
    }

    private void OnHoverExit()
    {

        // Optionally, you can deselect the slot when the mouse exits
        selectedShader.SetActive(false);
        thisItemSelected = false;
        inventoryManager.SetSelectedDeckSlot(null);
    }

    private void OnLeftClick()
    {
        if (thisItemSelected)
        {
            inventoryManager.ShowInventoryMenu();
        }
    }

    public void SetCard(string cardName, Sprite cardSprite)
    {
        // Remove the current card from DeckData if any
        if (currentCardSO != null)
        {
            DeckData.Instance.RemoveCard(currentCardSO);
            inventoryManager.RemoveCardEffect(currentCardSO);
            inventoryManager.AddItem(currentCardSO.cardName, currentCardSO.cardSprite, currentCardSO.description);
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
            DeckData.Instance.AddCard(currentCardSO);
            inventoryManager.ApplyCardEffect(currentCardSO);
        }

        isFull = true;
        inventoryManager.CheckAllDeckSlotsFilled();
    }
}
