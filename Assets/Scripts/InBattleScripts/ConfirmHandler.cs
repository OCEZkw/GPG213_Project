using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConfirmHandler : MonoBehaviour
{
    public Transform confirmedCardPosition;  // Position where the confirmed card will be displayed
    public Button confirmButton;  // Reference to the confirm button
    public DeckManager deckManager;  // Reference to the DeckManager
    public EnemySpawner enemySpawner; // Reference to the EnemySpawner script
    public PlayerSpawner playerSpawner; // Reference to the PlayerSpawner script

    private GameObject playerInstance;  // Instance of the player
    private GameObject enemyInstance;  // Instance of the enemy

    void Start()
    {
        confirmButton.onClick.AddListener(ConfirmCard);
        confirmButton.gameObject.SetActive(false);

        // Spawn the enemy and player instances
        enemySpawner.SpawnEnemy();
        enemyInstance = enemySpawner.GetEnemyInstance(); // Get reference to the enemy instance
        playerSpawner.SpawnPlayer();
        playerInstance = playerSpawner.GetPlayerInstance(); // Get reference to the player instance
    }

    void ConfirmCard()
    {
        if (CardClickHandler.selectedCard != null)
        {
            // Move the selected card to the confirmed position
            CardClickHandler.selectedCard.transform.position = confirmedCardPosition.position;

            // Disable the confirm button and hide the other cards
            confirmButton.gameObject.SetActive(false);
            HideOtherCards();

            // Optionally, disable further interaction with the selected card
            CardClickHandler.selectedCard.GetComponent<Collider2D>().enabled = false;

            // Start the coroutine to handle the confirmed card usage
            StartCoroutine(UseConfirmedCard(CardClickHandler.selectedCard));
        }
    }

    IEnumerator UseConfirmedCard(GameObject confirmedCard)
    {
        yield return new WaitForSeconds(2f);

        // Logic to use the confirmed card (e.g., apply effects, deal damage, etc.)
        Debug.Log("Confirmed card used: " + confirmedCard.name);

        // Apply the card's effect
        CardEffect cardEffect = confirmedCard.GetComponent<CardEffect>();
        if (cardEffect != null)
        {
            if (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense)
            {
                cardEffect.ApplyEffect(playerInstance);
            }
            else
            {
                cardEffect.ApplyEffect(enemyInstance);
            }
        }

        // Enemy attacks the player after the card's effect is applied
        yield return new WaitForSeconds(1f);
        EnemyAttack();

        // Wait for the enemy attack animation/effect
        yield return new WaitForSeconds(1f);

        // Start the next round
        StartNextRound();

        // Replace the used card with a new card from the deck
        ReplaceCardInHand(confirmedCard);
    }

    void EnemyAttack()
    {
        if (enemyInstance != null && playerInstance != null)
        {
            // Assuming the enemy has a method to attack the player
            Enemy enemy = enemyInstance.GetComponent<Enemy>();
            Player player = playerInstance.GetComponent<Player>();
            if (enemy != null && player != null)
            {
                int damage = enemy.CalculateDamage();  // Assume CalculateDamage() method exists in the Enemy script
                player.TakeDamage(damage);
                Debug.Log("Enemy attacked player for " + damage + " damage.");
            }
        }
    }

    void StartNextRound()
    {
        // Reset card positions, enable interactions, and show the confirm button
        ShowAllCards();
        confirmButton.gameObject.SetActive(true);
    }

    void HideOtherCards()
    {
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
        {
            if (card != CardClickHandler.selectedCard)
            {
                card.SetActive(false);
            }
        }
    }

    void ShowAllCards()
    {
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
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

                // Remove cards that are already in hand
                foreach (var card in deckManager.hand)
                {
                    availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                }
                availableCards.RemoveAll(c => c.name == usedCard.name.Replace("(Clone)", ""));

                // Draw a new card from the available cards
                if (availableCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableCards.Count);
                    GameObject newCard = Instantiate(availableCards[randomIndex], deckManager.handPositions[cardIndex].position, Quaternion.identity);
                    var cardClickHandler = newCard.GetComponent<CardClickHandler>();
                    cardClickHandler.confirmButton = confirmButton;  // Assign the confirm button
                    deckManager.hand.Insert(cardIndex, newCard);
                }
            }

            // Show all cards again
            foreach (GameObject card in deckManager.hand)
            {
                card.SetActive(true);
            }
        }
    }
}