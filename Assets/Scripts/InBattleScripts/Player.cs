using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int defense = 20;
    public int magicDefense = 20;
    public Slider healthSlider;
    public int currentCost;
    public GameObject damageTextPrefab;
    public TextMeshProUGUI healthText;
    public GameObject reticle;

    private Coroutine healthSliderCoroutine;

    void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();  // Assuming TextMeshProUGUI is a child of the Slider
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        UpdateHealthText();
        currentCost = RoundManager.Instance.playerCost;
        reticle.SetActive(false);
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
        ShowDamageText(actualDamage);
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
        ShowDamageText(actualMagicDamage);
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
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add logic for player death
    }

    void ShowDamageText(int damage)
    {
        if (damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 1, 0);  // Adjust the offset as needed
            GameObject damageTextInstance = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, transform);
            TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = damage.ToString();
            }
            StartCoroutine(AnimateDamageText(damageTextInstance));
        }
    }

    IEnumerator AnimateDamageText(GameObject damageTextInstance)
    {
        TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
        Vector3 initialPosition = damageTextInstance.transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(0, 1, 0);
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            damageTextInstance.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            Color color = damageText.color;
            color.a = Mathf.Lerp(1, 0, t);
            damageText.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(damageTextInstance);
    }

    public void ShowReticle(bool show)
    {
        if (reticle != null)
        {
            reticle.SetActive(show);
        }
    }
}