using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GloveAnimation : MonoBehaviour
{
    public Image gloveImage;
    public float tapDuration = 0.5f;
    public float tapDistance = 10f;

    private Vector2 originalPosition;
    private Sequence tapSequence;

    private void Awake()
    {
        if (gloveImage == null)
        {
            gloveImage = GetComponent<Image>();
        }
        originalPosition = gloveImage.rectTransform.anchoredPosition;
    }

    public void StartTapAnimation()
    {
        // Kill any ongoing animations
        StopTapAnimation();

        // Reset position
        gloveImage.rectTransform.anchoredPosition = originalPosition;

        // Create the tapping sequence
        tapSequence = DOTween.Sequence();

        tapSequence.Append(gloveImage.rectTransform.DOAnchorPosY(originalPosition.y - tapDistance, tapDuration / 2)
            .SetEase(Ease.OutQuad));
        tapSequence.Append(gloveImage.rectTransform.DOAnchorPosY(originalPosition.y, tapDuration / 2)
            .SetEase(Ease.InQuad));

        // Set the sequence to loop indefinitely
        tapSequence.SetLoops(-1);

        // Play the sequence
        tapSequence.Play();
    }

    public void StopTapAnimation()
    {
        if (tapSequence != null && tapSequence.IsActive())
        {
            tapSequence.Kill();
        }
        gloveImage.rectTransform.anchoredPosition = originalPosition;
    }

    private void OnDisable()
    {
        StopTapAnimation();
    }
}
