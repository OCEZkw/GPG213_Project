using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public void SavePlayerPosition(Vector3 position, string sceneName)
    {
        PlayerPrefs.SetFloat(sceneName + "_PosX", position.x);
        PlayerPrefs.SetFloat(sceneName + "_PosY", position.y);
        PlayerPrefs.SetFloat(sceneName + "_PosZ", position.z);
        PlayerPrefs.Save();
    }

    public Vector3 GetSavedPlayerPosition(string sceneName)
    {
        float x = PlayerPrefs.GetFloat(sceneName + "_PosX", 0f);
        float y = PlayerPrefs.GetFloat(sceneName + "_PosY", 0f);
        float z = PlayerPrefs.GetFloat(sceneName + "_PosZ", 0f);
        return new Vector3(x, y, z);
    }

    public bool HasSavedPosition(string sceneName)
    {
        return PlayerPrefs.HasKey(sceneName + "_PosX");
    }
}
