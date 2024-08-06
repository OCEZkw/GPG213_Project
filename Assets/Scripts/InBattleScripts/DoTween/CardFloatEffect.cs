using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardFloatEffect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private NewCardClick cardClick;
    private bool isSelected = false;
    private float selectionOffset = 0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        cardClick = GetComponent<NewCardClick>();
        startPosition = rectTransform.anchoredPosition;
        CardFloatManager.Instance.RegisterCard(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        float targetOffset = isSelected ? cardClick.moveDistance : 0f;

        // Animate the selection offset
        DOTween.To(() => selectionOffset, x => selectionOffset = x, targetOffset, 0.2f)
            .SetEase(Ease.OutQuad);
    }

    public void UpdatePosition(float yOffset)
    {
        Vector2 targetPosition = new Vector2(startPosition.x, startPosition.y + selectionOffset + yOffset);
        rectTransform.anchoredPosition = targetPosition;
    }

    private void OnDestroy()
    {
        CardFloatManager.Instance.UnregisterCard(this);
    }
}