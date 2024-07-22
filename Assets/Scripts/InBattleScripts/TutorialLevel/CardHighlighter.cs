using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHighlighter : MonoBehaviour
{
    public GameObject highlightPrefab;
    private GameObject currentHighlight;
    private Canvas canvas;

    private void Start()
    {
        // Find the Canvas in the scene
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
        }
    }

    public void HighlightCard(RectTransform cardRectTransform)
    {
        // Remove any existing highlight
        RemoveHighlight();

        if (canvas == null)
        {
            Debug.LogError("Cannot highlight card: No Canvas available.");
            return;
        }

        // Instantiate new highlight as a child of the Canvas
        currentHighlight = Instantiate(highlightPrefab, canvas.transform);

        // Position highlight
        RectTransform highlightRect = currentHighlight.GetComponent<RectTransform>();
        highlightRect.position = cardRectTransform.position;
        highlightRect.sizeDelta = cardRectTransform.sizeDelta + new Vector2(20, 20); // Make highlight slightly larger

        // Ensure highlight is behind the card
        currentHighlight.transform.SetSiblingIndex(cardRectTransform.GetSiblingIndex());
    }

    public void RemoveHighlight()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
            currentHighlight = null;
        }
    }
}
