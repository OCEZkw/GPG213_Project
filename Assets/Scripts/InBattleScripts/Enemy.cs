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
    public GameObject damageTextPrefab;

    private bool isSelected = false;
    [SerializeField] private ButtonManager buttonManager;


    public enum DamageType
    {
        Physical,
        Magical
    }

    public DamageType enemyDamageType;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
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
    }

    void OnMouseDown()
    {
        if (CardClickHandler.selectedCards.Count > 0)
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                if (enemy != this)
                {
                    enemy.Deselect();
                }
            }

            isSelected = true;
            ShowReticle(false);
            if (buttonManager != null)
            {
                Debug.Log("ButtonManager found. Calling ShowSelectTargetButton(false).");
                buttonManager.ShowSelectTargetButton(false);
                buttonManager.ShowConfirmButton(true);
            }
            else
            {
                Debug.LogWarning("ButtonManager is null. Unable to call ShowSelectTargetButton.");
            }
            ConfirmHandler.selectedEnemy = this;
            CardClickHandler.selectedEnemy = this;
        }
    }

    public void Deselect()
    {
        isSelected = false;
        ShowReticle(false);
    }

    public void ShowReticle(bool show)
    {
        reticle.SetActive(show);
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage * 100 / (100 + defense), 0);
        StartCoroutine(HandleDamage(actualDamage, false));
    }

    public void TakeMagicDamage(int magicDamage)
    {
        int actualMagicDamage = Mathf.Max(magicDamage * 100 / (100 + magicDefense), 0);
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
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnEnemyDefeated();
        }
        FindObjectOfType<EnemySpawner>().RemoveEnemyInstance(gameObject);
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
}