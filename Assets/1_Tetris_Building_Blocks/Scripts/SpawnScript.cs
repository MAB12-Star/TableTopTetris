using UnityEngine;
using UnityEngine.UI;
using static CubeMovement;

[System.Serializable]
public struct CubePrefabWithPreview
{
    public GameObject cubePrefab;
    public GameObject previewObject; // 3D object for the preview
}

public class SpawnScript : MonoBehaviour
{
    public CubePrefabWithPreview[] cubePrefabsWithPreview; // Array to hold multiple cube prefabs with previews
    public float fallSpeed = 1.0f;
    public GameObject stoppingPlane; // Reference to the stopping plane
    private Grid1 grid;
    public GameObject nextBlockObject; // Reference to the Next_Block object

    private GameObject nextPrefab; // Reference to the next cube prefab
    private GameObject currentPreviewObject; // To store the current preview object

    void Start()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");

        if (boundaryCube != null)
        {
            grid = boundaryCube.GetComponent<Grid1>();

            if (grid != null)
            {
                PrePickNextBlock();
            }
            else
            {
                Debug.LogError("Grid component not found or inactive on Boundary_Cube.");
            }
        }
        else
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
        }
    }

    public void SpawnNewBlock()
    {
        if (cubePrefabsWithPreview == null || cubePrefabsWithPreview.Length == 0)
        {
            Debug.LogError("No cube prefabs assigned to the spawner.");
            return;
        }

        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube == null)
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
            return;
        }
        Grid1 grid = boundaryCube.GetComponent<Grid1>();
        if (grid == null)
        {
            Debug.LogError("Grid component not found on Boundary_Cube.");
            return;
        }

        // Calculate the spawn position at the top center of the grid
        // This assumes the grid's width and depth are centered around the Boundary_Cube's position
        Vector3 spawnPosition = new Vector3(
            boundaryCube.transform.position.x + (boundaryCube.transform.localScale.x / grid.width) * 0.5f, // Dynamically adjust based on Boundary_Cube's width and grid size
            boundaryCube.transform.position.y + (boundaryCube.GetComponent<BoxCollider>().size.y / 1.0f),
            boundaryCube.transform.position.z + (boundaryCube.transform.localScale.z / grid.depth) * 0.5f // Dynamically adjust based on Boundary_Cube's depth and grid size
        );

        if (nextPrefab != null)
        {
            Debug.Log("Next prefab name: " + nextPrefab.name); // Check the name of the next prefab
            if (nextPrefab.name == "CubeT Variant" || nextPrefab.name == "CubeL" || nextPrefab.name == "CubeT Variant 1" || nextPrefab.name == "CubeL 1")
            {
                spawnPosition.x += 0.15f; // Add an offset of 0.15 on the X axis
            }
        }
        else
        {
            Debug.LogWarning("Next prefab is null.");
        }

        // Instantiate the pre-picked cube prefab at the calculated position with the same rotation as the Boundary_Cube
        GameObject newBlock = Instantiate(nextPrefab, spawnPosition, boundaryCube.transform.rotation, boundaryCube.transform);

        // Pre-pick the next block for the next spawn
        PrePickNextBlock();
        UpdatePreviewObject();
    }

    private void PrePickNextBlock()
    {
        if (cubePrefabsWithPreview == null || cubePrefabsWithPreview.Length == 0)
        {
            Debug.LogError("No cube prefabs assigned to the spawner.");
            return;
        }

        // Randomly choose a cube prefab for the next spawn
        CubePrefabWithPreview chosenPrefabWithPreview = cubePrefabsWithPreview[Random.Range(0, cubePrefabsWithPreview.Length)];
        nextPrefab = chosenPrefabWithPreview.cubePrefab;

        // Update the 3D preview object
        UpdatePreviewObject();
    }

    private void UpdatePreviewObject()
    {
        if (nextBlockObject == null) return;

        // Store the current preview object in a temporary variable to destroy it after instantiating the new one
        GameObject tempPreviewObject = currentPreviewObject;

        if (nextPrefab != null)
        {
            // Instantiate the preview object as a child of the preview area
            CubePrefabWithPreview chosenPrefabWithPreview = System.Array.Find(cubePrefabsWithPreview, prefab => prefab.cubePrefab == nextPrefab);
            if (chosenPrefabWithPreview.previewObject != null)
            {
                currentPreviewObject = Instantiate(chosenPrefabWithPreview.previewObject, nextBlockObject.transform);

                // Get the bounds of the nextBlockObject to fit the preview object within
                Renderer nextBlockRenderer = nextBlockObject.GetComponent<Renderer>();
                if (nextBlockRenderer != null)
                {
                    Bounds nextBlockBounds = nextBlockRenderer.bounds;
                    Vector3 previewSize = chosenPrefabWithPreview.previewObject.GetComponent<Renderer>().bounds.size;

                    // Scale the preview object to fit within the nextBlockObject's bounds
                    float scaleFactor = Mathf.Min(nextBlockBounds.size.x / previewSize.x,
                                                  nextBlockBounds.size.y / previewSize.y,
                                                  nextBlockBounds.size.z / previewSize.z);
                    currentPreviewObject.transform.localScale *= scaleFactor;
                }

                currentPreviewObject.transform.localPosition = Vector3.zero;
                currentPreviewObject.transform.localRotation = Quaternion.identity;

                Debug.Log("3D preview object updated for the next block: " + nextPrefab.name);

                // Destroy the temporary preview object after the new one is instantiated
                if (tempPreviewObject != null)
                {
                    Destroy(tempPreviewObject);
                }
            }
            else
            {
                Debug.LogWarning("Preview object not set for the next block.");
                // Destroy the temporary preview object if the new preview is not set
                if (tempPreviewObject != null)
                {
                    Destroy(tempPreviewObject);
                }
            }
        }
        else
        {
            Debug.LogWarning("Next prefab is null.");
            // Destroy the temporary preview object if the new prefab is null
            if (tempPreviewObject != null)
            {
                Destroy(tempPreviewObject);
            }
        }
    }
}


