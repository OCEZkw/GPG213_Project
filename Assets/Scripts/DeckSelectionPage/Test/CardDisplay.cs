using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;

    private CardSO cardData;

    public void Initialize(CardSO cardSO)
    {
        cardData = cardSO;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (cardData == null) return;

        if (cardImage != null) cardImage.sprite = cardData.cardSprite;
        if (cardNameText != null) cardNameText.text = cardData.cardName;
        if (descriptionText != null) descriptionText.text = cardData.description;
        if (levelText != null) levelText.text = $"Level: {cardData.level}";

        // You can add more fields here as needed
    }
}
