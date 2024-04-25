using UnityEngine;



public class MoveDown : MonoBehaviour
{
    public float moveSpeed = 0.5f; // Adjust the move speed for grid-based movement
    private Rigidbody _rigidbody;

    // Reference to the Grid1 component on the Boundary_Cube
    private Grid1 grid;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (!_rigidbody)
        {
            Debug.LogError("Rigidbody not found on the object.");
            return;
        }

        // Find and store a reference to the Grid1 component from the Boundary_Cube
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            grid = boundaryCube.GetComponent<Grid1>();
            if (grid == null)
            {
                Debug.LogError("Grid1 component not found on Boundary_Cube.");
            }
        }
        else
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
        }
    }

    void FixedUpdate()
    {
        if (_rigidbody && grid != null)
        {
            MoveDowntown();
            AlignWithinGrid();
        }
    }

    public void MoveDowntown()
    {
        // Apply a continuous force downwards
        _rigidbody.MovePosition(transform.position + Vector3.down * moveSpeed * Time.fixedDeltaTime);
    }

    private void AlignWithinGrid()
    {
        // Assuming the grid and boundary definitions are similar to those in your "EnsureObjectsWithinGrid" method
        Vector3 gridCenter = grid.transform.position;
        Vector3 gridSize = new Vector3(grid.width, grid.height, grid.depth);
        Vector3 gridUnitSize = grid.GetGridUnitSize(); // You might need to adjust this call based on your actual Grid1 implementation

        // Calculate the physical size of the grid
        Vector3 gridPhysicalSize = new Vector3(gridSize.x * gridUnitSize.x, gridSize.y * gridUnitSize.y, gridSize.z * gridUnitSize.z);
        Vector3 gridMin = gridCenter - (gridPhysicalSize / 2);
        Vector3 gridMax = gridCenter + (gridPhysicalSize / 2);

        // Check and adjust the position of the object to ensure it's within the grid boundaries
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, gridMin.x, gridMax.x),
            Mathf.Clamp(transform.position.y, gridMin.y, gridMax.y),
            Mathf.Clamp(transform.position.z, gridMin.z, gridMax.z)
        );
    }
}

