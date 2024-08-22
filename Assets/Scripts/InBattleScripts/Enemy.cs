using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense;
    public int magicDefense;
    public int attackDamage;
    public Slider healthSlider;
    public GameObject reticle;
    public GameObject selectedReticle;
    public GameObject damageTextPrefab;
    private WaveManager waveManager;
    private TutorialWaveManager tutorialWaveManager;

    [SerializeField] private ButtonManager buttonManager;

    public EnemyType enemyType;

    public int enemyCode;
    public static List<Enemy> AllEnemies = new List<Enemy>();

    public enum DamageType
    {
        Physical,
        Magical
    }

    public DamageType enemyDamageType;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        waveManager = FindObjectOfType<WaveManager>();
        tutorialWaveManager = FindObjectOfType<TutorialWaveManager>();
        healthSlider = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;

        defense = 20;
        magicDefense = 20;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        reticle.SetActive(false);
        selectedReticle.SetActive(false);
        AllEnemies.Add(this);
    }

    void OnMouseDown()
    {
        NewCardClick[] cards = FindObjectsOfType<NewCardClick>();
        foreach (NewCardClick card in cards)
        {
            if (card != null && card.isWaitingForTarget)
            {
                card.SelectEnemy(this);
                break;
            }
        }
    }
    public int GetEnemyCode()
    {
        return enemyCode;
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

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage * 100 / (100 + defense), 0);
        BackgroundShaker.Instance?.ShakeBackground();
        StartCoroutine(HandleDamage(actualDamage, false));
    }

    public void TakeMagicDamage(int magicDamage)
    {
        int actualMagicDamage = Mathf.Max(magicDamage * 100 / (100 + magicDefense), 0);
        BackgroundShaker.Instance?.ShakeBackground();
        StartCoroutine(HandleDamage(actualMagicDamage, true));
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

    IEnumerator UpdateHealthSlider(int oldHealth, int newHealth)
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

    public void Heal(int amount)
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

    public int CalculateDamage()
    {
        return attackDamage;
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        if (waveManager != null)
        {
            waveManager.OnEnemyDefeated();
            waveManager.RemoveEnemyInstance(gameObject);
        }
        else if (tutorialWaveManager != null)
        {
            tutorialWaveManager.OnEnemyDefeated();
            tutorialWaveManager.RemoveEnemyInstance(gameObject);
            Debug.LogWarning("WaveManager not found. Unable to process enemy defeat.");
        }
        Destroy(gameObject);
    }

    void ShowDamageText(int damage, bool isMagic)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageTextInstance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
            TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = damage.ToString() + (isMagic ? " MAGICAL" : " PHYSICAL");
                StartCoroutine(AnimateDamageText(damageTextInstance));
            }
        }
    }

    public static void HideAllReticles()
    {
        foreach (Enemy enemy in AllEnemies)
        {
            enemy.ShowReticle(false);
        }
    }

    IEnumerator AnimateDamageText(GameObject damageTextInstance)
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 startPos = damageTextInstance.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1f, 0);

        while (elapsedTime < duration)
        {
            damageTextInstance.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(damageTextInstance);
    }

    void OnDestroy()
    {
        AllEnemies.Remove(this);
    }
}