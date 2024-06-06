using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense;
    public int magicDefense;
    public int attackDamage;
    public Slider healthSlider;
    public GameObject reticle;  // Reference to the reticle GameObject

    private bool isSelected = false;
    [SerializeField]private ButtonManager buttonManager;

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
        if (CardClickHandler.selectedCard != null)
        {
            // Deselect other enemies
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                if (enemy != this)
                {
                    enemy.Deselect();
                }
            }

            // Select this enemy
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
        StartCoroutine(HandleDamage(actualDamage));
    }

    IEnumerator HandleDamage(int damage)
    {
        int newHealth = currentHealth - damage;
        if (newHealth < 0)
        {
            newHealth = 0;
        }

        yield return StartCoroutine(UpdateHealthSlider(currentHealth, newHealth));

        currentHealth = newHealth;
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void TakeMagicDamage(int magicDamage)
    {
        int actualMagicDamage = Mathf.Max(magicDamage * 100 / (100 + magicDefense), 0);
        StartCoroutine(HandleMagicDamage(actualMagicDamage));
    }

    IEnumerator HandleMagicDamage(int magicDamage)
    {
        int newHealth = currentHealth - magicDamage;
        if (newHealth < 0)
        {
            newHealth = 0;
        }

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
        float duration = 0.5f; // Duration of the health slider update

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
        // Calculate and return the damage dealt by the enemy
        return attackDamage;
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}