using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public string levelSelectSceneName = "MainMenu";

    private void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            LoadLevelSelectScene();
        }
    }

    private void LoadLevelSelectScene()
    {
        SceneManager.LoadScene(levelSelectSceneName);
    }
}
