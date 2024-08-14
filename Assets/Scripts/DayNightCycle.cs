using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private float cycleDuration = 240f; // Duration of a full day/night cycle in seconds
    [SerializeField] private Gradient colorGradient; // Color gradient for the day/night cycle
    [SerializeField] private AnimationCurve brightnessCurve; // Curve to control brightness throughout the day

    private float currentTime = 0f;

    private void Start()
    {
        if (backgroundSprite == null)
        {
            backgroundSprite = GetComponent<SpriteRenderer>();
        }

        if (backgroundSprite == null)
        {
            Debug.LogError("No SpriteRenderer found for the background!");
        }

        // Initialize the color gradient if not set in the inspector
        if (colorGradient.colorKeys.Length == 0)
        {
            SetDefaultColorGradient();
        }

        // Initialize the brightness curve if not set in the inspector
        if (brightnessCurve.keys.Length == 0)
        {
            SetDefaultBrightnessCurve();
        }
    }

    private void Update()
    {
        // Update the current time
        currentTime += Time.deltaTime;
        if (currentTime > cycleDuration)
        {
            currentTime -= cycleDuration;
        }

        // Calculate the current cycle progress (0 to 1)
        float cycleProgress = currentTime / cycleDuration;

        // Get the base color from the gradient
        Color baseColor = colorGradient.Evaluate(cycleProgress);

        // Apply brightness adjustment
        float brightness = brightnessCurve.Evaluate(cycleProgress);
        Color adjustedColor = new Color(
            baseColor.r * brightness,
            baseColor.g * brightness,
            baseColor.b * brightness,
            baseColor.a
        );

        // Apply the color to the background sprite
        if (backgroundSprite != null)
        {
            backgroundSprite.color = adjustedColor;
        }
    }

    private void SetDefaultColorGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[5];
        colorKeys[0] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f);    // Night
        colorKeys[1] = new GradientColorKey(new Color(0.8f, 0.6f, 0.4f), 0.25f); // Sunrise
        colorKeys[2] = new GradientColorKey(new Color(0.5f, 0.8f, 1f), 0.5f);    // Day
        colorKeys[3] = new GradientColorKey(new Color(0.8f, 0.4f, 0.2f), 0.75f); // Sunset
        colorKeys[4] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 1f);    // Back to night

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        colorGradient.SetKeys(colorKeys, alphaKeys);
    }

    private void SetDefaultBrightnessCurve()
    {
        brightnessCurve = new AnimationCurve(
            new Keyframe(0f, 0.2f),    // Night
            new Keyframe(0.25f, 0.8f), // Sunrise
            new Keyframe(0.5f, 1f),    // Midday
            new Keyframe(0.75f, 0.8f), // Sunset
            new Keyframe(1f, 0.2f)     // Back to night
        );
    }

    public float GetNormalizedTime()
    {
        return currentTime / cycleDuration;
    }

    public bool IsNight()
    {
        float normalizedTime = GetNormalizedTime();
        return normalizedTime > 0.8f || normalizedTime < 0.2f;
    }
}
