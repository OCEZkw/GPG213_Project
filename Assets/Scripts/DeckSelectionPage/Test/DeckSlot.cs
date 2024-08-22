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
    private string previousCardName;
    private int previousCardIndex = -1;

    // Audio-related fields
    private AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private bool isSelected = false;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (DeckData.Instance == null)
        {
            new GameObject("DeckData").AddComponent<DeckData>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (cardImage == null)
        {
            cardImage = GetComponent<Image>();
            if (cardImage == null)
            {
                cardImage = gameObject.AddComponent<Image>();
            }
        }

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
        selectedShader.SetActive(true);
        Debug.Log($"DeckSlot: Mouse entered {gameObject.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            selectedShader.SetActive(false);
        }
        Debug.Log($"DeckSlot: Mouse exited {gameObject.name}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PlaySound(clickSound);
            inventoryManager.DeselectAllDeckSlots(); // Deselect other DeckSlots
            Select();
            inventoryManager.ShowInventoryMenu();
        }
    }

    public void Select()
    {
        isSelected = true;
        selectedShader.SetActive(true);
        inventoryManager.SetSelectedDeckSlot(this);
        Debug.Log($"DeckSlot: Selected {gameObject.name}");
    }

    public void Deselect()
    {
        isSelected = false;
        selectedShader.SetActive(false);
        Debug.Log($"DeckSlot: Deselected {gameObject.name}");
    }

    private void OnHoverEnter()
    {
        Debug.Log("DeckSlot: OnHoverEnter");
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        inventoryManager.SetSelectedDeckSlot(this);
        Debug.Log($"DeckSlot: Selected DeckSlot {gameObject.name}");
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
        inventoryManager.ShowInventoryMenu();
        Debug.Log("DeckSlot: Showing Inventory Menu");
    }

    public void SetCard(CardSO cardSO, Sprite cardSprite, int cardIndex)
    {
        Debug.Log($"DeckSlot.SetCard called with cardSO: {(cardSO != null ? cardSO.cardName : "null")}, cardSprite: {(cardSprite != null ? "valid" : "null")}, cardIndex: {cardIndex}");

        if (cardSO == null)
        {
            Debug.LogError("DeckSlot.SetCard: cardSO is null");
            return;
        }

        if (DeckData.Instance == null)
        {
            Debug.LogError("DeckSlot.SetCard: DeckData.Instance is null");
            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogError("DeckSlot.SetCard: inventoryManager is null");
            return;
        }

        if (previousCardIndex != -1)
        {
            inventoryManager.UpdateCardSelectability(previousCardName, previousCardIndex, true);
        }

        if (currentCardSO != null)
        {
            DeckData.Instance.RemoveCard(currentCardSO);
            inventoryManager.RemoveCardEffect(currentCardSO);
        }

        currentCardSO = cardSO;

        if (cardImage != null)
        {
            cardImage.sprite = cardSprite;
        }
        else
        {
            Debug.LogWarning("DeckSlot.SetCard: cardImage is null");
        }

        DeckData.Instance.AddCard(currentCardSO);
        inventoryManager.ApplyCardEffect(currentCardSO);
        inventoryManager.UpdateCardSelectability(currentCardSO.cardName, cardIndex, false);
        previousCardName = currentCardSO.cardName;
        previousCardIndex = cardIndex;

        isFull = true;
        inventoryManager.CheckAllDeckSlotsFilled();

        Debug.Log($"DeckSlot.SetCard completed for {cardSO.cardName}");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

