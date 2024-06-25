using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    //===ITEM DATA===//
    public string cardName;
    public Sprite cardSprite;
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

    public void AddItem(string cardName, Sprite cardSprite, string itemDescription)
    {
        this.cardName = cardName;
        this.cardSprite = cardSprite;
        this.itemDescription = itemDescription;
        isFull = true;

        cardImage.sprite = cardSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        ItemDescriptionNameText.text = cardName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = cardSprite;
        if(itemDescriptionImage.sprite == null)
        { 
            itemDescriptionImage.sprite = emptySprite;
        } 
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
