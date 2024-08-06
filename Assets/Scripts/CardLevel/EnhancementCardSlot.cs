using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EnhancementCardSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image cardImage;
    public Image backgroundImage; // New background image
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardLevelText;
    public GameObject selectedShader;
    public GameObject hoverShader;
    [HideInInspector]
    public string cardName;
    [HideInInspector]
    public int cardLevel;
    [HideInInspector]
    public bool isSelected;
    private CardEnhancementManager enhancementManager;
    private LevelingInventory levelingInventory;
    public bool isMainSlot = false;

    void Start()
    {
        enhancementManager = FindObjectOfType<CardEnhancementManager>();
        levelingInventory = FindObjectOfType<LevelingInventory>();
        if (isMainSlot && enhancementManager != null)
        {
            enhancementManager.mainCardSlot = this;
        }
        ShowBackgroundImage(); // Show background image by default
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isSelected = !isSelected;
            selectedShader.SetActive(isSelected);
            hoverShader.SetActive(false);
            enhancementManager.OnSlotSelected(this);
            levelingInventory.ShowInventory(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            hoverShader.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            hoverShader.SetActive(false);
        }
    }

    public void UpdateSlot(Inventory.InventoryItem item, bool isMain)
    {
        cardName = item.cardName;
        cardLevel = item.level;
        isMainSlot = isMain;
        cardImage.sprite = item.cardSprite;
        cardNameText.text = item.cardName;
        cardLevelText.text = $"Lv. {item.level}";
        gameObject.SetActive(true);
        ShowCardImage(); // Show card image when updated
    }

    public void EmptySlot()
    {
        cardName = "";
        cardLevel = 0;
        isSelected = false;
        selectedShader.SetActive(false);
        hoverShader.SetActive(false);
        cardImage.sprite = null;
        cardNameText.text = "Empty";
        cardLevelText.text = "Lv. -";
        ShowBackgroundImage(); // Show background image when emptied
    }

    private void ShowBackgroundImage()
    {
        backgroundImage.gameObject.SetActive(true);
        cardImage.gameObject.SetActive(false);
    }

    private void ShowCardImage()
    {
        backgroundImage.gameObject.SetActive(false);
        cardImage.gameObject.SetActive(true);
    }
}