using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSimulation2D : MonoBehaviour
{
    [SerializeField] private float waveSpeed = 1f;
    [SerializeField] private float waveAmplitude = 0.1f;
    [SerializeField] private float wavePeriod = 2f;

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;
    private float offset = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            // Create a unique instance of the material to avoid affecting other sprites
            materialInstance = new Material(spriteRenderer.material);
            spriteRenderer.material = materialInstance;
        }
        else
        {
            Debug.LogError("SpriteRenderer or its material is missing!");
        }
    }

    private void Update()
    {
        if (materialInstance != null)
        {
            // Update the offset based on time
            offset += Time.deltaTime * waveSpeed;

            // Calculate the wave effect
            float waveEffect = Mathf.Sin(offset * wavePeriod) * waveAmplitude;

            // Apply the wave effect to the material's texture offset
            materialInstance.mainTextureOffset = new Vector2(0, waveEffect);
        }
    }

    // Optional: Add methods to dynamically change wave properties
    public void SetWaveAmplitude(float amplitude)
    {
        waveAmplitude = amplitude;
    }

    public void SetWaveSpeed(float speed)
    {
        waveSpeed = speed;
    }

    public void SetWavePeriod(float period)
    {
        wavePeriod = period;
    }
}
