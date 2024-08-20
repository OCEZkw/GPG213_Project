using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InBattleScripts;

public class BossPart : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense;
    public int magicDefense;
    public int enemyCode;

    public EnemyType bossPartType; // Add this line

    public Slider healthSlider;
    public GameObject reticle;
    public GameObject selectedReticle;
    public GameObject damageTextPrefab;

    public bool IsDead => currentHealth <= 0;
    private List<GameObject> activeDamageTexts = new List<GameObject>();

    public enum DamageType
    {
        Physical,
        Magical
    }

    public DamageType bossDamageType;
    public int attackDamage;
    public int magicDamage;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        reticle.SetActive(false);
        selectedReticle.SetActive(false);
    }

    void OnMouseDown()
    {
        NewCardClick[] cards = FindObjectsOfType<NewCardClick>();
        foreach (NewCardClick card in cards)
        {
            if (card != null && card.isWaitingForTarget)
            {
                card.SelectBossPart(this);
                break;
            }
        }
    }

    public void Initialize(int initialHealth, EnemyType type)
    {
        maxHealth = initialHealth;
        currentHealth = maxHealth;
        bossPartType = type; // Initialize the enemy type

        defense = 20;
        magicDefense = 20;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public virtual void TakeDamage(int damage, bool isMagic)
    {
        int actualDamage = isMagic ? Mathf.Max(damage * 100 / (100 + magicDefense), 0) : Mathf.Max(damage * 100 / (100 + defense), 0);
        StartCoroutine(HandleDamage(actualDamage, isMagic));
    }

    IEnumerator HandleDamage(int damage, bool isMagic)
    {
        int newHealth = currentHealth - damage;
        if (newHealth < 0)
        {
            newHealth = 0;
        }
        ShowDamageText(damage, isMagic);
        yield return StartCoroutine(UpdateHealthSlider(currentHealth, newHealth));

        currentHealth = newHealth;
        if (currentHealth == 0)
        {
            Die();
        }
    }

    protected IEnumerator UpdateHealthSlider(int oldHealth, int newHealth)
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

    public void ShowReticle(bool show)
    {
        reticle.SetActive(show);
    }

    public void ShowSelectedReticle(bool show)
    {
        selectedReticle.SetActive(show);
        selectedReticle.GetComponent<ReticleScaleAnimation>()?.PlayAnimation(show);
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        StartCoroutine(UpdateHealthSlider(currentHealth - amount, currentHealth));
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    public void IncreaseMagicDefense(int amount)
    {
        magicDefense += amount;
    }

    protected virtual void Die()
    {
        Debug.Log("Boss part died!");
        // Destroy all active damage texts before disabling the GameObject
        DestroyAllDamageTexts();

        // Add any additional logic for when a boss part dies
        if (healthSlider != null)
        {
            gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    void ShowDamageText(int damage, bool isMagic)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageTextInstance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = damage.ToString() + (isMagic ? " MAGICAL" : " PHYSICAL");
                activeDamageTexts.Add(damageTextInstance);
                StartCoroutine(AnimateDamageText(damageTextInstance));
            }
        }
    }

    IEnumerator AnimateDamageText(GameObject damageTextInstance)
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

    void DestroyAllDamageTexts()
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

    public void IncreaseMagicDamage(int amount)
    {
        magicDamage += amount;
        NotificationManager.Instance.ShowNotification("Wizard Increased Its Staff's Magic Damage");
    }
}