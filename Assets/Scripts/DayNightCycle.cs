using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private float cycleDuration = 240f;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private AnimationCurve brightnessCurve;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D[] globalLights; // Array of global lights
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D[] pointLights;  // Array of point lights
    [SerializeField] private AnimationCurve lightIntensityCurve; // Curve to control light intensity
    [SerializeField] private Gradient pointLightColorGradient;

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

        if (lightIntensityCurve.keys.Length == 0)
        {
            SetDefaultLightIntensityCurve();
        }

        if (pointLightColorGradient.colorKeys.Length == 0)
        {
            SetDefaultPointLightColorGradient();
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


        // Update global lights
        UpdateGlobalLights(adjustedColor, cycleProgress);

        // Update point lights
        UpdatePointLights(cycleProgress);
    }

    private void UpdateGlobalLights(Color adjustedColor, float cycleProgress)
    {
        float lightIntensity = lightIntensityCurve.Evaluate(cycleProgress);

        foreach (Light2D light in globalLights)
        {
            if (light != null)
            {
                light.color = adjustedColor;
                light.intensity = lightIntensity;
            }
        }
    }

    private void UpdatePointLights(float cycleProgress)
    {
        float lightIntensity = lightIntensityCurve.Evaluate(cycleProgress);
        Color pointLightColor = pointLightColorGradient.Evaluate(cycleProgress);

        foreach (Light2D light in pointLights)
        {
            if (light != null)
            {
                light.color = pointLightColor;
                light.intensity = lightIntensity;
            }
        }
    }

    private void SetDefaultLightIntensityCurve()
    {
        lightIntensityCurve = new AnimationCurve(
            new Keyframe(0f, 0.2f),    // Night
            new Keyframe(0.25f, 0.8f), // Sunrise
            new Keyframe(0.5f, 1f),    // Midday
            new Keyframe(0.75f, 0.8f), // Sunset
            new Keyframe(1f, 0.2f)     // Back to night
        );
    }

    private void SetDefaultPointLightColorGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[5];
        colorKeys[0] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0f);    // Night
        colorKeys[1] = new GradientColorKey(new Color(1f, 0.8f, 0.6f), 0.25f);   // Sunrise
        colorKeys[2] = new GradientColorKey(new Color(1f, 1f, 1f), 0.5f);        // Day
        colorKeys[3] = new GradientColorKey(new Color(1f, 0.6f, 0.4f), 0.75f);   // Sunset
        colorKeys[4] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 1f);    // Back to night

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        pointLightColorGradient.SetKeys(colorKeys, alphaKeys);
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
