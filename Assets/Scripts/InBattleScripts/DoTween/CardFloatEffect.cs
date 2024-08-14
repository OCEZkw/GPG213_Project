using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardFloatEffect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 handPosition;
    private NewCardClick cardClick;
    private bool isSelected = false;
    private float selectionOffset = 0f;
    private bool isInitialized = false;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cardClick = GetComponent<NewCardClick>();
        CardFloatManager.Instance.RegisterCard(this);
    }

    public void Initialize(Vector2 position)
    {
        handPosition = position;
        isInitialized = true;
        UpdatePosition(0);
    }

    public void SetSelected(bool selected)
    {
        if (!isInitialized) return;

        isSelected = selected;
        float targetOffset = isSelected ? cardClick.moveDistance : 0f;
        DOTween.To(() => selectionOffset, x => selectionOffset = x, targetOffset, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() => UpdatePosition(0));
    }

    public void UpdatePosition(float yOffset)
    {
        if (!isInitialized) return;

        Vector2 targetPosition = new Vector2(handPosition.x, handPosition.y + selectionOffset + yOffset);
        rectTransform.anchoredPosition = targetPosition;
    }

    private void OnDestroy()
    {
        CardFloatManager.Instance.UnregisterCard(this);
    }
}