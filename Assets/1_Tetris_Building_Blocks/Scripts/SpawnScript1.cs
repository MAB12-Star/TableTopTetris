using UnityEngine;
using static CubeMovement;

public class SpawnScript1 : MonoBehaviour
{
    public GameObject[] cubePrefabs; // Array to hold multiple cube prefab
    public GameObject stoppingPlane; // Reference to the stopping plane
    public float fallSpeed = 1.0f;
    private Grid1 grid;

    void Start()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube1");

        if (boundaryCube != null)
        {
            grid = boundaryCube.GetComponent<Grid1>();

            if (grid != null)
            {
                SpawnNewBlock();
            }
            else
            {
                Debug.LogError("Grid component not found or inactive on Boundary_Cube1.");
            }
        }
        else
        {
            Debug.LogError("Boundary_Cube1 not found in the scene.");
        }
    }

    public void SpawnNewBlock()
    {
        if (cubePrefabs == null || cubePrefabs.Length == 0)
        {
            Debug.LogError("No cube prefabs assigned to the spawner.");
            return;
        }

        GameObject boundaryCube = GameObject.Find("Boundary_Cube1");
        if (boundaryCube == null)
        {
            Debug.LogError("Boundary_Cube1 not found in the scene.");
            return;
        }

        Grid1 grid = boundaryCube.GetComponent<Grid1>();
        if (grid == null)
        {
            Debug.LogError("Grid component not found on Boundary_Cube1.");
            return;
        }

        // Calculate the spawn position at the top center of the grid
        // This assumes the grid's width and depth are centered around the Boundary_Cube's position
        Vector3 spawnPosition = new Vector3(
            boundaryCube.transform.position.x + (boundaryCube.GetComponent<BoxCollider>().size.x / grid.width) * 0.5f, // Dynamically adjust based on Boundary_Cube1's width and grid size
            boundaryCube.transform.position.y + (boundaryCube.GetComponent<BoxCollider>().size.y / 2.0f), // Spawn at the top of the grid
            boundaryCube.transform.position.z + (boundaryCube.GetComponent<BoxCollider>().size.z / grid.depth) * 0.5f // Dynamically adjust based on Boundary_Cube1's depth and grid size
        );

        // Randomly choose a cube prefab
        GameObject randomPrefab = cubePrefabs[Random.Range(0, cubePrefabs.Length)];

        // Instantiate the chosen cube prefab at the calculated position
        // Parent it under the Boundary_Cube1 to maintain a clean hierarchy and ensure it moves with the Boundary_Cube1 if needed
        GameObject spawnedObject = Instantiate(randomPrefab, spawnPosition, Quaternion.identity, boundaryCube.transform);

        // Adjust the spawned object's position to align with the grid
        AlignObjectToGrid(spawnedObject, grid);
    }

    void AlignObjectToGrid(GameObject obj, Grid1 grid)
    {
        // Retrieve the object's size
        Vector3 objectSize = ObjectSizeConstants.GetObjectSize(obj.name);
        Debug.Log("Object Position: " + obj.transform.position);
        Debug.Log("Object Size: " + objectSize);
        // Calculate the aligned position based on the grid size
        Vector3 alignedPosition = new Vector3(
            Mathf.Round(obj.transform.position.x / grid.width) * grid.width + objectSize.x / 2f,
            obj.transform.position.y, // Keep the same Y-position
            Mathf.Round(obj.transform.position.z / grid.depth) * grid.depth + objectSize.z / 2f
        );

        // Set the object's position to the aligned position
        obj.transform.position = alignedPosition;
    }

}


