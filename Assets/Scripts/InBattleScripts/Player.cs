using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense = 20; // Initial defense value
    public int magicDefense = 20; // Initial magic defense value
    public Slider healthSlider;
    public int currentCost;

    private Coroutine healthSliderCoroutine;

    void Start()
    {
        // Find the slider component in the child objects
        healthSlider = GetComponentInChildren<Slider>();

        // Initialize health
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Initialize cost
        currentCost = RoundManager.Instance.playerCost;
    }

    public void UpdateCost(int newCost)
    {
        currentCost = newCost;
        // Update any UI elements or other logic related to cost here
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.RoundToInt(damage * 100f / (100f + defense));
        currentHealth -= actualDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthSlider();
        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void TakeMagicDamage(int magicDamage)
    {
        int actualMagicDamage = Mathf.RoundToInt(magicDamage * 100f / (100f + magicDefense));
        currentHealth -= actualMagicDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthSlider();
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
        UpdateHealthSlider();
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    public void IncreaseMagicDefense(int amount)
    {
        magicDefense += amount;
    }

    void UpdateHealthSlider()
    {
        if (healthSliderCoroutine != null)
        {
            StopCoroutine(healthSliderCoroutine);
        }
        healthSliderCoroutine = StartCoroutine(SmoothHealthSliderUpdate());
    }

    IEnumerator SmoothHealthSliderUpdate()
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation
        float startValue = healthSlider.value;
        float endValue = currentHealth;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = endValue;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add logic for player death
    }
}