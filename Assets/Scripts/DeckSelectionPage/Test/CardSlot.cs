using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //===ITEM DATA===//
    public string cardName;
    public Sprite cardSprite;
    public int quantity;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    //===ITEM SLOT===//
    [SerializeField] private Image cardImage;
    public int slotIndex;
    //===ITEM DESCRIPTION SLOT===//
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    public GameObject selectedShader;
    public bool thisItemSelected;
    private InventoryManager inventoryManager;

    public GameObject cardPrefab;
    public bool isSelectable = true;
    private bool isSelected = false;

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
    }

    public void UpdateSlot(Inventory.InventoryItem item, int index, bool selectable)
    {
        if (item != null)
        {
            cardName = item.cardName;
            cardSprite = item.cardSprite;
            itemDescription = item.description;
            isFull = true;
            cardImage.sprite = cardSprite;
            slotIndex = index;
            isSelectable = selectable;
            UpdateVisuals();
        }
        else
        {
            EmptySlot();
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
        if (isSelectable)
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            isSelected = true;
            UpdateVisuals();
            UpdateItemDescription();
        }
    }

    private void OnHoverExit()
    {
        selectedShader.SetActive(false);
        thisItemSelected = false;
        isSelected = false;
        UpdateVisuals();
        ClearItemDescription();
    }

    private void OnLeftClick()
    {
        if (thisItemSelected && isFull && isSelectable)
        {
            inventoryManager.PlaceCardOnSelectedDeckSlot(cardName, cardSprite, slotIndex);
            inventoryManager.HideInventoryMenu();
            inventoryManager.UpdateCardSelectability(cardName, slotIndex, false);
            inventoryManager.UpdateCardSlots();
        }
    }

    private void UpdateItemDescription()
    {
        ItemDescriptionNameText.text = cardName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = cardSprite ?? emptySprite;
    }

    private void ClearItemDescription()
    {
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }

    public void EmptySlot()
    {
        cardName = "";
        cardSprite = null;
        quantity = 0;
        isFull = false;
        itemDescription = "";
        cardImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        selectedShader.SetActive(false);
        thisItemSelected = false;
        slotIndex = -1;  // Reset the slot index
        isSelectable = true;
        isSelected = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (!isSelectable)
        {
            cardImage.color = Color.gray; // Darken the card if not selectable
        }
        else if (isSelected)
        {
            cardImage.color = new Color(0.8f, 0.8f, 0.8f); // Slightly darken when selected
        }
        else
        {
            cardImage.color = Color.white; // Reset to normal color
        }
    }
}
