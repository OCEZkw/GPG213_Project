using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class CardSummonSceneManager : MonoBehaviour
{
    public static List<CardSO> summonedCards;
    public Transform summonResultParent;
    public GameObject summonResultPrefab;
    public GameObject summonEffect1GameObject;  // GameObject with multiple child VFX
    public VisualEffect summonEffect2;
    public float summonEffectDuration1 = 2f;
    public float summonEffectDuration2 = 2f;
    public float delayBetweenEffects = 0.5f;
    public float delayBetweenCards = 1f;
    public Camera mainCamera;
    public float startRotation = 15f;
    public float endRotation = 2f;
    public float rotationSpeed = 2f;

    private GameObject currentCardObject;
    private bool waitingForInput = false;
    private bool isSummoning = false;
    private bool skipSummon = false;

    public float initialSpinSpeed = 1440f; // Degrees per second
    public float spinSlowdownRate = 0.5f; // How quickly the spin slows down
    public float minSpinSpeed = 20f; // Minimum spin speed before stopping
    public Sprite cardBackSprite;

    public Button skipButton;
    private bool isSkipping = false;

    private void Start()
    {
        // Ensure VFX are initially disabled
        summonEffect1GameObject.SetActive(false);
        summonEffect2.gameObject.SetActive(false);

        // Set initial camera rotation
        mainCamera.transform.rotation = Quaternion.Euler(startRotation, 0, 0);

        if (summonedCards != null && summonedCards.Count > 0)
        {
            StartCoroutine(DisplaySummonedCardsWithEffect());
        }
        else
        {
            Debug.LogWarning("No summoned cards to display.");
            ReturnToCardSummonSelectScene();
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipAllAnimations);
        }
        else
        {
            Debug.LogWarning("Skip button not assignd in the inspector");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is left mouse button
        {
            if (isSummoning)
            {
                skipSummon = true;
            }
            else if (waitingForInput)
            {
                waitingForInput = false;
            }
        }
    }

    private IEnumerator DisplaySummonedCardsWithEffect()
    {
        if (skipSummon)
        {
            DisplayAllCardsImmediately();
            yield break;
        }

        foreach (CardSO card in summonedCards)
        {
            yield return StartCoroutine(PlaySummonEffectsForCard(card));

            // Reset camera rotation if skipped
            if (skipSummon)
            {
                mainCamera.transform.rotation = Quaternion.Euler(endRotation, 0, 0);
            }

            // Wait for user input
            waitingForInput = true;
            yield return new WaitUntil(() => !waitingForInput);

            if (currentCardObject != null)
            {
                Destroy(currentCardObject);
                currentCardObject = null;
            }
        }

        // Clear the static variable after use
        summonedCards = null;

        // Reset camera rotation at the end
        mainCamera.transform.rotation = Quaternion.Euler(startRotation, 0, 0);

        // Wait for a short delay before returning to the CardSummonSelect scene
        yield return new WaitForSeconds(1f);

        // Return to the CardSummonSelect scene
        ReturnToCardSummonSelectScene();
    }


    private IEnumerator PlaySummonEffectsForCard(CardSO card)
    {
        isSummoning = true;
        skipSummon = false;

        if (!skipSummon)
        {
            yield return StartCoroutine(PlayFirstSummonEffect());
            if (skipSummon) goto SkipToCard;

            yield return new WaitForSeconds(delayBetweenEffects);
            if (skipSummon) goto SkipToCard;

            yield return StartCoroutine(PlaySecondSummonEffect());
            if (skipSummon) goto SkipToCard;
        }

    SkipToCard:
        isSummoning = false;
        currentCardObject = DisplaySummonedCard(card);
        yield return StartCoroutine(RevealCard(currentCardObject));
        GachaSystem.Instance.AddCardToInventory(card);
    }

    private IEnumerator PlayFirstSummonEffect()
    {
        // Reset camera rotation to 15 degrees
        mainCamera.transform.rotation = Quaternion.Euler(startRotation, 0, 0);

        summonEffect1GameObject.SetActive(true);

        VisualEffect[] childEffects = summonEffect1GameObject.GetComponentsInChildren<VisualEffect>();
        foreach (VisualEffect effect in childEffects)
        {
            effect.Play();
        }

        float elapsedTime = 0f;
        while (elapsedTime < summonEffectDuration1 && !skipSummon)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (VisualEffect effect in childEffects)
        {
            effect.Stop();
        }

        summonEffect1GameObject.SetActive(false);
    }

    private IEnumerator PlaySecondSummonEffect()
    {
        summonEffect2.gameObject.SetActive(true);
        summonEffect2.Play();

        float elapsedTime = 0f;
        Quaternion startRotation = Quaternion.Euler(this.startRotation, 0, 0);
        Quaternion endRotation = Quaternion.Euler(this.endRotation, 0, 0);

        float rotationDuration = summonEffectDuration2 / 7f;

        while (elapsedTime < summonEffectDuration2 && !skipSummon)
        {
            if (elapsedTime < rotationDuration)
            {
                mainCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / rotationDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.rotation = endRotation;

        summonEffect2.Stop();
        summonEffect2.gameObject.SetActive(false);
    }

    private GameObject DisplaySummonedCard(CardSO card)
    {
        GameObject resultObject = Instantiate(summonResultPrefab, summonResultParent);
        resultObject.SetActive(false); // Start with the card hidden

        // Remove any existing Image component on the root object
        Image existingImage = resultObject.GetComponent<Image>();
        if (existingImage != null)
        {
            Destroy(existingImage);
        }

        // Create a child object for the front of the card
        GameObject frontObject = new GameObject("CardFront");
        frontObject.transform.SetParent(resultObject.transform, false);
        Image frontImage = frontObject.AddComponent<Image>();
        frontImage.sprite = card.cardSprite;
        frontImage.color = Color.clear; // Start fully transparent
        SetupImageComponent(frontImage);

        // Create a child object for the back of the card
        GameObject backObject = new GameObject("CardBack");
        backObject.transform.SetParent(resultObject.transform, false);
        Image backImage = backObject.AddComponent<Image>();
        backImage.sprite = cardBackSprite;
        backImage.color = Color.white; // Start fully opaque
        SetupImageComponent(backImage);

        Text cardNameText = resultObject.GetComponentInChildren<Text>();
        if (cardNameText != null)
        {
            cardNameText.text = card.cardName;
            cardNameText.color = Color.clear; // Start with transparent text
        }

        return resultObject;
    }

    private void SetupImageComponent(Image image)
    {
        // Stretch the image to fill its container
        image.rectTransform.anchorMin = Vector2.zero;
        image.rectTransform.anchorMax = Vector2.one;
        image.rectTransform.sizeDelta = Vector2.zero;
        image.rectTransform.anchoredPosition = Vector2.zero;

        // Set the image to stretch and preserve aspect ratio
        image.preserveAspect = true;
    }


    private IEnumerator RevealCard(GameObject cardObject)
    {
        cardObject.SetActive(true);
        Image frontImage = cardObject.transform.Find("CardFront").GetComponent<Image>();
        Image backImage = cardObject.transform.Find("CardBack").GetComponent<Image>();

        float currentSpinSpeed = initialSpinSpeed;
        float elapsedTime = 0f;
        float revealDuration = 3f; // Adjust this value as needed

        Vector3 smallScale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 largeScale = new Vector3(1.2f, 1.2f, 1.2f);

        cardObject.transform.localScale = smallScale;

        while (elapsedTime < revealDuration && !skipSummon)
        {
            cardObject.transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
            currentSpinSpeed = Mathf.Max(currentSpinSpeed * (1f - spinSlowdownRate * Time.deltaTime), minSpinSpeed);

            // Fade out the back and fade in the front based on rotation
            float fadeProgress = Mathf.PingPong(cardObject.transform.eulerAngles.y, 180f) / 180f;
            backImage.color = Color.Lerp(Color.white, Color.clear, fadeProgress);
            frontImage.color = Color.Lerp(Color.clear, Color.white, fadeProgress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the card ends up showing the front
        frontImage.color = Color.white;
        backImage.color = Color.clear;
        cardObject.transform.rotation = Quaternion.identity;

        // Pop animation
        float popDuration = 0.3f;
        float popElapsedTime = 0f;

        while (popElapsedTime < popDuration)
        {
            float t = popElapsedTime / popDuration;
            float easeOut = 1 - (1 - t) * (1 - t);
            cardObject.transform.localScale = Vector3.Lerp(smallScale, largeScale, easeOut);

            popElapsedTime += Time.deltaTime;
            yield return null;
        }

        cardObject.transform.localScale = Vector3.one;

        waitingForInput = true;
        yield return new WaitUntil(() => !waitingForInput);
    }

    private void SkipAllAnimations()
    {
        if (isSkipping) return; // Prevent multiple skips
        isSkipping = true;

        skipSummon = true;
        StopAllCoroutines();
        StartCoroutine(SkipAnimationsCoroutine());
    }

    private IEnumerator SkipAnimationsCoroutine()
    {
        // Disable the skip button to prevent further clicks
        if (skipButton != null)
        {
            skipButton.interactable = false;
        }

        yield return StartCoroutine(DisplayAllCardsImmediately());

        // Re-enable the skip button
        if (skipButton != null)
        {
            skipButton.interactable = true;
        }

        isSkipping = false;
    }

    private IEnumerator DisplayAllCardsImmediately()
    {
        if (currentCardObject != null)
        {
            Destroy(currentCardObject);
        }

        foreach (CardSO card in summonedCards)
        {
            GameObject cardObject = DisplaySummonedCard(card);
            cardObject.SetActive(true);
            Image frontImage = cardObject.transform.Find("CardFront").GetComponent<Image>();
            Image backImage = cardObject.transform.Find("CardBack").GetComponent<Image>();

            frontImage.color = Color.white;
            backImage.color = Color.clear;
            cardObject.transform.rotation = Quaternion.identity;
            cardObject.transform.localScale = Vector3.one;

            GachaSystem.Instance.AddCardToInventory(card);

            // Add a small delay between displaying each card
            yield return new WaitForSeconds(0.1f);
        }

        // Clear the static variable after use
        summonedCards = null;

        // Reset camera rotation
        mainCamera.transform.rotation = Quaternion.Euler(startRotation, 0, 0);

        // Return to the CardSummonSelect scene after a short delay
        yield return new WaitForSeconds(1f);
        ReturnToCardSummonSelectScene();
    }

    private IEnumerator ReturnToCardSummonSelectWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToCardSummonSelectScene();
    }

    private void ReturnToCardSummonSelectScene()
    {
        SceneManager.LoadScene("SummonShop");
    }
}
