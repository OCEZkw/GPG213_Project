using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialConfirmHandler : MonoBehaviour
{
    public static TutorialConfirmHandler Instance { get; private set; }

    public Transform confirmedCardPosition;
    public TutorialDeckManager tutorialDeckManager;
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
        Debug.Log("TutorialConfirmCard Function");
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
        foreach (GameObject cardObject in selectedCards)
        {
            cardObject.transform.position = confirmedCardPosition.position;
            cardObject.GetComponent<Collider2D>().enabled = false;
            HideOtherCards();

            NewCardClick cardClick = cardObject.GetComponent<NewCardClick>();
            CardEffect cardEffect = cardObject.GetComponent<CardEffect>();

            if (cardClick.GetSelectedEnemy() != null &&
                (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                Enemy enemyTarget = cardClick.GetSelectedEnemy().GetComponent<Enemy>();
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    enemyTarget.ShowReticle(false);
                    enemyTarget.ShowSelectedReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, enemyTarget.gameObject));
                }
                else
                {
                    Debug.Log("Not enough resources to use this card.");
                }
            }
            else if (cardClick.GetSelectedBossPart() != null &&
                     (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                BossPart bossPartTarget = cardClick.GetSelectedBossPart().GetComponent<BossPart>();
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    bossPartTarget.ShowReticle(false);
                    bossPartTarget.ShowSelectedReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, bossPartTarget.gameObject));
                }
                else
                {
                    Debug.Log("Not enough resources to use this card.");
                }
            }
            else if (cardClick.GetSelectedPlayer() != null &&
                     (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player playerTarget = cardClick.GetSelectedPlayer().GetComponent<Player>();
                if (playerTarget != null && playerTarget.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    playerTarget.ShowReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, playerTarget.gameObject));
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

        yield return new WaitForSeconds(1f); // Delay before enemy actions

        foreach (GameObject enemyObject in enemySpawner.GetEnemyInstances())
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                EnemyAttack(enemy);
                yield return new WaitForSeconds(2f); // Delay between enemy attacks
            }
        }

        playerInstance = playerSpawner.GetPlayerInstance();
        WizardBossEnemy wizardBoss = FindObjectOfType<WizardBossEnemy>();
        if (wizardBoss != null)
        {
            wizardBoss.BossAttackAfterPlayerActions(playerInstance.GetComponent<Player>());

            while (wizardBoss.IsBossAttacking)
            {
                yield return null;
            }
        }

        StartNextRound();
    }

    IEnumerator UseConfirmedCard(GameObject cardObject, CardEffect cardEffect, GameObject target)
    {
        yield return new WaitForSeconds(2f);

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
            else if (target.CompareTag("BossPart") && (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                BossPart bossPart = target.GetComponent<BossPart>();
                if (bossPart != null)
                {
                    cardEffect.ApplyEffect(bossPart.gameObject);
                }
            }
            else if (target.CompareTag("Player") && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    cardEffect.ApplyEffect(player.gameObject);
                }
            }
        }

        cardObject.SetActive(false);
        ReplaceCardInHand(cardObject);
        yield return new WaitForSeconds(1f);
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
        foreach (GameObject card in tutorialDeckManager.GetHand())
        {
            if (!NewCardClick.selectedCards.Contains(card))
            {
                card.SetActive(false);
            }
        }
    }

    void ShowAllCards()
    {
        foreach (GameObject card in tutorialDeckManager.GetHand())
        {
            card.SetActive(true);
        }
    }

    void ReplaceCardInHand(GameObject usedCard)
    {
        List<GameObject> hand = tutorialDeckManager.GetHand();
        int cardIndex = hand.IndexOf(usedCard);
        if (cardIndex != -1)
        {
            hand.RemoveAt(cardIndex);
            Destroy(usedCard);

            if (tutorialDeckManager.tutorialDeck.Count > 0)
            {
                List<GameObject> availableCards = new List<GameObject>(tutorialDeckManager.tutorialDeck);

                foreach (var card in hand)
                {
                    availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                }
                availableCards.RemoveAll(c => c.name == usedCard.name.Replace("(Clone)", ""));

                if (availableCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableCards.Count);
                    GameObject newCard = Instantiate(availableCards[randomIndex], tutorialDeckManager.canvasTransform);

                    RectTransform cardRectTransform = newCard.GetComponent<RectTransform>();
                    RectTransform handPositionRectTransform = tutorialDeckManager.handPositions[cardIndex] as RectTransform;

                    cardRectTransform.anchorMin = handPositionRectTransform.anchorMin;
                    cardRectTransform.anchorMax = handPositionRectTransform.anchorMax;
                    cardRectTransform.pivot = handPositionRectTransform.pivot;
                    cardRectTransform.anchoredPosition = handPositionRectTransform.anchoredPosition;
                    cardRectTransform.sizeDelta = handPositionRectTransform.sizeDelta;

                    Image image = newCard.GetComponent<Image>();
                    if (image != null)
                    {
                        image.raycastTarget = true;
                    }

                    BoxCollider2D boxCollider = newCard.GetComponent<BoxCollider2D>();
                    if (boxCollider == null)
                    {
                        boxCollider = newCard.AddComponent<BoxCollider2D>();
                    }
                    boxCollider.size = cardRectTransform.sizeDelta;

                    var newCardClick = newCard.GetComponent<NewCardClick>();
                    if (newCardClick != null)
                    {
                        newCardClick.deckManager = tutorialDeckManager;
                        newCardClick.buttonManager = ButtonManager.Instance;
                        // Set other necessary properties for NewCardClick
                        newCardClick.roundManager = FindObjectOfType<RoundManager>();
                        newCardClick.player = FindObjectOfType<Player>();
                    }

                    hand.Insert(cardIndex, newCard);
                    HideOtherCards();
                }
            }
        }
    }
}
