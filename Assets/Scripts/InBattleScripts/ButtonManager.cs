using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance { get; private set; }

    public Button confirmButton;  // Reference to the confirm button

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
    }

    public void ShowConfirmButton(bool show)
    {
        confirmButton.gameObject.SetActive(show);
    }

    private void OnConfirmButtonClick()
    {
        ConfirmHandler.Instance.ConfirmCard();
    }
}
