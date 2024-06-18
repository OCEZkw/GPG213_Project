using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public GameObject notificationObject; // Reference to the existing notification GameObject in the Canvas
    private TextMeshProUGUI notificationText;

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
}
