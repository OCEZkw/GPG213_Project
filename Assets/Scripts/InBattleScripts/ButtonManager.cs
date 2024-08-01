using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }
    public Button confirmButton;  // Reference to the confirm button

    private ConfirmHandler regularConfirmHandler;
    private TutorialConfirmHandler tutorialConfirmHandler;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        confirmButton.gameObject.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);

        // Find the appropriate confirm handler
        regularConfirmHandler = FindObjectOfType<ConfirmHandler>();
        tutorialConfirmHandler = FindObjectOfType<TutorialConfirmHandler>();

        if (regularConfirmHandler == null && tutorialConfirmHandler == null)
        {
            Debug.LogWarning("No ConfirmHandler or TutorialConfirmHandler found in the scene.");
        }
    }

    public void ShowConfirmButton(bool show)
    {
        confirmButton.gameObject.SetActive(show);
    }

    private void OnConfirmButtonClick()
    {
        if (tutorialConfirmHandler != null)
        {
            tutorialConfirmHandler.ConfirmCard();
        }
        else if (regularConfirmHandler != null)
        {
            regularConfirmHandler.ConfirmCard();
        }
        else
        {
            Debug.LogError("No ConfirmHandler or TutorialConfirmHandler available.");
        }
    }
}
