using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public GameObject notificationObject; // Reference to the existing notification GameObject in the Canvas
    public GameObject notificationObject2;
    private TextMeshProUGUI notificationText;
    private TextMeshProUGUI notificationText2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (notificationObject != null)
        {
            notificationText = notificationObject.GetComponentInChildren<TextMeshProUGUI>();
            notificationObject.SetActive(false); // Disable the notification GameObject at the start
        }
        else
        {
            Debug.LogWarning("Notification object is not assigned in the Inspector.");
        }

        if(notificationObject2 != null)
        {
            notificationText2 = notificationObject2.GetComponentInChildren<TextMeshProUGUI>();
            notificationObject2.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Notification object 2 is not assigned in the Inspector");
        }
    }

    public void ShowNotification(string message)
    {
        if (notificationObject != null && notificationText != null)
        {
            notificationText.text = message;
            notificationObject.SetActive(true);
            StartCoroutine(HideNotification());
        }
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        notificationObject.SetActive(false);
    }

    public void ShowTargetSelectionNotification(bool show)
    {
        if (notificationObject2 != null)
        {
            notificationObject2.SetActive(show);
            if (show)
            {
                StartCoroutine(BlinkingBreathingEffect());
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

    private IEnumerator BlinkingBreathingEffect()
    {
        float minScale = 0.8f;
        float maxScale = 1.2f;
        float speed = 2f;

        while (true)
        {
            // Breathing effect
            for (float t = 0; t < 1; t += Time.deltaTime * speed)
            {
                float scale = Mathf.Lerp(minScale, maxScale, t);
                notificationObject2.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            for (float t = 0; t < 1; t += Time.deltaTime * speed)
            {
                float scale = Mathf.Lerp(maxScale, minScale, t);
                notificationObject2.transform.localScale = Vector3.one * scale;
                yield return null;
            }

            // Blinking effect
            notificationObject2.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            notificationObject2.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
