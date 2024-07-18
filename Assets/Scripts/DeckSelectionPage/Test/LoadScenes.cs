using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";

    private void SaveMainMenuPosition()
    {
        if (SceneManager.GetActiveScene().name == MainMenuSceneName)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                GameManager.Instance.SavePlayerPosition(player.transform.position, MainMenuSceneName);
            }
        }
    }

    public void LoadDeckSelectScene()
    {
        SaveMainMenuPosition();
        SceneManager.LoadScene("DeckSelecting");
    }

    public void LoadSummonMenu()
    {
        SaveMainMenuPosition();
        SceneManager.LoadScene("SummonShop");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
