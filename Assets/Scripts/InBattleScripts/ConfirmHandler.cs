using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmHandler : MonoBehaviour
{
    public static ConfirmHandler Instance { get; private set; }

    public Transform confirmedCardPosition;
    public DeckManager deckManager;
    public EnemySpawner enemySpawner;
    public PlayerSpawner playerSpawner;

    public static Enemy selectedEnemy;
    public static Player selectedPlayer;

    private GameObject playerInstance;
    private GameObject enemyInstance;
    public ButtonManager buttonManager;
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
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
        else
        {
            Debug.LogWarning("WaveManager not found.");
        }

        playerSpawner.SpawnPlayer();
        playerInstance = playerSpawner.GetPlayerInstance();
    }

    public void ConfirmCard()
    {
        Debug.Log("ConfirmCard Function");
        List<GameObject> selectedCards = NewCardClick.selectedCards;
        if (selectedCards.Count > 0)
        {
            StartCoroutine(UseConfirmedCards(selectedCards));
        }
        else
        {
            Debug.LogWarning("No cards selected for confirmation.");
        }
    }

    IEnumerator UseConfirmedCards(List<GameObject> selectedCards)
    {
        Debug.Log("StartedIEnumerator");
        // Move all selected cards to the confirmed card position
        foreach (GameObject cardObject in selectedCards)
        {
            cardObject.transform.position = confirmedCardPosition.position;
            cardObject.GetComponent<Collider2D>().enabled = false;
            HideOtherCards();
        }

        // Wait a brief moment to ensure all cards are positioned correctly
        yield return new WaitForSeconds(0.5f);

        // Use each confirmed card
        foreach (GameObject cardObject in selectedCards)
        {
            NewCardClick cardClick = cardObject.GetComponent<NewCardClick>();
            CardEffect cardEffect = cardObject.GetComponent<CardEffect>();

            // Check if the card targets an enemy
            if (cardClick.GetSelectedEnemy() != null &&
                (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                Enemy enemyTarget = cardClick.GetSelectedEnemy().GetComponent<Enemy>(); // Get the Enemy component
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    enemyTarget.ShowReticle(false);
                    enemyTarget.ShowSelectedReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, enemyTarget.gameObject)); // Pass the GameObject of the enemy
                }
                else
                {
                    Debug.Log("Not enough resources to use this card.");
                }
            }
            // Check if the card targets a player for healing or defense
            else if (cardClick.GetSelectedPlayer() != null &&
                     (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player playerTarget = cardClick.GetSelectedPlayer().GetComponent<Player>(); // Get the Player component
                if (playerTarget != null && playerTarget.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    playerTarget.ShowReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, playerTarget.gameObject)); // Pass the GameObject of the player
                }
                else
                {
                    Debug.Log("Not enough resources to use this card.");
                }
            }
            else
            {
                Debug.LogWarning("No valid target selected or invalid card effect type.");
            }
        }

        // Handle enemy attacks after all cards are used
        foreach (GameObject enemyObject in enemySpawner.GetEnemyInstances())
        {
            if (enemyObject != null)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    EnemyAttack(enemy); // Pass the Enemy object directly
                    yield return new WaitForSeconds(2f);
                }
            }
        }

        StartNextRound();
    }

    IEnumerator UseConfirmedCard(GameObject cardObject, CardEffect cardEffect, GameObject target)
    {
        yield return new WaitForSeconds(2f);  // Delay to simulate card effect processing time

        if (target != null)
        {
            if (target.CompareTag("Enemy") && (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    cardEffect.ApplyEffect(enemy.gameObject);
                }
            }
            else if (target.CompareTag("Player") && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    player.UpdateCost(player.currentCost - cardEffect.cost);
                    cardEffect.ApplyEffect(player.gameObject);
                }
            }
        }

        // Handle post-effect logic
        cardObject.SetActive(false);
        ReplaceCardInHand(cardObject);
        yield return new WaitForSeconds(2f);
    }



    void EnemyAttack(Enemy enemy)
    {
        if (enemy != null && playerInstance != null)
        {
            Player player = playerInstance.GetComponent<Player>();

            if (player != null)
            {
                int damage = enemy.CalculateDamage();

                if (enemy.enemyDamageType == Enemy.DamageType.Physical)
                {
                    player.TakeDamage(damage);
                    Debug.Log($"Enemy {enemy.enemyCode} attacked player for {damage} physical damage.");
                }
                else if (enemy.enemyDamageType == Enemy.DamageType.Magical)
                {
                    player.TakeMagicDamage(damage);
                    Debug.Log($"Enemy {enemy.enemyCode} attacked player for {damage} magical damage.");
                }
            }
        }
    }

    void StartNextRound()
    {
        ShowAllCards();
        RoundManager.Instance.StartNextRound();
        NewCardClick.selectedCards.Clear();
    }

    public void HideOtherCards()
    {
        // Find all cards in the scene with the "Card" tag
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");

        // Iterate through all cards and hide those not in selectedCards
        foreach (GameObject card in allCards)
        {
            if (!CardClickHandler.selectedCards.Contains(card))
            {
                HideCard(card);
            }
        }
    }

    private void HideCard(GameObject card)
    {
        // Implement logic to hide the card, e.g., set inactive, move out of view, etc.
        card.SetActive(false);
    }

    void ShowAllCards()
    {
        foreach (GameObject card in deckManager.hand)
        {
            card.SetActive(true);
        }
    }

    void ReplaceCardInHand(GameObject usedCard)
    {
        int cardIndex = deckManager.hand.IndexOf(usedCard);
        if (cardIndex != -1)
        {
            deckManager.hand.RemoveAt(cardIndex);
            Destroy(usedCard);

            if (deckManager.deck.Count > 0)
            {
                List<GameObject> availableCards = new List<GameObject>(deckManager.allCards);

                foreach (var card in deckManager.hand)
                {
                    availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                }
                availableCards.RemoveAll(c => c.name == usedCard.name.Replace("(Clone)", ""));

                if (availableCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableCards.Count);
                    GameObject newCard = Instantiate(availableCards[randomIndex], deckManager.canvasTransform);

                    // Set the card's RectTransform properties to match the corresponding hand position
                    RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();
                    RectTransform handPositionRectTransform = deckManager.handPositions[cardIndex] as RectTransform;

                    // Copy RectTransform properties
                    cardRectTransform.anchorMin = handPositionRectTransform.anchorMin;
                    cardRectTransform.anchorMax = handPositionRectTransform.anchorMax;
                    cardRectTransform.pivot = handPositionRectTransform.pivot;
                    cardRectTransform.anchoredPosition = handPositionRectTransform.anchoredPosition;
                    cardRectTransform.sizeDelta = handPositionRectTransform.sizeDelta;

                    // Ensure the card has a Graphic component with Raycast Target enabled
                    Image image = newCard.GetComponent<Image>();
                    if (image != null)
                    {
                        image.raycastTarget = true;
                    }

                    // Ensure the card has a BoxCollider2D and adjust its size
                    BoxCollider2D boxCollider = newCard.GetComponent<BoxCollider2D>();
                    if (boxCollider == null)
                    {
                        boxCollider = newCard.AddComponent<BoxCollider2D>();
                    }
                    boxCollider.size = cardRectTransform.sizeDelta;

                    // Get the CardClickHandler component if it exists and set its properties
                    var cardClickHandler = newCard.GetComponent<CardClickHandler>();
                    if (cardClickHandler != null)
                    {
                        cardClickHandler.deckManager = deckManager;
                        cardClickHandler.buttonManager = ButtonManager.Instance;
                    }

                    deckManager.hand.Insert(cardIndex, newCard);
                    HideOtherCards();
                }
            }
        }
    }
}