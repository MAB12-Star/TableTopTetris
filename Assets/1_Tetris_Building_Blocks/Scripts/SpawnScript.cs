using UnityEngine;
using UnityEngine.UI;
using static CubeMovement;

[System.Serializable]
public struct CubePrefabWithPreview
{
    public GameObject cubePrefab;
    public Texture previewImage; // Texture for the preview image
}

public class SpawnScript : MonoBehaviour
{
    public CubePrefabWithPreview[] cubePrefabsWithPreview; // Array to hold multiple cube prefabs with previews
    public float fallSpeed = 1.0f;
    public GameObject stoppingPlane; // Reference to the stopping plane
    private Grid1 grid;
    public GameObject nextBlockObject; // Reference to the Next_Block object
    public RawImage nextBlockRawImage; // Reference to the RawImage component for preview in Next_Block
  
    private GameObject nextPrefab; // Reference to the next cube prefab

    
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
            boundaryCube.transform.position.y + (boundaryCube.GetComponent<BoxCollider>().size.y / 2.0f),
            boundaryCube.transform.position.z + (boundaryCube.transform.localScale.z / grid.depth) * 0.5f // Dynamically adjust based on Boundary_Cube's depth and grid size
        );
        if (nextPrefab != null)
        {
            Debug.Log("Next prefab name: " + nextPrefab.name); // Check the name of the next prefab
            if (nextPrefab.name == "CubeT Variant" || nextPrefab.name == "CubeL"|| nextPrefab.name == "CubeT Variant 1" || nextPrefab.name == "CubeL 1")
            {
                spawnPosition.x += 0.15f; // Add an offset of 0.15 on the X axis
            }
        
    }
        else
        {
            Debug.LogWarning("Next prefab is null.");
        }

        // Instantiate the pre-picked cube prefab at the calculated position
        // Parent it under the Boundary_Cube to maintain a clean hierarchy and ensure it moves with the Boundary_Cube if needed
        Instantiate(nextPrefab, spawnPosition, Quaternion.identity, boundaryCube.transform);

        // Pre-pick the next block for the next spawn
        PrePickNextBlock();
        UpdatePreviewImage();
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

        // Update the preview image
        if (nextBlockRawImage != null && chosenPrefabWithPreview.previewImage != null)
        {
            nextBlockRawImage.texture = chosenPrefabWithPreview.previewImage;
            Debug.Log("Next block prefab pre-picked: " + nextPrefab.name);
        }
        else
        {
            Debug.LogWarning("RawImage component or preview image not set.");
        }
    }
    private void UpdatePreviewImage()
    {
        if (nextBlockRawImage != null && nextPrefab != null)
        {
            // Find the CubePrefabWithPreview struct that matches the selected nextPrefab
            foreach (var cubePrefabWithPreview in cubePrefabsWithPreview)
            {
                if (cubePrefabWithPreview.cubePrefab == nextPrefab)
                {
                    // Update the RawImage component with the corresponding preview image
                    nextBlockRawImage.texture = cubePrefabWithPreview.previewImage;
                    Debug.Log("Preview image updated for the next block: " + cubePrefabWithPreview.cubePrefab.name);
                    return;
                }
            }
            Debug.LogWarning("No preview image found for the next block.");
        }
        else
        {
            Debug.LogWarning("RawImage component or next prefab not set.");
        }
    }

}
