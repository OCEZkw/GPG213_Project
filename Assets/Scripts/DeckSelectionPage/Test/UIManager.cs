using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Example function to load Level 1
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
}
