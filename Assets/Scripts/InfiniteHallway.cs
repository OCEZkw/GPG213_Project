using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteHallway : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    public Renderer floorRenderer;
    public Renderer wallRenderer;

    private Vector2 savedOffset;

    void Start()
    {
        if (floorRenderer == null || wallRenderer == null)
        {
            Debug.LogError("Please assign floor and wall renderers in the inspector.");
            enabled = false;
            return;
        }

        savedOffset = floorRenderer.material.mainTextureOffset;
    }

    void Update()
    {
        float offsetY = Mathf.Repeat(Time.time * scrollSpeed, 1);
        Vector2 offset = new Vector2(savedOffset.x, offsetY);

        floorRenderer.material.mainTextureOffset = offset;
        wallRenderer.material.mainTextureOffset = offset * 0.5f; // Slower for parallax effect
    }

    void OnDisable()
    {
        if (floorRenderer != null && wallRenderer != null)
        {
            floorRenderer.material.mainTextureOffset = savedOffset;
            wallRenderer.material.mainTextureOffset = savedOffset;
        }
    }
}
