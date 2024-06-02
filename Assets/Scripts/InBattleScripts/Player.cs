using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense;
    public Slider healthSlider;

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
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage - defense, 0);
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add logic for player death
    }
}
