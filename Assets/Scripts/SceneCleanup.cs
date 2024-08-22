using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCleanup : MonoBehaviour
{
    public string[] objectsToDestroy;

    private void Start()
    {
        StartCoroutine(CleanupCoroutine());
    }

    private IEnumerator CleanupCoroutine()
    {
        // Wait for the end of the frame to ensure all objects are loaded
        yield return new WaitForEndOfFrame();

        foreach (string objectName in objectsToDestroy)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                Debug.Log($"Destroying object: {objectName}");
                Destroy(obj);
            }
            else
            {
                Debug.Log($"Object not found: {objectName}");
            }
        }

        // Check for objects in DontDestroyOnLoad
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad" && System.Array.Exists(objectsToDestroy, name => obj.name == name))
            {
                Debug.Log($"Destroying DontDestroyOnLoad object: {obj.name}");
                Destroy(obj);
            }
        }

        Debug.Log("Cleanup completed");
    }
}
