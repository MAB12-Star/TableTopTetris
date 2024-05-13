using System.Collections.Generic;
using UnityEngine;

public class GridChecker : MonoBehaviour
{
    public Vector3 oneFourthOfCellSize;

    // Method to check specifically the grid position 1-1-1
    public void CheckForGameOver()
    {
        int x = 1, y = 1, z = 1; // Target cell position 1-1-1
        Vector3 cellCenter = CalculateCellCenter(x, y, z);
        Collider[] colliders = Physics.OverlapBox(cellCenter, oneFourthOfCellSize, Quaternion.identity);

        // Check if the cell at 1-1-1 is occupied by any collider tagged as 'cube_child' or 'child'
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("cube_child") || collider.gameObject.CompareTag("child"))
            {
                Debug.Log("Game Over: The grid position 1-1-1 is occupied.");
                break; // Once we find an occupation in 1-1-1, no need to check further
            }
        }
    }

    // Example calculation for cell center, adjust as necessary for your grid setup
    public Vector3 CalculateCellCenter(int x, int y, int z)
    {
        // Example: Assume each cell is a 1x1x1 unit cube
        return new Vector3(x, y, z) + new Vector3(0.5f, 0.5f, 0.5f); // Center of the cell
    }
}
