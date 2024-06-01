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
    public Button confirmButton;  // Reference to the confirm button

    void Start()
    {
        originalPosition = transform.position;
        confirmButton.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (isSelected)
        {
            // Deselect the card and move it back to the original position
            transform.position = originalPosition;
            confirmButton.gameObject.SetActive(false);
            selectedCard = null;
        }
        else
        {
            // If another card is selected, deselect it
            if (selectedCard != null)
            {
                selectedCard.GetComponent<CardClickHandler>().Deselect();
            }

            // Select this card and move it upwards
            transform.position = new Vector3(originalPosition.x, originalPosition.y + moveDistance, originalPosition.z);
            confirmButton.gameObject.SetActive(true);
            selectedCard = gameObject;
        }

        // Toggle the selection state
        isSelected = !isSelected;
    }

    public void Deselect()
    {
        isSelected = false;
        transform.position = originalPosition;
    }
}
