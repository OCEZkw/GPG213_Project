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

    public static List<GameObject> selectedCards = new List<GameObject>();
    public ButtonManager buttonManager;  // Reference to the ButtonManager
    public RoundManager roundManager;    // Reference to the RoundManager
    private CardEffect cardEffect;       // Reference to the CardEffect
    public Player player;

    public static Enemy selectedEnemy;
    public static Player selectedPlayer;

    public DeckManager deckManager;

    public GameObject notEnoughCostIndicator;  // The UI element to indicate not enough cost

    void Start()
    {
        originalPosition = transform.position;
        buttonManager = FindObjectOfType<ButtonManager>(); // Initialize buttonManager
        roundManager = FindObjectOfType<RoundManager>();   // Initialize roundManager
        deckManager = FindObjectOfType<DeckManager>();
        cardEffect = GetComponent<CardEffect>();           // Initialize cardEffect
        player = FindObjectOfType<Player>();
        notEnoughCostIndicator.SetActive(false);
        // Check if the player has enough cost to use this card
        if (cardEffect != null && !player.HasEnoughCost(cardEffect.cost))
        {
            // Show the not enough cost indicator
            if (notEnoughCostIndicator != null)
            {
                notEnoughCostIndicator.SetActive(true);
            }

            UpdateCostAndEnable();
        }
    }

    void OnMouseDown()
    {
        if (isSelected)
        {
            // Deselect the card and move it back to the original position
            Deselect();
            selectedPlayer = null;
            selectedEnemy = null;
        }
        else
        {
            // If another card is selected, deselect it unless an enemy or player is selected
            if (selectedCards.Count > 0 && selectedEnemy == null && selectedPlayer == null)
            {
                // Create a list to hold cards to be deselected
                List<GameObject> cardsToDeselect = new List<GameObject>();

                // Collect cards to be deselected
                foreach (GameObject card in selectedCards)
                {
                    if (card != null) // Check if card is not null
                    {
                        CardClickHandler cardClickHandler = card.GetComponent<CardClickHandler>();
                        if (cardClickHandler != null)
                        {
                            cardsToDeselect.Add(card);
                        }
                    }
                }

                // Deselect collected cards
                foreach (GameObject card in cardsToDeselect)
                {
                    if (card != null) // Check if card is not null
                    {
                        CardClickHandler cardClickHandler = card.GetComponent<CardClickHandler>();
                        if (cardClickHandler != null)
                        {
                            cardClickHandler.Deselect();
                        }
                    }
                }
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
        selectedCards.Add(gameObject);

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
        CheckNonSelectedCards();
    }

    public void Deselect()
    {
        isSelected = false;
        transform.position = originalPosition;
        selectedCards.Remove(gameObject);

        // Check if any card is still selected
        if (selectedCards.Count == 0)
        {
            buttonManager.ShowSelectTargetButton(false);
            buttonManager.ShowConfirmButton(false);

            // Reset cost UI
            roundManager.UpdateUI();
        }
        else
        {
            // Update the cost UI if there are still selected cards
            UpdateCostUI();
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
        CheckNonSelectedCards();
        
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
        int totalCost = 0;

        foreach (var card in selectedCards)
        {
            CardEffect effect = card.GetComponent<CardEffect>();
            if (effect != null)
            {
                totalCost += effect.cost;
            }
        }

        int remainingCost = roundManager.playerCost - totalCost;
        roundManager.costText.text = $"Costs: {remainingCost}/{roundManager.playerCost}";
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

    void UpdateCostAndEnable()
    {
        if (cardEffect != null && !player.HasEnoughCost(cardEffect.cost))
        {
            // Show the not enough cost indicator
            if (notEnoughCostIndicator != null)
            {
                notEnoughCostIndicator.SetActive(true);
            }

            // Disable the collider to prevent selection
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
        else
        {
            if (notEnoughCostIndicator != null)
            {
                notEnoughCostIndicator.SetActive(false);
            }

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    private void CheckNonSelectedCards()
    {
        int totalSelectedCost = 0;

        foreach (var card in selectedCards)
        {
            CardEffect effect = card.GetComponent<CardEffect>();
            if (effect != null)
            {
                totalSelectedCost += effect.cost;
            }
        }

        int remainingCost = roundManager.playerCost - totalSelectedCost;

        foreach (var card in deckManager.hand)
        {
            if (!selectedCards.Contains(card))
            {
                CardClickHandler cardClickHandler = card.GetComponent<CardClickHandler>();
                if (cardClickHandler != null)
                {
                    CardEffect effect = card.GetComponent<CardEffect>();
                    if (effect != null)
                    {
                        if (remainingCost < effect.cost)
                        {
                            cardClickHandler.notEnoughCostIndicator.SetActive(true);
                            card.GetComponent<Collider2D>().enabled = false;
                        }
                        else
                        {
                            cardClickHandler.notEnoughCostIndicator.SetActive(false);
                            card.GetComponent<Collider2D>().enabled = true;
                        }
                    }
                }
            }
        }
    }
}

