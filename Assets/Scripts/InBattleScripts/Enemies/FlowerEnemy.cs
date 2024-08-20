using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowerEnemy : MonoBehaviour
{
    public enum FlowerType { Healing, CardLocking }
    public FlowerType flowerType;

    public EnemyType enemyType;

    public int maxHealth = 100;
    public int currentHealth;
    public int defense = 10;
    public int magicDefense = 10;
    public int healAmount = 50;

    public Slider healthSlider;
    public GameObject reticle;
    public GameObject selectedReticle;
    public GameObject damageTextPrefab;

    private TripartiteBoss tripartiteBoss;
    private DeckManager deckManager;
    private GameObject lockedCard;
    private WaveManager waveManager;
    private int spawnPointIndex = -1;

    public bool IsDead => currentHealth <= 0;

    public int enemyCode { get; set; }

    private List<GameObject> activeDamageTexts = new List<GameObject>();

    private void Start()
    {
        currentHealth = maxHealth;
        tripartiteBoss = FindObjectOfType<TripartiteBoss>();
        deckManager = FindObjectOfType<DeckManager>();
        waveManager = FindObjectOfType<WaveManager>();

        InitializeUI();
        Debug.Log($"FlowerEnemy of type {flowerType} initialized with {currentHealth} HP. Enemy Type: {enemyType}");
    }

    private void InitializeUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (reticle != null) reticle.SetActive(false);
        if (selectedReticle != null) selectedReticle.SetActive(false);
    }

    public void TakeDamage(int damage, CardType cardType)
    {
        int actualDamage = Mathf.Max(damage - defense, 1); // Ensure at least 1 damage is dealt
        StartCoroutine(HandleDamage(actualDamage, cardType));
    }

    private IEnumerator HandleDamage(int damage, CardType cardType)
    {
        int newHealth = currentHealth - damage;
        if (newHealth < 0)
        {
            newHealth = 0;
        }
        ShowDamageText(damage, cardType);
        yield return StartCoroutine(UpdateHealthSlider(currentHealth, newHealth));

        currentHealth = newHealth;
        if (currentHealth == 0)
        {
            Die();
        }
    }

    private IEnumerator UpdateHealthSlider(int oldHealth, int newHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            currentHealth = (int)Mathf.Lerp(oldHealth, newHealth, elapsedTime / duration);
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentHealth = newHealth;
        if (healthSlider != null)
        {
            healthSlider.value = newHealth;
        }
    }

    private void ShowDamageText(int damage, CardType cardType)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageTextInstance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = $"-{damage} {cardType.ToString().ToUpper()}";
                activeDamageTexts.Add(damageTextInstance);
                StartCoroutine(AnimateDamageText(damageTextInstance));
            }
        }
    }

    private IEnumerator AnimateDamageText(GameObject damageTextInstance)
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 startPos = damageTextInstance.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1f, 0);

        while (elapsedTime < duration && damageTextInstance != null)
        {
            damageTextInstance.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (damageTextInstance != null)
        {
            activeDamageTexts.Remove(damageTextInstance);
            Destroy(damageTextInstance);
        }
    }

    public void PerformAction()
    {
        if (flowerType == FlowerType.Healing && tripartiteBoss != null)
        {
            TripartiteBossPart mainBody = tripartiteBoss.mainBody;
            if (mainBody != null)
            {
                mainBody.Heal(healAmount);
                Debug.Log($"Healing Flower performed action: Healed boss main body for {healAmount} HP.");
            }
            else
            {
                Debug.LogWarning("Healing Flower couldn't find the main body of the boss.");
            }
        }
        else if (flowerType == FlowerType.CardLocking && deckManager != null && lockedCard == null)
        {
            lockedCard = deckManager.LockRandomCard();
            if (lockedCard != null)
            {
                Debug.Log($"Card-Locking Flower locked card: {lockedCard.name}");
            }
            else
            {
                Debug.Log("Card-Locking Flower couldn't lock any card.");
            }
        }
        else
        {
            Debug.LogWarning($"FlowerEnemy of type {flowerType} failed to perform action. TripartiteBoss: {tripartiteBoss}, DeckManager: {deckManager}");
        }
    }

    private void Die()
    {
        Debug.Log($"FlowerEnemy of type {flowerType} has been destroyed.");
        if (flowerType == FlowerType.CardLocking && deckManager != null && lockedCard != null)
        {
            deckManager.UnlockCard(lockedCard);
            Debug.Log($"Card-Locking Flower destroyed. Unlocked card: {lockedCard.name}");
        }

        // Notify about the freed spawn point
        if (tripartiteBoss != null)
        {
            tripartiteBoss.OnFlowerDestroyed(spawnPointIndex);
        }
        else if (waveManager != null)
        {
            waveManager.OnFlowerDestroyed(spawnPointIndex);
        }
        else
        {
            Debug.LogWarning("Neither TripartiteBoss nor WaveManager found to notify about freed spawn point.");
        }

        DestroyAllDamageTexts();
        Destroy(gameObject);
    }

    private void DestroyAllDamageTexts()
    {
        foreach (GameObject damageText in activeDamageTexts)
        {
            if (damageText != null)
            {
                Destroy(damageText);
            }
        }
        activeDamageTexts.Clear();
    }

    public void ShowReticle(bool show)
    {
        if (reticle != null) reticle.SetActive(show);
    }

    public void ShowSelectedReticle(bool show)
    {
        if (selectedReticle != null) selectedReticle.SetActive(show);
        selectedReticle.GetComponent<ReticleScaleAnimation>()?.PlayAnimation(show);
    }

    public int GetEnemyCode()
    {
        return enemyCode;
    }

    public void SetSpawnPointIndex(int index)
    {
        spawnPointIndex = index;
    }
}
