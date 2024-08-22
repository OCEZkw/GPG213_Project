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

    // Audio-related fields
    private AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public CardSO cardSO;

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void UpdateSlot(Inventory.InventoryItem item, int index, bool selectable, CardSO cardSO)
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
            this.cardSO = cardSO;
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
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleClick();
            //OnLeftClick();
        }
    }

    private void OnHoverEnter()
    {
        Debug.Log("CardSlot: OnHoverEnter");
        if (isSelectable)
        {
            inventoryManager.DeselectAllCardSlots(); // Changed from DeselectAllSlots
            selectedShader.SetActive(true);
            thisItemSelected = true;
            isSelected = true;
            UpdateVisuals();
            UpdateItemDescription();
            Debug.Log($"CardSlot: Selected CardSlot {gameObject.name}");
        }
        else
        {
            Debug.Log($"CardSlot: {gameObject.name} is not selectable");
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
        Debug.Log("CardSlot: OnLeftClick");
        if (thisItemSelected && isFull && isSelectable)
        {
            Debug.Log($"CardSlot: Attempting to place card {cardName} on selected DeckSlot");
            inventoryManager.PlaceCardOnSelectedDeckSlot(cardSO, cardSprite, slotIndex);
            inventoryManager.HideInventoryMenu();
            inventoryManager.UpdateCardSelectability(cardName, slotIndex, false);
            inventoryManager.UpdateCardSlots();
        }
        else
        {
            Debug.Log($"CardSlot: Cannot place card. thisItemSelected: {thisItemSelected}, isFull: {isFull}, isSelectable: {isSelectable}");
        }
    }

    private void HandleClick()
    {
        if (thisItemSelected && isFull && isSelectable)
        {
            PlaySound(clickSound);

            inventoryManager.PlaceCardOnSelectedDeckSlot(cardSO, cardSprite, slotIndex);

            // Use Invoke to slightly delay hiding the menu
            Invoke(nameof(DelayedHideMenu), 0.1f);

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

    private void DelayedHideMenu()
    {
        inventoryManager.HideInventoryMenu();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing on CardSlot: " + gameObject.name);
        }
    }
}
