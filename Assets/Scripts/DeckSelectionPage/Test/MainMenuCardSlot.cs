using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuCardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string cardName;
    public Sprite cardSprite;
    public string itemDescription;
    public Sprite emptySprite;

    [SerializeField] private Image cardImage;
    public int slotIndex;

    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    public GameObject selectedShader;

    public void UpdateSlot(Inventory.InventoryItem item, int index)
    {
        if (item != null)
        {
            cardName = item.cardName;
            cardSprite = item.cardSprite;
            itemDescription = item.description;
            cardImage.sprite = cardSprite;
            slotIndex = index;
        }
        else
        {
            EmptySlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectedShader.SetActive(true);
        UpdateItemDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
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
        slotIndex = -1;
    }
}