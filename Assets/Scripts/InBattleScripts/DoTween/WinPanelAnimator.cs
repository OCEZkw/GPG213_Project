using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WinPanelAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float bounceStrength = 0.2f;

    public void AnimateText()
    {
        // Reset text scale
        winText.transform.localScale = Vector3.zero;

        // Animate text scale with bounce effect
        winText.transform.DOScale(1f, animationDuration)
            .SetEase(Ease.OutBounce);

        // Animate text color
        winText.DOColor(Color.white, animationDuration)
            .From(new Color(1f, 1f, 1f, 0f));

        // Add a subtle rotation animation
        winText.transform.DORotate(new Vector3(0f, 0f, 10f), animationDuration / 2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }
}
