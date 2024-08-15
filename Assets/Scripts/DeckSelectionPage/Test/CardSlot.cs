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

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
    }

    public void UpdateSlot(Inventory.InventoryItem item, int index)
    {
        if (item != null)
        {
            cardName = item.cardName;
            cardSprite = item.cardSprite;
            itemDescription = item.description;
            isFull = true;
            cardImage.sprite = cardSprite;
            slotIndex = index;  // Set the slot index
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
        inventoryManager.DeselectAllSlots();
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
        if (thisItemSelected && isFull)
        {
            inventoryManager.PlaceCardOnSelectedDeckSlot(cardName, cardSprite);
            inventoryManager.HideInventoryMenu();
            inventoryManager.inventory.RemoveItem(cardName);  // Changed this line
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
    }
}
