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
    public RoundManager roundManager;    // Reference to the RoundManager
    private CardEffect cardEffect;       // Reference to the CardEffect
    public Player player;

    void Start()
    {
        originalPosition = transform.position;
        buttonManager = FindObjectOfType<ButtonManager>(); // Initialize buttonManager
        roundManager = FindObjectOfType<RoundManager>();   // Initialize roundManager
        cardEffect = GetComponent<CardEffect>();           // Initialize cardEffect
        player = FindObjectOfType<Player>();
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

        // Update the cost UI
        UpdateCostUI();

        // Determine whether to show the reticle on the player or enemies
        if (cardEffect != null && cardEffect.IsDefenseOrHealCard())
        {
            ShowPlayerReticle(true);
            DisableEnemyColliders(true); // Disable enemy colliders if it's a heal or defense card
            DisablePlayerCollider(false); // Enable player collider for heal or defense card
        }
        else
        {
            ShowAllReticles(true);
            DisableEnemyColliders(false); // Enable enemy colliders if it's not a heal or defense card
            DisablePlayerCollider(true); // Disable player collider for other card types
        }
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

            // Reset cost UI
            roundManager.UpdateUI();
        }

        // Determine whether to hide the reticle on the player or enemies
        if (cardEffect != null && cardEffect.IsDefenseOrHealCard())
        {
            ShowPlayerReticle(false);
            DisableEnemyColliders(false); // Enable enemy colliders if it's a heal or defense card
            DisablePlayerCollider(false); // Enable player collider for heal or defense card
        }
        else
        {
            ShowAllReticles(false);
            DisableEnemyColliders(false); // Enable enemy colliders if it's not a heal or defense card
            DisablePlayerCollider(false); // Enable player collider for other card types
        }
    }

    private void ShowAllReticles(bool show)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ShowReticle(show);
        }
    }

    private void ShowPlayerReticle(bool show)
    {
        if (player != null)
        {
            player.ShowReticle(show); // Assuming the player has a ShowReticle method
        }
    }

    private void UpdateCostUI()
    {
        if (cardEffect != null && roundManager != null && roundManager.costText != null)
        {
            int remainingCost = roundManager.playerCost - cardEffect.cost;
            roundManager.costText.text = $"Costs: {remainingCost}/{roundManager.playerCost}";
        }
    }

    private void DisableEnemyColliders(bool disable)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Collider2D collider = enemy.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = !disable;
            }
        }
    }

    private void DisablePlayerCollider(bool disable)
    {
        Collider2D collider = player.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = !disable;
        }
    }
}

