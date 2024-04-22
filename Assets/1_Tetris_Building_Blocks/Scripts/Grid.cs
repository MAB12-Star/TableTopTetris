using UnityEngine;

public class Grid : MonoBehaviour
{
    public int width;
    public int height;
    public int depth;
    public Transform[,,] gridArray;
    public GameObject prefab;
    private Vector3 lastBoundaryCubeSize;

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
            float cellSizeX = objectScale.x / 10;
            float cellSizeY = objectScale.y / 10;
            float cellSizeZ = objectScale.z / 10;

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
            // Initialize grid size based on the initial scale of the boundary cube
            UpdateGridSize(boundaryCube.transform.localScale);
            InitializeGrid();
            // Additional initialization code...
        }


    }

    private void Update()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            Vector3 currentScale = boundaryCube.transform.localScale;

            UpdateGridSize(currentScale);
            InitializeGrid(); // Optionally reinitialize or update the grid based on the new size
            lastBoundaryCubeSize = currentScale;

        }
    }

    // Helper method to determine if two Vector3 values are approximately equal


    private void UpdateGridSize(Vector3 objectScale)
    {
        // Calculate the size of each grid unit to fit the scaled object
        gridUnitSize = new Vector3(objectScale.x / width, objectScale.y / height, objectScale.z / depth);

        // No need to change width, height, and depth here
        Debug.Log($"Grid unit size updated: {gridUnitSize}");
    }




    private void InitializeGrid()
    {
        gridArray = new Transform[width, height, depth];
        Debug.Log("Grid Dimensions: " + width + " x " + height + " x " + depth);
    }


    private Vector3 gridUnitSize; // Size of each grid unit in world space




}