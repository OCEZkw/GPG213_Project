using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image cardImage; // UI Image to display the card
    public Text cardName; // UI Text to display the card name

    public void SetCard(Card card)
    {
        cardImage.sprite = card.sprite; // Assign the card's sprite
        cardName.text = card.name; // Assign the card's name
    }
}
