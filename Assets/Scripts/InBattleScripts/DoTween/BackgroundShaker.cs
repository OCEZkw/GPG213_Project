using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundShaker : MonoBehaviour
{
    public static BackgroundShaker Instance { get; private set; }

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.1f;
    public int shakeVibrato = 10;
    public float shakeRandomness = 90f;

    private Vector3 initialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        initialPosition = transform.position;
    }

    public void ShakeBackground()
    {
        // Kill any ongoing shake
        DOTween.Kill(transform);

        // Reset position before starting the shake
        transform.position = initialPosition;

        // Shake animation
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, false)
            .OnComplete(() =>
            {
                // Ensure the background returns to the initial position after shaking
                transform.position = initialPosition;
            });
    }

    private void OnDestroy()
    {
        // Kill all tweens associated with this transform when the object is destroyed
        DOTween.Kill(transform);
    }
}