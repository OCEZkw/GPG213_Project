using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    public Button singleSummonButton;
    public Button multiSummonButton;
    public Transform summonResultParent;
    public GameObject summonResultPrefab;

    private void Start()
    {
        singleSummonButton.onClick.AddListener(SingleSummon);
        multiSummonButton.onClick.AddListener(MultiSummon);
    }

    private void SingleSummon()
    {
        CardSO summonedCard = GachaSystem.Instance.SummonSingleCard();
        DisplaySummonedCard(summonedCard);
        GachaSystem.Instance.AddCardToInventory(summonedCard);
    }

    private void MultiSummon()
    {
        List<CardSO> summonedCards = GachaSystem.Instance.SummonMultipleCards(10);
        foreach (CardSO card in summonedCards)
        {
            DisplaySummonedCard(card);
            GachaSystem.Instance.AddCardToInventory(card);
        }
    }

    private void DisplaySummonedCard(CardSO card)
    {
        GameObject resultObject = Instantiate(summonResultPrefab, summonResultParent);
        Image cardImage = resultObject.GetComponent<Image>();
        Text cardNameText = resultObject.GetComponentInChildren<Text>();

        if (cardImage != null) cardImage.sprite = card.cardSprite;
        if (cardNameText != null) cardNameText.text = card.cardName;
        // You might want to add some effects or animations here
    }
}