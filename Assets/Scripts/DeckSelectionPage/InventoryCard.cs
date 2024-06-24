using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCard : MonoBehaviour, IPointerClickHandler
{
    public Card card;
    private DeckSelectionManager deckSelectionManager;
    public Image cardImage; // UI Image to display the card
    public Text cardName; // UI Text to display the card name

    void Start()
    {
        deckSelectionManager = FindObjectOfType<DeckSelectionManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            deckSelectionManager.OnInventoryCardClick(card);
        }
    }

    public void UpdateCardDisplay()
    {
        cardImage.sprite = card.sprite; // Assign the card's sprite
        cardName.text = card.name; // Assign the card's name
    }
}
