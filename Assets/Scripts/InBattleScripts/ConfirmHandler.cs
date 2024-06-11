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

    private GameObject playerInstance;
    private GameObject enemyInstance;
    public ButtonManager buttonManager;

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
        if (CardClickHandler.selectedCard != null && selectedEnemy != null)
        {
            CardEffect cardEffect = CardClickHandler.selectedCard.GetComponent<CardEffect>();
            Player player = playerInstance.GetComponent<Player>();

            if (player.HasEnoughCost(cardEffect.cost))
            {
                buttonManager.ShowConfirmButton(false);
                CardClickHandler.selectedCard.transform.position = confirmedCardPosition.position;
                HideOtherCards();
                CardClickHandler.selectedCard.GetComponent<Collider2D>().enabled = false;
                selectedEnemy.ShowReticle(false);
                StartCoroutine(UseConfirmedCard(CardClickHandler.selectedCard, cardEffect));
            }
            else
            {
                Debug.Log("Not enough resources to use this card.");
            }
        }
    }

    IEnumerator UseConfirmedCard(GameObject confirmedCard, CardEffect cardEffect)
    {
        yield return new WaitForSeconds(2f);

        Player player = playerInstance.GetComponent<Player>();

        player.UpdateCost(player.currentCost - cardEffect.cost);

        if (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense)
        {
            cardEffect.ApplyEffect(playerInstance);
        }
        else
        {
            cardEffect.ApplyEffect(selectedEnemy.gameObject);
        }

        yield return new WaitForSeconds(2f);
        confirmedCard.SetActive(false);

        foreach (GameObject enemy in enemySpawner.GetEnemyInstances())
        {
            EnemyAttack(enemy);
            yield return new WaitForSeconds(1f);
        }

        StartNextRound();
        ReplaceCardInHand(confirmedCard);
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
                player.TakeDamage(damage);
                Debug.Log("Enemy attacked player for " + damage + " damage.");
            }
        }
    }

    void StartNextRound()
    {
        selectedEnemy = null;
        ShowAllCards();
        RoundManager.Instance.StartNextRound();
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

                foreach (var card in deckManager.hand)
                {
                    availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                }
                availableCards.RemoveAll(c => c.name == usedCard.name.Replace("(Clone)", ""));

                if (availableCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableCards.Count);
                    GameObject newCard = Instantiate(availableCards[randomIndex], deckManager.handPositions[cardIndex].position, Quaternion.identity);
                    var cardClickHandler = newCard.GetComponent<CardClickHandler>();
                    cardClickHandler.buttonManager = ButtonManager.Instance;
                    deckManager.hand.Insert(cardIndex, newCard);
                }
            }

            foreach (GameObject card in deckManager.hand)
            {
                card.SetActive(true);
            }
        }
    }
}