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
    public TextMeshProUGUI healText;
    public GameObject reticle;

    public Vector3 damageTextOffset = new Vector3(0, 2, 0);
    public Vector3 healTextOffset = new Vector3(0, 2, 0);

    private Coroutine healthSliderCoroutine;

    private bool isSelected = false;
    [SerializeField] private ButtonManager buttonManager;

    void Start()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
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

        if (healText != null)
        {
            healText.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (CardClickHandler.selectedCards.Count > 0)
        {
            Player[] players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                if (player != this)
                {
                    player.Deselect();
                }
            }

            isSelected = true;
            ShowReticle(false);

            if (buttonManager != null)
            {
                buttonManager.ShowSelectTargetButton(false);
                buttonManager.ShowConfirmButton(true);
            }
            else
            {
                Debug.LogWarning("ButtonManager is null. Unable to show Confirm Button.");
            }

            ConfirmHandler.selectedPlayer = this; // Corrected to use the class name
            CardClickHandler.selectedPlayer = this;
        }
    }

    public void Deselect()
    {
        isSelected = false;
        ShowReticle(false);

        if (buttonManager != null)
        {
            buttonManager.ShowConfirmButton(false);
        }
    }

    public void ConfirmAction()
    {
        // Check if a card is selected and player is selected
        if (CardClickHandler.selectedCards.Count > 0 && isSelected)
        {
            // Call ConfirmCard function from ConfirmHandler
            ConfirmHandler.Instance.ConfirmCard();
        }
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
        ShowDamageText(actualDamage, "PHYSICAL");
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
        ShowDamageText(actualMagicDamage, "MAGICAL");
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

        // Display heal text
        if (healText != null)
        {
            healText.gameObject.SetActive(true); // Ensure healText is active
            healText.text = $"{amount}";
            StartCoroutine(AnimateHealText());
        }
    }

    IEnumerator AnimateHealText()
    {
        if (healText != null)
        {
            healText.gameObject.SetActive(true);
            Vector3 initialPosition = healText.transform.position;
            Vector3 targetPosition = initialPosition + healTextOffset;
            float duration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                healText.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                Color color = healText.color;
                color.a = Mathf.Lerp(1, 0, t);
                healText.color = color;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            healText.gameObject.SetActive(false);
            healText.transform.position = initialPosition;
        }
    }

    public void IncreasePhysicalDefense(int amount)
    {
        defense += amount;
        NotificationManager.Instance.ShowNotification("Increased Player's Physical Defense");
    }

    public void IncreaseMagicalDefense(int amount)
    {
        magicDefense += amount;
        NotificationManager.Instance.ShowNotification("Increased Player's Magical Defense");
    }

    IEnumerator DestroyDefenseTextAfterDelay(GameObject defenseTextInstance)
    {
        yield return new WaitForSeconds(2f); // Wait for 1 second
        Destroy(defenseTextInstance);
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

    void ShowDamageText(int damage, string damageType)
    {
        if (damageTextPrefab != null)
        {
            // Apply the offset to the initial position
            Vector3 spawnPosition = transform.position + damageTextOffset;

            GameObject damageTextInstance = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, transform);
            TextMeshPro damageText = damageTextInstance.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = $"{damage} {damageType}";
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