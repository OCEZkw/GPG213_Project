using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public float cardFadeOutDuration = 0.5f;
    public float cardSlideDownDuration = 0.5f;
    public float cardFadeInDuration = 0.5f;
    public float cardMoveUpDistance = 100f;

    public GameObject deckCardObject; // Reference to the deck card object
    public GameObject movingCardObject; // Reference to the moving card object
    public float cardAnimationDuration = 1f; // Duration of the card animation

    private List<GameObject> usedCards = new List<GameObject>();
    private WaveManager waveManager;
    private bool shouldStartNextWave = false;

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

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
           // waveManager.StartNextWave();
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
        foreach (GameObject cardObject in selectedCards)
        {
            HideOtherCards();
            yield return StartCoroutine(AnimateCardConfirmation(cardObject));

            cardObject.transform.position = confirmedCardPosition.position;
            cardObject.GetComponent<Collider2D>().enabled = false;


            NewCardClick cardClick = cardObject.GetComponent<NewCardClick>();
            CardEffect cardEffect = cardObject.GetComponent<CardEffect>();

            if (cardClick.GetSelectedEnemy() != null &&
                (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                Enemy enemyTarget = cardClick.GetSelectedEnemy().GetComponent<Enemy>();
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    Enemy.HideAllReticles();
                    enemyTarget.ShowSelectedReticle(true);
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
            else if (cardClick.GetSelectedFlowerEnemy() != null &&
                (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                FlowerEnemy flowerEnemy = cardClick.GetSelectedFlowerEnemy();
                if (player.HasEnoughCost(cardEffect.cost))
                {
                    buttonManager.ShowConfirmButton(false);
                    flowerEnemy.ShowReticle(false);
                    flowerEnemy.ShowSelectedReticle(false);
                    yield return StartCoroutine(UseConfirmedCard(cardObject, cardEffect, flowerEnemy.gameObject));
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

        if (waveManager.enemiesRemainingAlive > 0)
        {
            // Perform enemy actions only if there are enemies left
            yield return StartCoroutine(PerformEnemyActions());
        }
        else
        {
            // Set the flag to start the next wave in the next round
            shouldStartNextWave = true;
            Debug.Log("All enemies defeated. Next wave will start in the next round.");
        }


        StartNextRound();
    }

    IEnumerator PerformEnemyActions()
    {
        // Perform FlowerEnemy actions
        FlowerEnemy[] flowerEnemies = FindObjectsOfType<FlowerEnemy>();
        foreach (FlowerEnemy flowerEnemy in flowerEnemies)
        {
            flowerEnemy.PerformAction();
            yield return new WaitForSeconds(0.5f); // Short delay between flower actions
        }

        foreach (GameObject enemyObject in waveManager.GetEnemyInstances())
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                EnemyAttack(enemy);
                yield return new WaitForSeconds(2f); // Delay between enemy attacks
            }
        }

        // Retrieve player instance if needed (ensure it's updated)
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

        TripartiteBoss tripartiteBoss = FindObjectOfType<TripartiteBoss>();
        if (tripartiteBoss != null)
        {
            tripartiteBoss.BossAttackAfterPlayerActions(playerInstance.GetComponent<Player>());

            while (tripartiteBoss.IsPerformingActions)
            {
                yield return null;
            }
        }
    }

    IEnumerator UseConfirmedCard(GameObject cardObject, CardEffect cardEffect, GameObject target)
    {
        yield return new WaitForSeconds(2f); // Delay before card effect applies

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
            else if (target.CompareTag("FlowerEnemy") && (cardEffect.effectType == CardEffectType.AttackDamage || cardEffect.effectType == CardEffectType.MagicAttackDamage))
            {
                FlowerEnemy flowerEnemy = target.GetComponent<FlowerEnemy>();
                if (flowerEnemy != null)
                {
                    cardEffect.ApplyEffect(flowerEnemy.gameObject);
                }
            }
            else if (target.CompareTag("Player") && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    // player.UpdateCost(player.currentCost - cardEffect.cost);
                    cardEffect.ApplyEffect(player.gameObject);
                }
            }
        }

        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        cardRect.anchoredPosition = new Vector2(5000, 5000); // Move far off-screen

        cardObject.SetActive(false);
        usedCards.Add(cardObject);
        yield return new WaitForSeconds(1f); // Delay after card effect applies
    }

    private IEnumerator AnimateCardConfirmation(GameObject cardObject)
    {
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = cardObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = cardObject.AddComponent<CanvasGroup>();
        }

        // Disable CardFloatEffect
        CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
        if (floatEffect != null)
        {
            floatEffect.enabled = false;
            CardFloatManager.Instance.UnregisterCard(floatEffect);
        }

        // Get the canvas
        Canvas canvas = cardRect.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Cannot find canvas for the card.");
            yield break;
        }

        // Move up and fade out
        Vector2 startPos = cardRect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, cardMoveUpDistance);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(cardRect.DOAnchorPos(endPos, cardFadeOutDuration));
        sequence.Join(canvasGroup.DOFade(0, cardFadeOutDuration));

        yield return sequence.WaitForCompletion();

        // Convert world position to screen position
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, confirmedCardPosition.position);
        Vector2 confirmedAnchoredPosition;

        // Convert screen position to anchored position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPoint,
            canvas.worldCamera,
            out confirmedAnchoredPosition
        );

        // Move to confirmed position
        cardRect.anchoredPosition = confirmedAnchoredPosition + new Vector2(0, -cardMoveUpDistance);

        // Slide down and fade in
        sequence = DOTween.Sequence();
        sequence.Append(cardRect.DOAnchorPos(confirmedAnchoredPosition, cardSlideDownDuration));
        sequence.Join(canvasGroup.DOFade(1, cardFadeInDuration));

        yield return sequence.WaitForCompletion();
        // Return the final position of the card
        yield return confirmedAnchoredPosition;
    }

    void EnemyAttack(Enemy enemy)
    {
        if (enemy != null && playerInstance != null)
        {
            Player player = playerInstance.GetComponent<Player>();

            if (player != null)
            {
                int damage = enemy.CalculateDamage();

                // Get the EnemyAnimator component
                EnemyAnimator enemyAnimator = enemy.GetComponent<EnemyAnimator>();

                // Play the attack animation if the EnemyAnimator component exists
                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayAttackAnimation();
                }

                // Use DOTween to delay the damage application
                DOVirtual.DelayedCall(0.5f, () =>
                {
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

                    // The player's damage animation will be triggered inside the TakeDamage or TakeMagicDamage methods
                });
            }
        }
    }


    void StartNextRound()
    {
        ShowAllCards();
        ReplaceUsedCards();

        if (shouldStartNextWave)
        {
            Debug.Log("Starting next wave...");
            waveManager.StartNextWave();
            shouldStartNextWave = false; // Reset the flag
        }

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
            if (!NewCardClick.selectedCards.Contains(card))
            {
                HideCard(card);
                buttonManager.ShowConfirmButton(false);
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

    void ReplaceUsedCards()
    {
        StartCoroutine(ReplaceUsedCardsSimultaneously());
    }

    IEnumerator ReplaceUsedCardsSimultaneously()
    {
        List<GameObject> cardsToReplace = new List<GameObject>(usedCards);
        usedCards.Clear();

        List<(GameObject, int)> newCards = new List<(GameObject, int)>();

        foreach (GameObject usedCard in cardsToReplace)
        {
            int cardIndex = deckManager.hand.IndexOf(usedCard);
            if (cardIndex != -1)
            {
                deckManager.hand[cardIndex] = null; // Temporarily set to null to maintain indices
                Destroy(usedCard);

                if (deckManager.deck.Count > 0)
                {
                    List<GameObject> availableCards = new List<GameObject>(deckManager.allCards);

                    foreach (var card in deckManager.hand)
                    {
                        if (card != null)
                        {
                            availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                        }
                    }

                    if (availableCards.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availableCards.Count);
                        GameObject newCard = Instantiate(availableCards[randomIndex], deckManager.deckPosition.position, Quaternion.identity, deckManager.canvasTransform);
                        newCards.Add((newCard, cardIndex));
                    }
                }
            }
        }

        // Remove null entries from the hand
        deckManager.hand.RemoveAll(card => card == null);

        // Animate all new cards simultaneously
        List<Coroutine> animationCoroutines = new List<Coroutine>();
        foreach ((GameObject newCard, int cardIndex) in newCards)
        {
            animationCoroutines.Add(StartCoroutine(AnimateReplacementCard(newCard, cardIndex)));
        }

        // Wait for all animations to complete
        foreach (Coroutine coroutine in animationCoroutines)
        {
            yield return coroutine;
        }

        // Add all new cards to the hand at once
        foreach ((GameObject newCard, int cardIndex) in newCards)
        {
            deckManager.hand.Insert(cardIndex, newCard);
        }
    }

    IEnumerator ReplaceCardInHandCoroutine(GameObject usedCard)
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
                    GameObject newCard = Instantiate(availableCards[randomIndex], deckManager.deckPosition.position, Quaternion.identity, deckManager.canvasTransform);

                    yield return StartCoroutine(AnimateReplacementCard(newCard, cardIndex));
                }
            }
        }
    }

    IEnumerator AnimateReplacementCard(GameObject newCard, int cardIndex)
    {
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        RectTransform handPositionRect = deckManager.handPositions[cardIndex] as RectTransform;

        // Find the card front and back
        Image cardFront = newCard.transform.Find("CardFront").GetComponent<Image>();
        Image cardBack = newCard.transform.Find("CardBack").GetComponent<Image>();

        if (cardFront == null || cardBack == null)
        {
            Debug.LogError("Card front or back not found in prefab: " + newCard.name);
            yield break;
        }

        // Set up initial state
        cardRect.localScale = Vector3.one;
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(true);

        // Move to hand position
        yield return cardRect.DOAnchorPos(handPositionRect.anchoredPosition, deckManager.drawDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        // Set anchor and pivot before flipping
        cardRect.anchorMin = handPositionRect.anchorMin;
        cardRect.anchorMax = handPositionRect.anchorMax;
        cardRect.pivot = handPositionRect.pivot;
        cardRect.sizeDelta = handPositionRect.sizeDelta;

        // Flip card
        yield return cardRect.DORotate(new Vector3(0, 90, 0), deckManager.flipDuration / 2).SetEase(Ease.InOutQuad).OnComplete(() => {
            cardFront.gameObject.SetActive(true);
            cardBack.gameObject.SetActive(false);
        }).WaitForCompletion();

        yield return cardRect.DORotate(Vector3.zero, deckManager.flipDuration / 2).SetEase(Ease.InOutQuad).WaitForCompletion();

        // Setup card components
        SetupReplacementCard(newCard, cardIndex);
    }

    void SetupReplacementCard(GameObject card, int handIndex)
    {
        // Ensure the card has a BoxCollider2D and adjust its size
        BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = card.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = card.GetComponent<RectTransform>().sizeDelta;

        // Get the CardClickHandler component if it exists and set its properties
        var cardClickHandler = card.GetComponent<CardClickHandler>();
        if (cardClickHandler != null)
        {
            cardClickHandler.deckManager = deckManager;
            cardClickHandler.buttonManager = ButtonManager.Instance;
        }

        // Ensure the CardFloatEffect is present and active
        CardFloatEffect floatEffect = card.GetComponent<CardFloatEffect>();
        if (floatEffect == null)
        {
            floatEffect = card.AddComponent<CardFloatEffect>();
        }

        // Set the correct hand position for the CardFloatEffect
        RectTransform handPositionRect = deckManager.handPositions[handIndex] as RectTransform;
        floatEffect.Initialize(handPositionRect.anchoredPosition);
    }
}