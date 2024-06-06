using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardClickHandler : MonoBehaviour
{
    private bool isSelected = false;
    private Vector3 originalPosition;
    private float moveDistance = 1f;  // Distance to move the card upwards when selected

    public static GameObject selectedCard;
    public ButtonManager buttonManager;  // Reference to the ButtonManager

    void Start()
    {
        originalPosition = transform.position;
        buttonManager = FindObjectOfType<ButtonManager>(); // Initialize buttonManager
    }

    void OnMouseDown()
    {
        if (isSelected)
        {
            // Deselect the card and move it back to the original position
            Deselect();
        }
        else
        {
            // If another card is selected, deselect it
            if (selectedCard != null)
            {
                selectedCard.GetComponent<CardClickHandler>().Deselect();
            }

            // Select this card and move it upwards
            Select();
        }
    }

    public void Select()
    {
        isSelected = true;
        transform.position = new Vector3(originalPosition.x, originalPosition.y + moveDistance, originalPosition.z);
        buttonManager.ShowSelectTargetButton(true);
        selectedCard = gameObject;

        // Show all reticles on enemies
        ShowAllReticles(true);
    }

    public void Deselect()
    {
        isSelected = false;
        transform.position = originalPosition;
        if (selectedCard == gameObject)
        {
            selectedCard = null;
        }

        // Check if any card is still selected
        if (selectedCard == null)
        {
            buttonManager.ShowSelectTargetButton(false);
            buttonManager.ShowConfirmButton(false);
        }

        // Hide all reticles on enemies
        ShowAllReticles(false);
    }

    private void ShowAllReticles(bool show)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ShowReticle(show);
        }
    }
}

