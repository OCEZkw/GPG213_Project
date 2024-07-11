using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ViewOnlyCardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string cardName;
    public Sprite cardSprite;
    public string itemDescription;
    public Sprite emptySprite;
    [SerializeField] private Image cardImage;

    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    public GameObject selectedShader;

    private ViewOnlyInventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<ViewOnlyInventoryManager>();
    }

    public void UpdateSlot(Inventory.InventoryItem item)
    {
        if (item != null)
        {
            cardName = item.cardName;
            cardSprite = item.cardSprite;
            itemDescription = item.description;
            cardImage.sprite = cardSprite;
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

    private void OnHoverEnter()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        UpdateItemDescription();
    }

    private void OnHoverExit()
    {
        selectedShader.SetActive(false);
        ClearItemDescription();
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
        itemDescription = "";
        cardImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        selectedShader.SetActive(false);
    }
}