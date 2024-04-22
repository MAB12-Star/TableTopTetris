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

            Vector3 objectScale = boundaryCube.GetComponent<BoxCollider>().size;

            // Calculate cell size based on boundary cube size and grid size
            float cellSizeX = objectScale.x / gridSizeX;
            float cellSizeY = objectScale.y / gridSizeY;
            float cellSizeZ = objectScale.z / gridSizeZ;

            Vector3 startPosition = boundaryCube.transform.position - objectScale / 2; // Bottom left back corner

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
                    Vector3 end = start + new Vector3(0, objectScale.y, 0);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }

    public Vector3 GetGridUnitSize()
    {
        return gridUnitSize;
    }

    private void Start()
    {   
        
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Initialize grid size based on the initial scale of the boundary cube's BoxCollider
            UpdateGridSize(boundaryCube.GetComponent<BoxCollider>().size);
            InitializeGrid();
        }
    }

    private void Update()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            lastBoundaryCubeSize = boundaryCube.transform.localScale;
        }
        else
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
        }
    }

    private void UpdateGridSize(Vector3 colliderSize)
    {
        // Calculate the size of each grid unit to fit the scaled object
        gridUnitSize = new Vector3(colliderSize.x / width, colliderSize.y / height, colliderSize.z / depth);

        // No need to change width, height, and depth here
        Debug.Log($"Grid unit size updated: {gridUnitSize}");
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

        float colliderOffset = 0.05f; // Offset value for collider size

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
                    collider.isTrigger = true; // Set box collider as trigger

                    // Set the tag for the grid unit collider
                    gridUnit.tag = "GridUnitCollider";

                    // Set the layer to GridInteraction
                    //collider.gameObject.layer = LayerMask.NameToLayer("GridInteraction");

                    gridUnit.transform.position = position;
                    gridUnit.name = $"{x}_{y}_{z}";

                    // Create a new GridCell instance and assign the grid unit object to it
                    gridCells[x, y, z] = new GridCell(gridUnitSize) { GridUnitObject = gridUnit }; // Modified line

                    // Set boundaryCube as the parent of gridUnit

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
            float cellSizeY = objectScale.y / height;
            float cellSizeZ = objectScale.z / depth;
            return startPosition + new Vector3(x * cellSizeX + cellSizeX / 2f, y * cellSizeY + cellSizeY / 2f, z * cellSizeZ + cellSizeZ / 2f);
        }
        else
        {
            Debug.LogError("Boundary_Cube GameObject not found in the scene.");
            return Vector3.zero;
        }
    }



    public void ConfirmGridOccupation(string[] tagsToCheck)
    {
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


    public Material swapMaterial;
    public int score = 0;
    public void IsGridUnitOccupied()
    {
        Vector3 halfCellSize = new Vector3(gridUnitSize.x / 2, gridUnitSize.y / 2, gridUnitSize.z / 2);

        // Check for full rows along the X-axis
        CheckAndColorFullRowsAxis("X", halfCellSize);

        // Check for full rows along the Z-axis
        CheckAndColorFullRowsAxis("Z", halfCellSize);

        CheckAndClearFullLayers(halfCellSize);
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
    private void CheckAndColorFullRowsAxis(string axis, Vector3 halfCellSize)
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                bool isRowFull = true;

               
                List<GameObject> objectsToColor = new List<GameObject>(); // Store objects to change material

                int axisLength = axis == "X" ? width : depth;
                for (int i = 0; i < axisLength; i++)
                {
                    int x = axis == "X" ? i : z;
                    int zCoord = axis == "Z" ? i : z;
                    Vector3 cellCenter = CalculateCellCenter(x, y, zCoord);
                    Collider[] colliders = Physics.OverlapBox(cellCenter, halfCellSize, Quaternion.identity);

                    bool foundChild = false;
                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("child"))
                        {
                            foundChild = true;
                            objectsToColor.Add(collider.gameObject); // Add object to the list for changing material
                            break; // Assuming one relevant child object per cell
                        }
                    }

                    if (!foundChild)
                    {
                        isRowFull = false;
                        break;
                    }
                }

                if (isRowFull)
                {
                    

                    // Change materials of all objects in the full row
                    foreach (GameObject obj in objectsToColor)
                    {
                        Renderer renderer = obj.GetComponent<Renderer>();
                        if (renderer != null && swapMaterial != null)
                        {
                            renderer.material = swapMaterial;
                        }
                    }
                 
                    Debug.Log($"Full row found along {axis}-axis at Y={y} and {(axis == "X" ? "Z=" + z : "X=" + z)}. Materials have been swapped.");
                }
            }
        }
    }




    private void UpdateScoreDisplay(int newScore)
    {
        // Find the ScoreUpdate UI component
        TextMeshProUGUI scoreText = GameObject.Find("Next_Block/Canvas/Image/ScoreUpdate").GetComponent<TextMeshProUGUI>();
        if (scoreText != null)
        {
            // Update the score display
            scoreText.text = newScore.ToString();
        }
        else
        {
            Debug.LogError("ScoreUpdate TextMeshProUGUI component not found.");
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
                    Vector3 cellCenter = CalculateCellCenter(x, y, z);
                    Collider[] colliders = Physics.OverlapBox(cellCenter, halfCellSize, Quaternion.identity);
                    bool foundChild = false;

                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject.CompareTag("child"))
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

               

                
                // Process and delete parents and their children
                ProcessAndDeleteObjects(parentsToDelete, childObjectsToDelete);
                
                Debug.Log($"Full layer found at Y={y}. Score updated, parents and children processed.");
                // If you want to check and clear only the first full layer found, uncomment the next line
                // break;
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

                Bounds bounds = new Bounds(transform.position, Vector3.zero);
                foreach (Transform child in transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                }

                // Assuming you want to align the object based on its bottom-left-back corner
                Vector3 gridAlignedPosition = new Vector3(
                    Mathf.Round(bounds.min.x / gridSize.x) * gridSize.x,
                    Mathf.Round(bounds.min.y / gridSize.y) * gridSize.y,
                    Mathf.Round(bounds.min.z / gridSize.z) * gridSize.z
                );


                // Calculate the offset needed to align the bounds with the grid
                Vector3 offset = gridAlignedPosition - bounds.min;
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

}










