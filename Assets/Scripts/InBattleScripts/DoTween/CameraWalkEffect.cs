using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraWalkEffect : MonoBehaviour
{
    [SerializeField] private float stepUpDistance = 0.1f;
    [SerializeField] private float stepDuration = 0.25f;
    [SerializeField] private Ease stepEase = Ease.InOutSine;

    private Vector3 originalPosition;
    private Tween currentTween;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void PlayStepEffect()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        currentTween = DOTween.Sequence()
            .Append(transform.DOLocalMoveY(originalPosition.y + stepUpDistance, stepDuration / 2).SetEase(stepEase))
            .Append(transform.DOLocalMoveY(originalPosition.y, stepDuration / 2).SetEase(stepEase));
    }

    public IEnumerator PlayStepSequence(int steps, float delayBetweenSteps)
    {
        for (int i = 0; i < steps; i++)
        {
            PlayStepEffect();
            yield return new WaitForSeconds(stepDuration + delayBetweenSteps);
        }
    }
}