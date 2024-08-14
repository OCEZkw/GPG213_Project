using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class NewCardClick : MonoBehaviour
{
    private bool isSelected = false;
    public bool isWaitingForTarget = false;
    private Vector3 originalPosition;
    public float moveDistance = 10f;  // Distance to move the card upwards when selected

    public static List<GameObject> selectedCards = new List<GameObject>();
    public ButtonManager buttonManager;  // Reference to the ButtonManager
    public RoundManager roundManager;    // Reference to the RoundManager
    private CardEffect cardEffect;       // Reference to the CardEffect
    public Player player;

    public IDeckManager deckManager;

    public GameObject notEnoughCostIndicator;  // The UI element to indicate not enough cost

    private Enemy selectedEnemy;
    private Player selectedPlayer;
    private int selectedEnemyCode;

    private BossPart selectedBossPart;

    private CardFloatEffect cardFloatEffect;
    private Vector3 originalScale;
    public float hoverScaleFactor = 1.1f;  // Scale factor when hovering
    public float hoverDuration = 0.2f;     // Duration of hover animation

    private bool isLocked = false;
    public GameObject cardLockedIndicator;
    private Image cardImage;

    void Start()
    {
        originalPosition = transform.position;
        buttonManager = FindObjectOfType<ButtonManager>(); // Initialize buttonManager
        roundManager = FindObjectOfType<RoundManager>();   // Initialize roundManager
        deckManager = FindObjectOfType<DeckManager>() as IDeckManager;
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<TutorialDeckManager>() as IDeckManager;
        }
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

        cardFloatEffect = GetComponent<CardFloatEffect>();
        if (cardFloatEffect == null)
        {
            cardFloatEffect = gameObject.AddComponent<CardFloatEffect>();
        }
        originalScale = transform.localScale;

        cardImage = GetComponent<Image>();
        if (cardImage == null)
        {
            cardImage = GetComponentInChildren<Image>();
        }

        if (cardLockedIndicator != null)
        {
            cardLockedIndicator.SetActive(false);
        }
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        // Update visual feedback to show locked state
        if (cardLockedIndicator != null)
        {
            cardLockedIndicator.SetActive(locked);
        }

        // Disable the collider to prevent selection
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = !locked;
        }

        // Darken the card except for the lock object
        if (cardImage != null)
        {
            Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            cardImage.color = locked ? darkColor : Color.white;
        }

        Debug.Log($"Card {gameObject.name} {(locked ? "locked" : "unlocked")}");
    }

    void Update()
    {

    }

    void OnMouseEnter()
    {
        if (!isSelected)
        {
            transform.DOScale(originalScale * hoverScaleFactor, hoverDuration);
        }
    }

    void OnMouseExit()
    {
        if (!isSelected)
        {
            transform.DOScale(originalScale, hoverDuration);
        }
    }

    // Method to handle selecting an enemy
    public void SelectEnemy(Enemy enemy)
    {
        if (isWaitingForTarget)
        {
            selectedEnemy = enemy;
            selectedEnemyCode = enemy.enemyCode; // Store the enemy code
            // Optionally, you can show some indication that this enemy is selected
            enemy.ShowSelectedReticle(true);
            // Optionally, update UI or perform other actions related to selecting an enemy
            Debug.Log($"Selected Enemy Code: {selectedEnemyCode}");

            buttonManager.ShowConfirmButton(true);
            NotificationManager.Instance.ShowTargetSelectionNotification(false);
            //   buttonManager.ShowSelectTargetButton(false);
            isWaitingForTarget = false;
        }
    }

    // Method to handle selecting a boss part
    public void SelectBossPart(BossPart bossPart)
    {
        if (isWaitingForTarget)
        {
            selectedBossPart = bossPart;
            selectedEnemyCode = bossPart.enemyCode; // Store the boss part code
            ShowAllBossPartReticles(false);
            // Optionally, you can show some indication that this boss part is selected
            bossPart.ShowSelectedReticle(true);
            // Optionally, update UI or perform other actions related to selecting a boss part
            Debug.Log($"Selected Boss Part Code: {selectedEnemyCode}");

            buttonManager.ShowConfirmButton(true);
            NotificationManager.Instance.ShowTargetSelectionNotification(false);
            //  buttonManager.ShowSelectTargetButton(false);
            isWaitingForTarget = false;
        }
    }

    public void SelectPlayer(Player player)
    {
        if (isWaitingForTarget)
        {
            selectedPlayer = player;
            // Optionally, you can show some indication that this player is selected
            // Update UI or perform other actions related to selecting a player
            player.ShowReticle(false);
            buttonManager.ShowConfirmButton(true);
            NotificationManager.Instance.ShowTargetSelectionNotification(false);
            //   buttonManager.ShowSelectTargetButton(false);
            isWaitingForTarget = false;
        }
    }

    // Method to get selected enemy
    public Enemy GetSelectedEnemy()
    {
        return selectedEnemy;
    }

    public BossPart GetSelectedBossPart()
    {
        return selectedBossPart;
    }

    public Player GetSelectedPlayer()
    {
        return selectedPlayer;
    }

    // Method to get selected enemy code
    public int GetSelectedEnemyCode()
    {
        return selectedEnemyCode;
    }

    void OnMouseDown()
    {
        if (isLocked)
        {
            Debug.Log($"Cannot select locked card: {gameObject.name}");
            return;
        }
        else if (isSelected)
        {
            // Deselect the card and move it back to the original position
            Deselect();
        }
        else
        {
            // Check if there is a card waiting for a target
            bool anyCardWaitingForTarget = false;
            foreach (GameObject card in selectedCards)
            {
                if (card != null)
                {
                    NewCardClick newCardClick = card.GetComponent<NewCardClick>();
                    if (newCardClick != null && newCardClick.isWaitingForTarget)
                    {
                        anyCardWaitingForTarget = true;
                        break;
                    }
                }
            }

            if (anyCardWaitingForTarget)
            {
                // Deselect all cards waiting for a target before selecting the new card
                List<GameObject> cardsToDeselect = new List<GameObject>();
                foreach (GameObject card in selectedCards)
                {
                    if (card != null)
                    {
                        NewCardClick newCardClick = card.GetComponent<NewCardClick>();
                        if (newCardClick != null)
                        {
                            cardsToDeselect.Add(card);
                        }
                    }
                }
                foreach (GameObject card in cardsToDeselect)
                {
                    if (card != null)
                    {
                        NewCardClick newCardClick = card.GetComponent<NewCardClick>();
                        if (newCardClick != null)
                        {
                            newCardClick.Deselect();
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
        isWaitingForTarget = true;
        cardFloatEffect.SetSelected(true);
        // Scale up the card when selected
        transform.DOScale(originalScale * hoverScaleFactor, hoverDuration);
        //  buttonManager.ShowSelectTargetButton(true);

        selectedCards.Add(gameObject);
        NotificationManager.Instance.ShowTargetSelectionNotification(true);

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
            ShowSelectedReticles(false);
            ShowAllBossPartReticles(true);
            ShowBossSelectedReticle(false);
            DisableEnemyColliders(false); // Enable enemy colliders if it's not a heal or defense card
            DisablePlayerCollider(true); // Disable player collider for other card types
        }

        CheckNonSelectedCards();
    }

    public void Deselect()
    {
        isSelected = false;
        isWaitingForTarget = false;
        cardFloatEffect.SetSelected(false);
        // Scale down the card when deselected
        transform.DOScale(originalScale, hoverDuration);
        selectedCards.Remove(gameObject);

        // Check if any card is still selected
        if (selectedCards.Count == 0)
        {
            // buttonManager.ShowSelectTargetButton(false);
            NotificationManager.Instance.ShowTargetSelectionNotification(false);
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
            ShowSelectedReticles(false);
            ShowAllBossPartReticles(false);
            ShowBossSelectedReticle(false);
            DisableEnemyColliders(false); // Enable enemy colliders if it's not a heal or defense card
            DisablePlayerCollider(false); // Enable player collider for other card types
        }

        CheckNonSelectedCards();
    }

    // Add this new public method
    public bool IsSelected()
    {
        return isSelected;
    }



    //========Enemy Reticle======//
    private void ShowAllReticles(bool show)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ShowReticle(show);
        }
    }
    private void ShowSelectedReticles(bool show)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ShowSelectedReticle(show);
        }
    }



    //========Boss Reticle========//
    private void ShowAllBossPartReticles(bool show)
    {
        BossPart[] bossParts = FindObjectsOfType<BossPart>();
        foreach (BossPart bossPart in bossParts)
        {
            bossPart.ShowReticle(show);
        }
    }

    private void ShowBossSelectedReticle(bool show)
    {
        BossPart[] bossParts = FindObjectsOfType<BossPart>();
        foreach (BossPart bossPart in bossParts)
        {
            bossPart.ShowSelectedReticle(show);
        }
    }



    //========Player Reticle========//
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

        var hand = deckManager.GetHand();
        if (hand == null)
        {
            Debug.LogError("Hand is null in CheckNonSelectedCards");
            return;
        }

        foreach (var card in hand)
        {
            if (card == null)
            {
                Debug.LogWarning("Null card found in hand");
                continue;
            }

            if (!selectedCards.Contains(card))
            {
                NewCardClick cardClickHandler = card.GetComponent<NewCardClick>();
                if (cardClickHandler != null)
                {
                    CardEffect effect = card.GetComponent<CardEffect>();
                    if (effect != null)
                    {
                        if (cardClickHandler.notEnoughCostIndicator == null)
                        {
                            Debug.LogWarning("notEnoughCostIndicator is null for a card");
                            continue;
                        }

                        Collider2D collider = card.GetComponent<Collider2D>();
                        if (collider == null)
                        {
                            Debug.LogWarning("Collider2D is null for a card");
                            continue;
                        }

                        if (remainingCost < effect.cost)
                        {
                            cardClickHandler.notEnoughCostIndicator.SetActive(true);
                            collider.enabled = false;
                        }
                        else
                        {
                            cardClickHandler.notEnoughCostIndicator.SetActive(false);
                            collider.enabled = true;
                        }
                    }
                }
            }
        }
    }
}