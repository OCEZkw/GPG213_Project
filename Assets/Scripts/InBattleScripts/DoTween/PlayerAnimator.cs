using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Damage Shake Animation")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.3f;
    public int shakeVibrato = 10;
    public float shakeRandomness = 90f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void PlayDamageAnimation()
    {
        // Kill any ongoing animations
        DOTween.Kill(transform);

        // Reset position before starting the shake animation
        transform.position = initialPosition;

        // Shake animation
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, false)
            .OnComplete(() =>
            {
                // Ensure the player returns to the initial position after shaking
                transform.position = initialPosition;
            });
    }

    private void OnDestroy()
    {
        // Kill all tweens associated with this transform when the object is destroyed
        DOTween.Kill(transform);
    }
}
