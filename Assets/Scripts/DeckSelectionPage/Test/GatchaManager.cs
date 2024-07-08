using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class GachaManager : MonoBehaviour
{
    public Button singleSummonButton;
    public Button multiSummonButton;
    public Transform summonResultParent;
    public GameObject summonResultPrefab;
    public GameObject summonEffectPrefab;
    public float summonEffectDuration = 2f;

    private void Start()
    {
        Debug.Log("GachaManager Start method called");
        singleSummonButton.onClick.AddListener(SingleSummon);
        multiSummonButton.onClick.AddListener(MultiSummon);

    }

    private void SingleSummon()
    {
        if (GachaSystem.Instance != null)
        {
            CardSO summonedCard = GachaSystem.Instance.SummonSingleCard();
            StartCoroutine(DisplaySummonedCardWithEffect(summonedCard));
            GachaSystem.Instance.AddCardToInventory(summonedCard);
        }
        else
        {
            Debug.LogError("GachaSystem instance is null!");
        }
    }

    private void MultiSummon()
    {
        if (GachaSystem.Instance != null)
        {
            List<CardSO> summonedCards = GachaSystem.Instance.SummonMultipleCards(10);
            StartCoroutine(DisplayMultipleSummonedCardsWithEffect(summonedCards));
        }
        else
        {
            Debug.LogError("GachaSystem instance is null!");
        }
    }

    private IEnumerator DisplaySummonedCardWithEffect(CardSO card)
    {
        yield return StartCoroutine(PlaySummonEffect());
        DisplaySummonedCard(card);
    }

    private IEnumerator DisplayMultipleSummonedCardsWithEffect(List<CardSO> cards)
    {
        foreach (CardSO card in cards)
        {
            yield return StartCoroutine(DisplaySummonedCardWithEffect(card));
            GachaSystem.Instance.AddCardToInventory(card);
        }
    }

    private IEnumerator PlaySummonEffect()
    {
        GameObject effectObject = Instantiate(summonEffectPrefab, summonResultParent);
        Debug.Log("Effect instantiated at: " + effectObject.transform.position);

        VisualEffect visualEffect = effectObject.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Play();

            yield return new WaitForSeconds(summonEffectDuration);

            visualEffect.Stop();
        }
        else
        {
            Debug.LogWarning("No VisualEffect component found on the summon effect prefab.");
            yield return new WaitForSeconds(summonEffectDuration);
        }

        Destroy(effectObject);
    }

    private void DisplaySummonedCard(CardSO card)
    {
        GameObject resultObject = Instantiate(summonResultPrefab, summonResultParent);
        Image cardImage = resultObject.GetComponent<Image>();
        Text cardNameText = resultObject.GetComponentInChildren<Text>();
        if (cardImage != null) cardImage.sprite = card.cardSprite;
        if (cardNameText != null) cardNameText.text = card.cardName;
    }
}