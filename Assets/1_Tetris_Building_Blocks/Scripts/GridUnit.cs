using UnityEngine;

public class GridUnit : MonoBehaviour
{
    // Boolean to track if the grid unit is occupied
    [SerializeField] private bool isOccupied = false;

    private BoxCollider boxCollider;

    #region Properties

    private BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
            }
            return boxCollider;
        }
    }

    #endregion

    public bool IsOccupied
    {
        get { return isOccupied; }
        set
        {
            isOccupied = value;
            Debug.Log("Grid unit isOccupied set to: " + isOccupied);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cube_child") || (other.CompareTag("child")))
        {
            
            IsOccupied = true;
            Debug.Log("Grid unit entered by object with tag: " + other.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("cube_child") || (other.CompareTag("child")))
        {
            
            IsOccupied = false;
        }
    }

    // Perform a double check to confirm occupancy status
    public void DoubleCheckOccupancy()
    {
        // Check if any box collider overlaps with this grid unit
        Collider[] overlappingColliders = Physics.OverlapBox(transform.position, transform.localScale / 2f);
        foreach (Collider collider in overlappingColliders)
        {
            if (collider.CompareTag("child") || (collider.CompareTag("cube_child")))
            {
                // Mark the grid unit as occupied
                IsOccupied = true;
                return; // Exit loop after finding one overlapping collider
            }
        }
        // No overlapping collider found, mark the grid unit as vacant
        IsOccupied = false;
    }
    public void finalLayerCheckOccupancy()
    {
        Vector3 sizeToDetectObjects = Vector3.zero;
        if (BoxCollider)
        {
            sizeToDetectObjects = Vector3.Scale(BoxCollider.size, transform.lossyScale) / 2;
        }
        else
        {
            Debug.LogError($"Box collider not found in the GridUnit gameobject.\nPlease attach a box collider to {gameObject.name}");
            return;
        }
        // Check if any box collider overlaps with this grid unit
        Collider[] overlappingColliders = Physics.OverlapBox(transform.position, sizeToDetectObjects);
        
        foreach (Collider collider in overlappingColliders)
        {
            if (collider.CompareTag("child") || (collider.CompareTag("cube_child")))
            {
                // Mark the grid unit as occupied
                IsOccupied = true;
                return; // Exit loop after finding one overlapping collider
            }
        }
        // No overlapping collider found, mark the grid unit as vacant
        IsOccupied = false;
    }
}
