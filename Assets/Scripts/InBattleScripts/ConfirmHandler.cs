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
        List<GameObject> selectedCards = CardClickHandler.selectedCards;
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
        // Move all selected cards to the confirmed card position
        foreach (GameObject card in selectedCards)
        {
            card.transform.position = confirmedCardPosition.position;
            card.GetComponent<Collider2D>().enabled = false;
            HideOtherCards();
        }

        // Wait a brief moment to ensure all cards are positioned correctly
        yield return new WaitForSeconds(0.5f);

        // Use each confirmed card
        foreach (GameObject card in selectedCards)
        {
            CardEffect cardEffect = card.GetComponent<CardEffect>();

            // Check if the card targets an enemy
            if (selectedEnemy != null && (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                Enemy enemy = selectedEnemy.GetComponent<Enemy>();
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    selectedEnemy.ShowReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(card, cardEffect));
                }
                else
                {
                    Debug.Log("Not enough resources to use this card.");
                }
            }
            // Check if the card targets a player for healing or defense
            else if (selectedPlayer != null && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player playerTarget = selectedPlayer.GetComponent<Player>();
                if (playerTarget != null && playerTarget.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    selectedPlayer.ShowReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(card, cardEffect));
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
        foreach (GameObject enemy in enemySpawner.GetEnemyInstances())
        {
            EnemyAttack(enemy);
            yield return new WaitForSeconds(2f);
        }

        StartNextRound();
    }

    IEnumerator UseConfirmedCard(GameObject confirmedCard, CardEffect cardEffect)
    {
        yield return new WaitForSeconds(2f);  // Delay to simulate card effect processing time

        if (selectedEnemy != null && (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
        {
            cardEffect.ApplyEffect(selectedEnemy.gameObject);
        }
        else if (selectedPlayer != null && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
        {
            Player playerTarget = selectedPlayer.GetComponent<Player>();
            if (playerTarget != null)
            {
                playerTarget.UpdateCost(playerTarget.currentCost - cardEffect.cost);
                cardEffect.ApplyEffect(selectedPlayer.gameObject);
            }
        }

        // Handle post-effect logic
        confirmedCard.SetActive(false);
        ReplaceCardInHand(confirmedCard);
        yield return new WaitForSeconds(2f);
    }


    void EnemyAttack(GameObject enemyInstance)
    {
        if (enemyInstance != null && playerInstance != null)
        {
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            Player player = playerInstance.GetComponent<Player>();
            if (enemy != null && player != null)
            {
                int damage = enemy.CalculateDamage();
                if (enemy.enemyDamageType == Enemy.DamageType.Physical)
                {
                    player.TakeDamage(damage);
                    Debug.Log("Enemy attacked player for " + damage + " physical damage.");
                }
                else if (enemy.enemyDamageType == Enemy.DamageType.Magical)
                {
                    player.TakeMagicDamage(damage);
                    Debug.Log("Enemy attacked player for " + damage + " magical damage.");
                }
            }
        }
    }

    void StartNextRound()
    {
        selectedEnemy = null;
        ShowAllCards();
        RoundManager.Instance.StartNextRound();
        CardClickHandler.selectedCards.Clear();
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