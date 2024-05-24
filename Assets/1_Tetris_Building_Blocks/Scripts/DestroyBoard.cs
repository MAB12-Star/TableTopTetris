using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBoard : MonoBehaviour
{
    public GameObject prefabSpawnerPrefab; // Reference to the PrefabSpawner prefab

    // Method to destroy the parent object
    public void DestroyParentObject()
    {
        // Get the position where you want to instantiate the PrefabSpawner
        Vector3 spawnPosition = gameObject.transform.position;

        // Instantiate a new instance of the PrefabSpawner prefab
        Instantiate(prefabSpawnerPrefab, spawnPosition, Quaternion.identity);

        // Destroy the parent object of the GameObject this script is attached to
        Destroy(gameObject.transform.parent.gameObject);
    }


    public void ReactivatePrefab()
    {
        GameObject prefabSpawner = GameObject.Find("SimplePrefabSpawner(Clone)");
        if (prefabSpawner != null)
        {
            prefabSpawner.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PrefabSpawner(Clone) object not found in the scene.");
        }
    }
}
