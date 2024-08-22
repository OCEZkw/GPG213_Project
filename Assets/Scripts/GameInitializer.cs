using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject gachaSystemPrefab;
    public GameObject gachaManagerPrefab;
    public GameObject inventory;
    public GameObject inventoryItemVisualizer;
    public GameObject playerStats;

    private void Awake()
    {
        InitializeManager(gameManagerPrefab);
        InitializeManager(gachaSystemPrefab);
        InitializeManager(gachaManagerPrefab);
        InitializeManager(inventory);
        InitializeManager(inventoryItemVisualizer);
        InitializeManager(playerStats);

    }

    private void InitializeManager(GameObject managerPrefab)
    {
        if (managerPrefab != null)
        {
            var existingManager = FindObjectOfType(managerPrefab.GetComponent<MonoBehaviour>().GetType());
            if (existingManager == null)
            {
                Instantiate(managerPrefab);
            }
        }
        else
        {
            Debug.LogError($"Manager prefab is null: {managerPrefab.name}");
        }
    }
}
