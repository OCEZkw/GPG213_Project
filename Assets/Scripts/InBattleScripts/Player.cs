using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense = 20;
    public int magicDefense = 20;
    public Slider healthSlider;
    public int currentCost;

    private Coroutine healthSliderCoroutine;

    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        currentCost = RoundManager.Instance.playerCost;
    }

    public bool HasEnoughCost(int cost)
    {
        return RoundManager.Instance.playerCost >= cost;
    }
    

    public void UpdateCost(int newCost)
    {
        currentCost = newCost;
        // Update any UI elements or other logic related to cost here
        RoundManager.Instance.UpdateUI();
    }

    public void ResetCost()
    {
        currentCost = RoundManager.Instance.playerCost;
        RoundManager.Instance.UpdateUI();
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
        float duration = 0.5f;
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