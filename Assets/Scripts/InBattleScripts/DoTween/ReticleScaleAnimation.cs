using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReticleScaleAnimation : MonoBehaviour
{
    [Header("Initial Shrink Animation")]
    public float startScale = 3f;
    public float endScale = 1f;
    public float initialShrinkDuration = 0.5f;
    public Ease initialShrinkEaseType = Ease.InOutQuad;

    [Header("Continuous Scale Animation")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float scaleDuration = 0.5f;

    [Header("Rotation Animation")]
    public float rotationDuration = 1f;
    public float rotationAngle = 360f;

    private Vector3 originalScale;
    private Sequence animationSequence;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayAnimation(bool isSelected)
    {
        if (animationSequence != null)
        {
            animationSequence.Kill();
        }

        if (isSelected)
        {
            // Create a new sequence
            animationSequence = DOTween.Sequence();

            // Set initial scale
            transform.localScale = originalScale * startScale;

            // Add initial shrink animation
            animationSequence.Append(transform.DOScale(originalScale * endScale, initialShrinkDuration)
                .SetEase(initialShrinkEaseType));

            // Add continuous scale animation
            animationSequence.Append(transform.DOScale(originalScale * maxScale, scaleDuration)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo));

            // Add rotation animation
            animationSequence.Join(transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental));

            // Play the sequence
            animationSequence.Play();
        }
        else
        {
            // Stop all animations and reset
            if (animationSequence != null)
            {
                animationSequence.Kill();
            }
            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnDisable()
    {
        if (animationSequence != null)
        {
            animationSequence.Kill();
        }
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
    }
}
