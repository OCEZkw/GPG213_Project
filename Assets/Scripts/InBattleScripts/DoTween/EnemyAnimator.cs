using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine;
using DG.Tweening;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Floating Animation")]
    public float floatDistance = 0.5f;
    public float floatDuration = 1f;

    [Header("Attack Animation")]
    public float attackScaleFactor = 1.2f;
    public float attackScaleDuration = 0.5f;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;

        // Start the continuous floating animation
        StartFloatingAnimation();
    }

    private void StartFloatingAnimation()
    {
        // Create a sequence for continuous up and down movement
        Sequence floatSequence = DOTween.Sequence();

        floatSequence.Append(transform.DOMoveY(initialPosition.y + floatDistance, floatDuration).SetEase(Ease.InOutSine));
        floatSequence.Append(transform.DOMoveY(initialPosition.y, floatDuration).SetEase(Ease.InOutSine));

        // Set the sequence to loop indefinitely
        floatSequence.SetLoops(-1, LoopType.Restart);
    }

    public void PlayAttackAnimation()
    {
        // Pause the floating animation
        DOTween.Pause(transform);

        // Scale up
        transform.DOScale(initialScale * attackScaleFactor, attackScaleDuration)
            .OnComplete(() =>
            {
                // Scale back down
                transform.DOScale(initialScale, attackScaleDuration)
                    .OnComplete(() =>
                    {
                        // Resume the floating animation
                        DOTween.Play(transform);
                    });
            });
    }

    private void OnDestroy()
    {
        // Kill all tweens associated with this transform when the object is destroyed
        DOTween.Kill(transform);
    }
}
