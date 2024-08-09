using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectManager : MonoBehaviour
{
    public string[] levelSceneNames; // Array of level scene names

    // Call this method when the start button is clicked
    public void StartLevel()
    {
        // Get the selected level index from PlayerPrefs
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 0);

        // Load the corresponding level scene
        if (selectedLevel < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[selectedLevel]);
        }
        else
        {
            Debug.LogError("Invalid level index: " + selectedLevel);
        }
    }
}
