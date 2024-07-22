using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkOverlayHighlight : MonoBehaviour
{
    public Image darkOverlay;
    public RectTransform highlightArea;

    void Start()
    {
        CreateOverlay();
        CreateHighlightMask();
    }

    void CreateOverlay()
    {
        darkOverlay.color = new Color(0, 0, 0, 0.7f); // Adjust alpha as needed
        darkOverlay.rectTransform.anchorMin = Vector2.zero;
        darkOverlay.rectTransform.anchorMax = Vector2.one;
        darkOverlay.rectTransform.sizeDelta = Vector2.zero;
    }

    void CreateHighlightMask()
    {
        GameObject maskObj = new GameObject("HighlightMask");
        maskObj.transform.SetParent(darkOverlay.transform, false);

        Image maskImage = maskObj.AddComponent<Image>();
        maskImage.color = Color.white;

        Mask mask = maskObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        maskObj.GetComponent<RectTransform>().position = highlightArea.position;
        maskObj.GetComponent<RectTransform>().sizeDelta = highlightArea.sizeDelta;

        GameObject transparentArea = new GameObject("TransparentArea");
        transparentArea.transform.SetParent(maskObj.transform, false);

        Image transparentImage = transparentArea.AddComponent<Image>();
        transparentImage.color = new Color(1, 1, 1, 0);

        transparentArea.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        transparentArea.GetComponent<RectTransform>().anchorMax = Vector2.one;
        transparentArea.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    }
}
