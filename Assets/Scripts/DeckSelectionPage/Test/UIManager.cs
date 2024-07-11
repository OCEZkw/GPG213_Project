using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button levelLoadButton;
    void Start()
    {
        // Initially disable the button
        EnableLevelLoadButton(false);
    }
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
    public void EnableLevelLoadButton(bool enable)
    {
        levelLoadButton.interactable = enable;
    }
    public void LoadDeckSelectScene()
    {
        SceneManager.LoadScene("DeckSelecting");
    }
    public void LoadSummonMenu()
    {
        SceneManager.LoadScene("SummonShop");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
