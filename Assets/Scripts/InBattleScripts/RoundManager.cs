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
    }

    public void StartNextRound()
    {
        currentRound++;
        playerCost++;
        UpdateUI();
        player.UpdateCost(playerCost); // Assume Player has a method to update the cost
    }

    private void UpdateUI()
    {
        if (roundText != null)
        {
            roundText.text = "Round: " + currentRound;
        }

        if (costText != null)
        {
            costText.text = "Cost: " + playerCost;
        }
    }
}
