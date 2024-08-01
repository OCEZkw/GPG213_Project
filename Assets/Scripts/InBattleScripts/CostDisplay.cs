using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostDisplay : MonoBehaviour
{
    public Image crystalPrefab;
    public Color usedCrystalColor = Color.gray;
    public Color availableCrystalColor = Color.white;
    public float spacing = 10f;

    private Image[] crystals;

    public void Initialize(int maxCost)
    {
        crystals = new Image[maxCost];
        for (int i = 0; i < maxCost; i++)
        {
            Image crystal = Instantiate(crystalPrefab, transform);
            crystal.rectTransform.anchoredPosition = new Vector2(i * spacing, 0);
            crystals[i] = crystal;
        }
    }

    public void UpdateDisplay(int currentCost, int maxCost)
    {
        for (int i = 0; i < crystals.Length; i++)
        {
            if (i < currentCost)
            {
                crystals[i].color = availableCrystalColor;
            }
            else
            {
                crystals[i].color = usedCrystalColor;
            }
        }
    }
}
