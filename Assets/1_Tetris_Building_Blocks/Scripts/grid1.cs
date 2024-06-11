using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Grid1 : MonoBehaviour
{
    public int width;
    public int height;
    public int depth;
    public Transform[,,] gridArray;
    public GameObject prefab;
    private Vector3 lastBoundaryCubeSize;
    private Vector3 gridUnitSize; // Size of each grid unit in world space

    GridUnit GridUnit;
    float originalTimeScale = 1.0f;
    public Material swapMaterial;
    public int score = 0;
    public int level = 1;
    public int currentLevel = 1;
    private int scoreThreshold = 10;
    private EncouragingWords encouragingWords;
    
    


    /// <summary>
    /// This is used to check for any object is inside a grid unit or not.
    /// This divides the size of the grid unit to 1/4 so that
    /// when detecting objects only those are consider which are inside it
    /// and not on the borders of it.
    /// </summary>
    private const int gridSizeDividerForColorCheck = 4;

    [SerializeField] private TextMeshProUGUI scoreTextMeshPro;
    [SerializeField] private TextMeshProUGUI levelTextMeshPro;
    [SerializeField] private TextMeshProUGUI youWinTextMeshPro;

    void OnDrawGizmos()
{
    GameObject boundaryCube = GameObject.Find("Boundary_Cube");
    if (boundaryCube != null)
    {
        Gizmos.color = Color.yellow;

        // Assuming gridSize represents the number of cells along each dimension
        int gridSizeX = width;  // Assuming 'width' is the number of cells along the X-axis
        int gridSizeY = height; // Assuming 'height' is the number of cells along the Y-axis
        int gridSizeZ = depth;  // Assuming 'depth' is the number of cells along the Z-axis

        Vector3 objectScale = boundaryCube.transform.localScale;

        // Calculate cell size based on boundary cube size and grid size
        float cellSizeX = objectScale.x / gridSizeX;
        float cellSizeY = (objectScale.y * 2) / gridSizeY; // Adjusted for the Y-axis
        float cellSizeZ = objectScale.z / gridSizeZ;

        Vector3 startPosition = boundaryCube.transform.position - new Vector3(objectScale.x / 2, objectScale.y, objectScale.z / 2); // Bottom left back corner adjusted for Y

        // Draw grid lines along X-axis
        for (int x = 0; x <= gridSizeX; x++)
        {
            for (int y = 0; y <= gridSizeY; y++)
            {
                Vector3 start = startPosition + new Vector3(x * cellSizeX, y * cellSizeY, 0);
                Vector3 end = start + new Vector3(0, 0, objectScale.z);
                Gizmos.DrawLine(start, end);
            }
        }

        // Draw grid lines along Y-axis
        for (int y = 0; y <= gridSizeY; y++)
        {
            for (int z = 0; z <= gridSizeZ; z++)
            {
                Vector3 start = startPosition + new Vector3(0, y * cellSizeY, z * cellSizeZ);
                Vector3 end = start + new Vector3(objectScale.x, 0, 0);
                Gizmos.DrawLine(start, end);
            }
        }

        // Draw grid lines along Z-axis
        for (int z = 0; z <= gridSizeZ; z++)
        {
            for (int x = 0; x <= gridSizeX; x++)
            {
                Vector3 start = startPosition + new Vector3(x * cellSizeX, 0, z * cellSizeZ);
                Vector3 end = start + new Vector3(0, objectScale.y * 2, 0); // Adjusted for the Y-axis
                Gizmos.DrawLine(start, end);
            }
        }
    }
}


    public Vector3 GetGridUnitSize()
    {
        // Assuming the boundary cube's transform represents the bounds of the grid
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Get the Grid component on Boundary_Cube
            Grid1 grid = boundaryCube.GetComponent<Grid1>();
            if (grid != null)
            {
                // Calculate the size of one unit in each axis based on the grid dimensions
                Vector3 gridSize = new Vector3(grid.width, grid.height, grid.depth);
                Vector3 scale = boundaryCube.transform.localScale;
                return new Vector3(scale.x / gridSize.x, (scale.y*2) / gridSize.y, scale.z / gridSize.z);
            }
            else
            {
                Debug.LogError("Grid component not found on Boundary_Cube.");
            }
        }
        else
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
        }

        // Return a default unit size if grid information is not available
        return Vector3.one;
    }







    private void Start()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Initialize grid size based on the boundary cube's transform scale
            UpdateGridSize(boundaryCube.transform.localScale);
            Quaternion rotation = GameObject.Find("Boundary_Cube").transform.rotation;

            // Update the grid rotation
           
            InitializeGrid();
            // Initialize the level display to show level 1
            UpdateLevelDisplay(currentLevel);
        }

        encouragingWords = FindObjectOfType<EncouragingWords>();
        if (encouragingWords == null)
        {
            Debug.LogError("EncouragingWords script not found!");
        }
    }


    private void Update()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            lastBoundaryCubeSize = boundaryCube.transform.localScale;
            UpdateGridRotation(boundaryCube.transform.rotation);
        }
        else
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
        }

    }

    private void UpdateGridSize(Vector3 transformScale)
    {
        // Calculate the size of each grid unit to fit the scaled object, with height doubled
        gridUnitSize = new Vector3(transformScale.x / width, (transformScale.y * 2) / height, transformScale.z / depth);

        // No need to change width, height, and depth here
        Debug.Log($"Grid unit size updated: {gridUnitSize}");
    }

    public void UpdateGridRotation(Quaternion rotation)
    {
        // Update the grid rotation based on the provided rotation
        transform.rotation = rotation;

        // Update other aspects of the grid if needed...
    }

    public class GridCell
    {
        public GameObject GridUnitObject { get; set; }
        public bool IsOccupied { get; set; }
        public Vector3 Size { get; set; } // Add Size property

        // Constructor to initialize Size property
        public GridCell(Vector3 size)
        {
            Size = size;    
        }
    }


    GridCell[,,] gridCells; // Table array to hold grid cells

    private void InitializeGrid()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube == null)
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
            return;
        }

        float colliderOffset = 0.02f; // Offset value for collider size
        float colliderCenterYOffset = 0.0f; // Offset for collider center on the Y-axis

        gridCells = new GridCell[width, height, depth]; // Initialize the table array
        Debug.Log("Grid Dimensions: " + width + " x " + height + " x " + depth);

        // Create grid units with a single box collider set as trigger
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 position = CalculateCellCenter(x, y, z);
                    GameObject gridUnit = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(gridUnit.GetComponent<MeshRenderer>()); // Remove mesh renderer
                    BoxCollider existingCollider = gridUnit.GetComponent<BoxCollider>();
                    if (existingCollider != null)
                    {
                        Destroy(existingCollider); // Remove existing collider if present
                    }
                    BoxCollider collider = gridUnit.AddComponent<BoxCollider>();

                    // Set collider size slightly smaller than the grid unit size
                    collider.size = new Vector3(gridUnitSize.x - colliderOffset, gridUnitSize.y - colliderOffset, gridUnitSize.z - colliderOffset);
                    collider.center = new Vector3(0, colliderCenterYOffset, 0); // Adjust collider center

                    collider.isTrigger = true; // Set box collider as trigger

                    // Set the tag for the grid unit collider
                    gridUnit.tag = "GridUnitCollider";

                    gridUnit.transform.position = position;
                    gridUnit.name = $"{x}_{y}_{z}";

                    // Create a new GridCell instance and assign the grid unit object to it
                    gridCells[x, y, z] = new GridCell(gridUnitSize) { GridUnitObject = gridUnit }; // Modified line

                    // Set boundaryCube as the parent of gridUnit
                    gridUnit.transform.parent = boundaryCube.transform;

                    // Add IsOccupied boolean and set it to false by default
                    gridUnit.AddComponent<GridUnit>().IsOccupied = false;
                }
            }
        }
        // Debug information to verify grid initialization
        /*  Debug.Log("Grid initialized. Contents of gridCells array:");
          for (int x = 0; x < width; x++)
          {
              for (int y = 0; y < height; y++)
              {
                  for (int z = 0; z < depth; z++)
                  {
                      Debug.Log($"GridCell at ({x}, {y}, {z}): {gridCells[x, y, z].GridUnitObject.name}");
                  }
              }
          }*/
    }


    public Vector3 CalculateCellCenter(int x, int y, int z)
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            Vector3 objectScale = boundaryCube.GetComponent<BoxCollider>().size;
            Vector3 startPosition = boundaryCube.transform.position - objectScale / 2; // Bottom left back corner
            float cellSizeX = objectScale.x / width;
            float cellSizeY = (objectScale.y) /height; 
            float cellSizeZ = objectScale.z / depth;
            return startPosition + new Vector3(x * cellSizeX + cellSizeX / 2f, y * cellSizeY + cellSizeY / 2f, z * cellSizeZ + cellSizeZ / 2f);
        }
        else
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
            return Vector3.zero;
        }
    }




    public void ConfirmGridOccupation()


    {
        string[] tagsToCheck = new string[] { "cube_child" };

        foreach (GridCell cell in gridCells)
        {
            GameObject gridUnitObject = cell.GridUnitObject;
            if (gridUnitObject != null)
            {
                Collider[] overlappingColliders = Physics.OverlapBox(gridUnitObject.transform.position, cell.Size / 2);
                foreach (Collider collider in overlappingColliders)
                {
                    if (Array.Exists(tagsToCheck, tag => collider.CompareTag(tag)))
                    {
                        GridUnit gridUnit = collider.GetComponent<GridUnit>();
                        if (gridUnit != null && !gridUnit.IsOccupied)
                        {
                            gridUnit.IsOccupied = true;

                        }
                    }
                }
            }
        }
    }


    public void IsGridUnitOccupied()
    {
        // Get the current grid unit size
        Vector3 gridUnitSize = GetGridUnitSize();

        // Calculate the size to detect objects within a cell
        Vector3 cellSizeToDetect = new Vector3(gridUnitSize.x / gridSizeDividerForColorCheck, gridUnitSize.y / gridSizeDividerForColorCheck, gridUnitSize.z / gridSizeDividerForColorCheck);

        // Check for full rows along the X-axis
        CheckAndColorFullRowsAxis("X", cellSizeToDetect);

        // Check for full rows along the Z-axis
        CheckAndColorFullRowsAxis("Z", cellSizeToDetect);

 

        // Check and clear full layers
        CheckAndClearFullLayers(cellSizeToDetect);
    }


    public void UpdateGridOccupancyStatus()
    {
        foreach (var cell in gridCells)
        {
            GridUnit gridUnitComponent = cell.GridUnitObject.GetComponent<GridUnit>();
            if (gridUnitComponent != null)
            {
                gridUnitComponent.finalLayerCheckOccupancy();
            }
        }
    }
    private void CheckAndColorFullRowsAxis(string axis, Vector3 oneFourthOfCellSize)
    {
        int axisLength = axis == "X" ? width : depth;

        // Iterate over each row along the specified axis
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                bool isRowFull = true;
                List<GameObject> objectsToColor = new List<GameObject>(); // Store parent objects to change material

                // Iterate over each cell in the current row along the specified axis
                for (int i = 0; i < axisLength; i++)
                {
                    int x = axis == "X" ? i : z;
                    int zCoord = axis == "Z" ? i : z;

                    // Access the grid cell directly
                    GridCell cell = gridCells[x, y, zCoord];
                    if (cell == null || !cell.GridUnitObject.GetComponent<GridUnit>().IsOccupied)
                    {
                        isRowFull = false;
                        break; // No need to continue checking this row
                    }

                    GameObject gridUnitObject = cell.GridUnitObject;
                    Collider[] colliders = Physics.OverlapBox(gridUnitObject.transform.position, oneFourthOfCellSize, Quaternion.identity);

                    bool foundChildOrCubeChild = false;

                    // Check if any cell in the row is empty
                    if (colliders.Length == 0)
                    {
                        isRowFull = false;
                        break; // No need to continue checking this row
                    }

                    // Check each collider in the cell
                    foreach (Collider collider in colliders)
                    {
                        // Check for 'cube_child' within the cell and add its parent if it has the 'child' tag
                        if (collider.gameObject.CompareTag("cube_child"))
                        {
                            Transform parent = collider.transform.parent;
                            if (parent != null && parent.CompareTag("child"))
                            {
                                foundChildOrCubeChild = true;
                                if (!objectsToColor.Contains(parent.gameObject))
                                {
                                    objectsToColor.Add(parent.gameObject); // Add parent object to the list for changing material
                                }
                            }
                        }
                        // Directly check for 'child' tagged objects
                        else if (collider.gameObject.CompareTag("child"))
                        {
                            foundChildOrCubeChild = true;
                            if (!objectsToColor.Contains(collider.gameObject))
                            {
                                objectsToColor.Add(collider.gameObject); // Directly add to list for changing material
                            }
                        }
                    }

                    // If no child objects were found in the current cell, the row is not full
                    if (!foundChildOrCubeChild)
                    {
                        isRowFull = false;
                        break; // No need to continue checking this row
                    }
                }

                // If the row is full, change the material of all objects in the row
                if (isRowFull)
                {
                    foreach (GameObject obj in objectsToColor)
                    {
                        Renderer renderer = obj.GetComponent<Renderer>();
                        if (renderer != null && swapMaterial != null)
                        {
                            renderer.material = swapMaterial;
                        }
                    }

                    Debug.Log($"Full row found along {axis}-axis at Y={y}. Materials have been swapped.");
                }
                else
                {
                    Debug.Log($"Row is not full at Y={y} along {axis}-axis.");
                }
            }
        }
    }






    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreTextMeshPro != null)
        {
            // Update the score display
            scoreTextMeshPro.text = newScore.ToString();

            // Check if the new score meets the threshold for increasing the level
            if (newScore >= currentLevel * scoreThreshold)
            {
                // Increase the level
                currentLevel++;
                AudioManager1.Instance.PlayMusic("Theme2");

                // Update the level display
                UpdateLevelDisplay(currentLevel);

                if (currentLevel == 5)
                {
                    youWinTextMeshPro.text = "You Win";
                    Time.timeScale = 0f;
                }
                else
                {
                    StartCoroutine(DisplayEncouragingWordForDuration(3f));
                }
            }
        }
        else
        {
            Debug.LogError("No reference found for scoreTextMeshPro. Failed to update score!");
        }
    }

    private IEnumerator DisplayEncouragingWordForDuration(float duration)
    {
        // Display encouraging word
        encouragingWords.DisplayEncouragingWord();

        // Wait for specified duration
        yield return new WaitForSeconds(duration);

        // Clear the encouraging word
        encouragingWords.ClearText();
    }

    public void UpdateLevelDisplay(int newLevel)
    {
        if (levelTextMeshPro != null)
        {
            // Update the level display
            levelTextMeshPro.text = newLevel.ToString();
        }
        else
        {
            Debug.LogError("No reference found for levelTextMeshPro. Failed to update level!");
        }
    }

    private void CheckAndClearFullLayers(Vector3 halfCellSize)
    {
        for (int y = 0; y < height; y++)
        {
            bool isLayerFull = true;
            HashSet<GameObject> parentsToDelete = new HashSet<GameObject>();
            List<GameObject> childObjectsToDelete = new List<GameObject>();

            // Check for full layer by checking every cell in the layer
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GridCell cell = gridCells[x, y, z];
                    if (cell == null || !cell.GridUnitObject.GetComponent<GridUnit>().IsOccupied)
                    {
                        isLayerFull = false; // This cell is not occupied, layer is not full
                        break; // No need to check further cells in this layer
                    }

                    GameObject gridUnitObject = cell.GridUnitObject;
                    Collider[] colliders = Physics.OverlapBox(gridUnitObject.transform.position, halfCellSize, Quaternion.identity);
                    bool foundChild = false;

                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("cube_child") || (collider.gameObject.CompareTag("child")))
                        {
                            foundChild = true;
                            GameObject parentObject = collider.transform.parent ? collider.transform.parent.gameObject : null;
                            if (parentObject != null)
                            {
                                parentsToDelete.Add(parentObject);
                            }
                            childObjectsToDelete.Add(collider.gameObject);
                            break; // Assuming one relevant child object per cell
                        }
                    }

                    if (!foundChild)
                    {
                        isLayerFull = false; // This cell is not occupied by a 'child', layer is not full
                        break; // No need to check further cells in this layer
                    }
                }
                if (!isLayerFull) break; // No need to check further layers if this one is not full
            }

            // If the entire layer is full
            if (isLayerFull)
            {
                score += 10; // Increase score for a full layer
                UpdateScoreDisplay(score);
                if (score % 10 == 0)
                {
                    // Calculate the number of increments
                    int increments = score / 10;

                    // Increase time scale by 0.5 * increments
                    Time.timeScale = 1.0f + (0.3f * increments);
                }

                // Process and delete parents and their children
                ProcessAndDeleteObjects(parentsToDelete, childObjectsToDelete);

                Debug.Log($"Full layer found at Y={y}. Score updated, parents and children processed.");
                // If you want to check and clear only the first full layer found, uncomment the next line
                // break;
            }
            else
            {
                Debug.Log($"Layer is not full at Y={y}.");
            }
        }
    }



    private void ProcessAndDeleteObjects(HashSet<GameObject> parentsToDelete, List<GameObject> childObjectsToDelete)
    {

        foreach (GameObject parent in parentsToDelete)
        {
            foreach (Transform child in parent.transform)
            {
                if (!childObjectsToDelete.Contains(child.gameObject)) // If child is not being deleted
                {
                    child.SetParent(null); // Unparent the child
                    AddOrUpdateRigidbody(child.gameObject);
                   
                }

            }
            Destroy(parent); // Delete the parent
        }

        // Delete the child objects
        foreach (GameObject childObject in childObjectsToDelete)
        {
            Destroy(childObject);
        }
        UpdateGridOccupancyStatus();
        EnsureAllRigidbodiesHaveGravity();
    }

    private void EnsureAllRigidbodiesHaveGravity()
    {
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (!rb.useGravity)
            {
                rb.useGravity = true; // Enforce gravity
            }
        }
    }
    private void AddOrUpdateRigidbody(GameObject child)
    {   
        MeshCollider childMeshCollider = child.GetComponent<MeshCollider>();
        if (childMeshCollider == null)
        {
            childMeshCollider = child.AddComponent<MeshCollider>();

        }

        childMeshCollider.convex = true;

        Rigidbody childRigidbody = child.GetComponent<Rigidbody>();
        if (childRigidbody == null) // Add Rigidbody if it doesn't exist
        {
            childRigidbody = child.AddComponent<Rigidbody>();
            
        }

        // Set Rigidbody constraints
        childRigidbody.useGravity = true;
        childRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                                     RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        AlignParentObjectToGrid();
        
    }

    void AlignParentObjectToGrid()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            Grid1 gridComponent = boundaryCube.GetComponent<Grid1>();
            if (gridComponent != null)
            {
                // Retrieve the grid unit size from the Grid component
                Vector3 gridSize = gridComponent.GetGridUnitSize();
                Debug.Log($"Grid Size: {gridSize}");

                Bounds bounds = new Bounds(transform.position, Vector3.zero);
                foreach (Transform child in transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                }

                Debug.Log($"Original Bounds Min: {bounds.min}");

                // Align based on the bottom-left-back corner
                Vector3 gridAlignedPosition = new Vector3(
                    Mathf.Floor(bounds.min.x / gridSize.x) * gridSize.x,
                    Mathf.Floor(bounds.min.y / gridSize.y) * gridSize.y,
                    Mathf.Floor(bounds.min.z / gridSize.z) * gridSize.z
                );

                Debug.Log($"Calculated Grid-Aligned Position: {gridAlignedPosition}");

                // Calculate the offset needed to align the bounds with the grid
                Vector3 offset = gridAlignedPosition - bounds.min;
                Debug.Log($"Offset to Apply: {offset}");

                // Apply the offset to the parent object to align it with the grid
                transform.position += offset;
            }
            else
            {
                Debug.LogError("Grid component not found on Boundary_Cube.");
            }
        }
        else
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
        }
    }

    public void AdjustAndReinitializeGrid()
    {

        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Initialize grid size based on the initial scale of the boundary cube's BoxCollider
           
            InitializeGrid();
        }


        // Reinitialize the grid to reflect new dimensions

    }
    // Add to your existing class

    public void ResizeGridAndBoundaryCube(float newWidth, float newHeight, float newDepth)
    {
        // Assuming newWidth, newHeight, and newDepth are the new sizes for the boundary cube
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Update boundary cube scale - adjust this line based on how your game scales objects
            boundaryCube.transform.localScale = new Vector3(newWidth, newHeight, newDepth);

            // Update the grid dimensions if they are directly tied to the size of the boundary cube
            UpdateGridSize(boundaryCube.GetComponent<BoxCollider>().size);

            // Clear existing grid cells if necessary
            

            // Reinitialize the grid with new dimensions
            InitializeGrid();
        }
        else
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
        }
    }

    private void ClearExistingGridCells()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Get a list of all child objects but not the boundary cube itself
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in boundaryCube.transform)
            {
                children.Add(child.gameObject);
            }

            // Destroy all child objects, which should be the grid cells
            children.ForEach(child => Destroy(child));
        }
    }


}










