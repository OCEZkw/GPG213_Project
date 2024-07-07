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

    //===ITEM DESCRIPTION SLOT===//
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;


    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
    }

    public void AddItem(string cardName, int quantity, Sprite cardSprite, string itemDescription)
    {
        // Update NAME
        this.cardName = cardName;

        // Update QUANTITY
        this.quantity = quantity;

        // Update Image 
        this.cardSprite = cardSprite;

        // Update QUANTITY 
        this.itemDescription = itemDescription;
        isFull = true;

        cardImage.sprite = cardSprite;
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
        // Optionally, you can deselect the slot when the mouse exits
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
            this.quantity -= 1;
            if (this.quantity <= 0)
            {
                EmptySlot();
            }
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
        Debug.Log("EmptySlot called");

        // Clear all card-related data
        cardName = "";
        cardSprite = null;
        quantity = 0;
        isFull = false;
        itemDescription = "";

        // Update the UI to reflect the empty state
        cardImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;

        // Hide the selected shader and reset selection state
        selectedShader.SetActive(false);
        thisItemSelected = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
