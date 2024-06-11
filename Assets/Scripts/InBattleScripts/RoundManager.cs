using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    public int currentRound = 1;
    public int playerCost = 3;
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
        playerCost++; // Increase the total cost available
        player.UpdateCost(playerCost); // Reset player's current cost to the new total cost
        player.ResetCost(); // Reset player's current cost to full
        UpdateUI();
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
}