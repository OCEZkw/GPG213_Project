using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardFloatManager : MonoBehaviour
{
    public static CardFloatManager Instance { get; private set; }

    [SerializeField] private float floatDistance = 10f;
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private Ease floatEase = Ease.InOutSine;

    private List<CardFloatEffect> cardEffects = new List<CardFloatEffect>();
    private Sequence masterSequence;
    private float currentOffset = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartFloatingEffect();
    }

    public void RegisterCard(CardFloatEffect cardEffect)
    {
        if (!cardEffects.Contains(cardEffect))
        {
            cardEffects.Add(cardEffect);
        }
    }

    public void UnregisterCard(CardFloatEffect cardEffect)
    {
        cardEffects.Remove(cardEffect);
    }

    private void StartFloatingEffect()
    {
        masterSequence = DOTween.Sequence();
        masterSequence.Append(DOVirtual.Float(0, floatDistance, floatDuration / 2, UpdateOffset).SetEase(floatEase));
        masterSequence.Append(DOVirtual.Float(floatDistance, 0, floatDuration / 2, UpdateOffset).SetEase(floatEase));
        masterSequence.SetLoops(-1, LoopType.Restart);
    }

    private void UpdateOffset(float value)
    {
        currentOffset = value;
        UpdateAllCardPositions();
    }

    private void UpdateAllCardPositions()
    {
        foreach (var cardEffect in cardEffects)
        {
            cardEffect.UpdatePosition(currentOffset);
        }
    }

    private void OnDestroy()
    {
        if (masterSequence != null)
        {
            masterSequence.Kill();
        }
    }
}
