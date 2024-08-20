using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReticleAnimation : MonoBehaviour
{
    public float rotationDuration = 1f;
    public float scaleDuration = 0.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float rotationAngle = 360f;

    private void Start()
    {
        // Create a sequence for combining animations
        Sequence sequence = DOTween.Sequence();

        // Rotation animation
        sequence.Append(transform.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental));

        // Scale in-and-out animation
        sequence.Join(transform.DOScale(maxScale, scaleDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo));

        // Play the sequence
        sequence.Play();
    }
}