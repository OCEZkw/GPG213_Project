using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    public int currentRound = 1;
    public int playerCost = 3;
    private const int MAX_PLAYER_COST = 10;
    public TMP_Text roundText;
    public TMP_Text costText;
    public Player player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
        player.UpdateCost(playerCost); // Initialize player cost
    }

    public void StartNextRound()
    {
        currentRound++;
        if (playerCost < MAX_PLAYER_COST)
        {
            playerCost = Mathf.Min(playerCost + 1, MAX_PLAYER_COST); // Increase the total cost available, but cap at MAX_PLAYER_COST
        }
        player.UpdateCost(playerCost); // Reset player's current cost to the new total cost
        player.ResetCost(); // Reset player's current cost to full
        UpdateUI();
        UpdateAllCardColliders();
    }

    public void UpdateUI()
    {
        if (roundText != null)
        {
            roundText.text = "Round: " + currentRound;
        }

        if (costText != null)
        {
            costText.text = "Costs: " + player.currentCost + "/" + playerCost;
        }
    }

    private void UpdateAllCardColliders()
    {
        Debug.Log($"Updating all card colliders. Player cost: {player.currentCost}");
        NewCardClick[] cards = FindObjectsOfType<NewCardClick>();
        foreach (var card in cards)
        {
            CardEffect cardEffect = card.GetComponent<CardEffect>();
            Collider2D collider = card.GetComponent<Collider2D>();

            if (cardEffect != null && collider != null && card.notEnoughCostIndicator != null)
            {
                bool hasEnoughCost = player.HasEnoughCost(cardEffect.cost);
                collider.enabled = hasEnoughCost;
                card.notEnoughCostIndicator.SetActive(!hasEnoughCost);
                Debug.Log($"Card: {card.name}, Cost: {cardEffect.cost}, Enabled: {collider.enabled}");
            }
        }
    }
}