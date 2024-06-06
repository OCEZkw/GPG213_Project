using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;  // Reference to the player prefab
    public Transform playerSpawnPoint; // Reference to the player spawn point

    private GameObject playerInstance;  // Instance of the player

    void Start()
    {
        Debug.Log("PlayerSpawner Start() called.");
       // SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player...");
        if (playerPrefab != null && playerSpawnPoint != null && playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Player prefab or spawn point not set in PlayerSpawner.");
        }
    }

    public GameObject GetPlayerInstance()
    {
        return playerInstance;
    }
}