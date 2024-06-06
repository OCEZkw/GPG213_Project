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
    public int attackDamage; // Add this variable for the enemy's attack damage
    public Slider healthSlider;

    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;
        defense = 20;  // Initial defense value
        magicDefense = 20;  // Initial magic defense value

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public int CalculateDamage()
    {
        // Calculate and return the damage dealt by the enemy
        return attackDamage;
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage * 100 / (100 + defense), 0);
        StartCoroutine(UpdateHealthSlider(currentHealth, currentHealth - actualDamage));
        currentHealth -= actualDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
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

    public void TakeMagicDamage(int magicDamage)
    {
        int actualMagicDamage = Mathf.Max(magicDamage * 100 / (100 + magicDefense), 0);
        StartCoroutine(UpdateHealthSlider(currentHealth, currentHealth - actualMagicDamage));
        currentHealth -= actualMagicDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        if (currentHealth == 0)
        {
            Die();
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

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}