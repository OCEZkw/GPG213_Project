using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelingInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string cardName;
    public Sprite cardSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;

    [SerializeField] private Image cardImage;
    public int slotIndex;

    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    public GameObject selectedShader;
    public bool thisItemSelected;

    public bool isSelected = false;

    public LevelingInventory levelingInventory;
    private CardEnhancementManager cardEnhancementManager;

    void Start()
    {
        levelingInventory = GetComponentInParent<LevelingInventory>();
        cardEnhancementManager = FindObjectOfType<CardEnhancementManager>();
    }

    public void UpdateSlot(Inventory.InventoryItem item, int index, bool selected)
    {
        if (item != null)
        {
            cardName = item.cardName;
            cardSprite = item.cardSprite;
            itemDescription = item.description;
            isFull = true;
            cardImage.sprite = cardSprite;
            slotIndex = index;
            isSelected = selected;
            UpdateVisuals();
        }
        else
        {
            EmptySlot();
        }
    }

    private void UpdateVisuals()
    {
        if (isSelected)
        {
            cardImage.color = Color.gray; // Darken the card
        }
        else
        {
            cardImage.color = Color.white; // Reset to normal color
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
        selectedShader.SetActive(true);
        thisItemSelected = true;
        UpdateItemDescription();
    }

    private void OnHoverExit()
    {
        selectedShader.SetActive(false);
        thisItemSelected = false;
        ClearItemDescription();
    }

    private void OnLeftClick()
    {
        if (isFull && !isSelected)  // Only allow selection if the slot is full and not already selected
        {
            // Set the card as selected
            isSelected = true;

            Inventory.InventoryItem selectedItem = new Inventory.InventoryItem(cardName, cardSprite, itemDescription, 1, 0);

            cardEnhancementManager.UpdateSelectedSlot(selectedItem);
            levelingInventory.OnInventoryItemSelected(selectedItem, slotIndex);

            UpdateVisuals();
            levelingInventory.HideInventory();
        }
        else if (isSelected)
        {
            // Optionally, you can add feedback here to inform the user that the card is already selected
            Debug.Log($"Card {cardName} is already selected and cannot be selected again.");
        }
        selectedShader.SetActive(false);
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
        isFull = false;
        itemDescription = "";
        cardImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        selectedShader.SetActive(false);
        thisItemSelected = false;
        slotIndex = -1;
    }
}
