using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }

    public Button confirmButton;  // Reference to the confirm button
    public Button selectTargetButton;  // Reference to the select target button

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
        selectTargetButton.gameObject.SetActive(false);

        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        selectTargetButton.onClick.AddListener(OnSelectTargetButtonClick);
    }

    public void ShowConfirmButton(bool show)
    {
        confirmButton.gameObject.SetActive(show);
    }

    public void ShowSelectTargetButton(bool show)
    {
        selectTargetButton.gameObject.SetActive(show);
    }

    private void OnConfirmButtonClick()
    {
        ConfirmHandler.Instance.ConfirmCard();
    }

    private void OnSelectTargetButtonClick()
    {
        // Logic for handling select target button click
        // This can be customized based on your specific game logic
    }

}
