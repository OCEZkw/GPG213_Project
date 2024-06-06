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
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
    }

    public GameObject GetPlayerInstance()
    {
        return playerInstance;
    }
}
