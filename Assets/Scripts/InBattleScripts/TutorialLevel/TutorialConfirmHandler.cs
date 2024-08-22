using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public float cardFadeOutDuration = 0.5f;
    public float cardSlideDownDuration = 0.5f;
    public float cardFadeInDuration = 0.5f;
    public float cardMoveUpDistance = 100f;

    public GameObject deckCardObject;
    public GameObject movingCardObject;
    public float cardAnimationDuration = 1f;

    private List<GameObject> usedCards = new List<GameObject>();
    private TutorialWaveManager tutorialWaveManager;
    private bool isFirstRound = true;

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
        tutorialWaveManager = FindObjectOfType<TutorialWaveManager>();
        if (tutorialWaveManager == null)
        {
            Debug.LogError("WaveManager not found in the scene!");
        }
        else
        {
            // Start the first wave immediately for the tutorial
           // tutorialWaveManager.StartNextWave();
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
            yield return StartCoroutine(AnimateCardConfirmation(cardObject));

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

        if (tutorialWaveManager != null && tutorialWaveManager.enemiesRemainingAlive > 0)
        {
            // Perform enemy actions only if there are enemies left
            yield return StartCoroutine(PerformEnemyActions());
        }

        StartNextRound();
    }

    IEnumerator PerformEnemyActions()
    {
        foreach (GameObject enemyObject in tutorialWaveManager.GetEnemyInstances())
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                EnemyAttack(enemy);
                yield return new WaitForSeconds(2f); // Delay between enemy attacks
            }
        }
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
            else if (target.CompareTag("Player") && (cardEffect.effectType == CardEffectType.Healing || cardEffect.effectType == CardEffectType.Defense))
            {
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    cardEffect.ApplyEffect(player.gameObject);
                }
            }
        }

        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        cardRect.anchoredPosition = new Vector2(5000, 5000); // Move far off-screen

        cardObject.SetActive(false);
        usedCards.Add(cardObject);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator AnimateCardConfirmation(GameObject cardObject)
    {
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = cardObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = cardObject.AddComponent<CanvasGroup>();
        }

        CardFloatEffect floatEffect = cardObject.GetComponent<CardFloatEffect>();
        if (floatEffect != null)
        {
            floatEffect.enabled = false;
            CardFloatManager.Instance.UnregisterCard(floatEffect);
        }

        Canvas canvas = cardRect.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Cannot find canvas for the card.");
            yield break;
        }

        Vector2 startPos = cardRect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, cardMoveUpDistance);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(cardRect.DOAnchorPos(endPos, cardFadeOutDuration));
        sequence.Join(canvasGroup.DOFade(0, cardFadeOutDuration));

        yield return sequence.WaitForCompletion();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, confirmedCardPosition.position);
        Vector2 confirmedAnchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPoint,
            canvas.worldCamera,
            out confirmedAnchoredPosition
        );

        cardRect.anchoredPosition = confirmedAnchoredPosition + new Vector2(0, -cardMoveUpDistance);

        sequence = DOTween.Sequence();
        sequence.Append(cardRect.DOAnchorPos(confirmedAnchoredPosition, cardSlideDownDuration));
        sequence.Join(canvasGroup.DOFade(1, cardFadeInDuration));

        yield return sequence.WaitForCompletion();
    }

    void EnemyAttack(Enemy enemy)
    {
        if (enemy != null && playerInstance != null)
        {
            Player player = playerInstance.GetComponent<Player>();

            if (player != null)
            {
                int damage = enemy.CalculateDamage();

                EnemyAnimator enemyAnimator = enemy.GetComponent<EnemyAnimator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.PlayAttackAnimation();
                }

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
                });
            }
        }
    }

    void StartNextRound()
    {
        ShowAllCards();
        ReplaceUsedCards();

        if (tutorialWaveManager.ShouldStartNextWave())
        {
            Debug.Log("Starting next wave...");
            tutorialWaveManager.StartNextWave();
        }
        else
        {
            Debug.Log("Continuing current wave...");
        }

        RoundManager.Instance.StartNextRound();
        NewCardClick.selectedCards.Clear();
    }


    public void HideOtherCards()
    {
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
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
            int cardIndex = tutorialDeckManager.GetHand().IndexOf(usedCard);
            if (cardIndex != -1)
            {
                tutorialDeckManager.GetHand()[cardIndex] = null;
                Destroy(usedCard);

                if (tutorialDeckManager.tutorialDeck.Count > 0)
                {
                    List<GameObject> availableCards = new List<GameObject>(tutorialDeckManager.tutorialDeck);

                    foreach (var card in tutorialDeckManager.GetHand())
                    {
                        if (card != null)
                        {
                            availableCards.RemoveAll(c => c.name == card.name.Replace("(Clone)", ""));
                        }
                    }

                    if (availableCards.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availableCards.Count);
                        GameObject newCard = Instantiate(availableCards[randomIndex], tutorialDeckManager.deckPosition.position, Quaternion.identity, tutorialDeckManager.canvasTransform);
                        newCards.Add((newCard, cardIndex));
                    }
                }
            }
        }

        tutorialDeckManager.GetHand().RemoveAll(card => card == null);

        List<Coroutine> animationCoroutines = new List<Coroutine>();
        foreach ((GameObject newCard, int cardIndex) in newCards)
        {
            animationCoroutines.Add(StartCoroutine(AnimateReplacementCard(newCard, cardIndex)));
        }

        foreach (Coroutine coroutine in animationCoroutines)
        {
            yield return coroutine;
        }

        foreach ((GameObject newCard, int cardIndex) in newCards)
        {
            tutorialDeckManager.GetHand().Insert(cardIndex, newCard);
        }
    }

    IEnumerator AnimateReplacementCard(GameObject newCard, int cardIndex)
    {
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        RectTransform handPositionRect = tutorialDeckManager.handPositions[cardIndex] as RectTransform;

        Image cardFront = newCard.transform.Find("CardFront").GetComponent<Image>();
        Image cardBack = newCard.transform.Find("CardBack").GetComponent<Image>();

        if (cardFront == null || cardBack == null)
        {
            Debug.LogError("Card front or back not found in prefab: " + newCard.name);
            yield break;
        }

        cardRect.localScale = Vector3.one;
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(true);

        yield return cardRect.DOAnchorPos(handPositionRect.anchoredPosition, tutorialDeckManager.drawDuration).SetEase(Ease.OutQuad).WaitForCompletion();

        cardRect.anchorMin = handPositionRect.anchorMin;
        cardRect.anchorMax = handPositionRect.anchorMax;
        cardRect.pivot = handPositionRect.pivot;
        cardRect.sizeDelta = handPositionRect.sizeDelta;

        yield return cardRect.DORotate(new Vector3(0, 90, 0), tutorialDeckManager.flipDuration / 2).SetEase(Ease.InOutQuad).OnComplete(() => {
            cardFront.gameObject.SetActive(true);
            cardBack.gameObject.SetActive(false);
        }).WaitForCompletion();

        yield return cardRect.DORotate(Vector3.zero, tutorialDeckManager.flipDuration / 2).SetEase(Ease.InOutQuad).WaitForCompletion();

        SetupReplacementCard(newCard, cardIndex);
    }

    void SetupReplacementCard(GameObject card, int handIndex)
    {
        BoxCollider2D boxCollider = card.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = card.AddComponent<BoxCollider2D>();
        }
        boxCollider.size = card.GetComponent<RectTransform>().sizeDelta;

        var newCardClick = card.GetComponent<NewCardClick>();
        if (newCardClick != null)
        {
            newCardClick.deckManager = tutorialDeckManager;
            newCardClick.buttonManager = ButtonManager.Instance;
            newCardClick.roundManager = FindObjectOfType<RoundManager>();
            newCardClick.player = FindObjectOfType<Player>();
        }

        CardFloatEffect floatEffect = card.GetComponent<CardFloatEffect>();
        if (floatEffect == null)
        {
            floatEffect = card.AddComponent<CardFloatEffect>();
        }

        RectTransform handPositionRect = tutorialDeckManager.handPositions[handIndex] as RectTransform;
        floatEffect.Initialize(handPositionRect.anchoredPosition);
    }

}
