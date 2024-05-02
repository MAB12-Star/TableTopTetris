using System.Collections.Generic;
using UnityEngine;

public class GridUnit : MonoBehaviour
{
    // Boolean to track if the grid unit is occupied
    [SerializeField] private bool isOccupied = false;

    private BoxCollider boxCollider;
    private List<GameObject> objectsInsideCollider = new List<GameObject>();

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
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cube_child") || (other.CompareTag("child")))
        {
            ToggleObjectsInsideCollider(true, other.gameObject);
            
            //IsOccupied = true;
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("cube_child") || (other.CompareTag("child")))
        {
            ToggleObjectsInsideCollider(false, other.gameObject);
            
            //IsOccupied = false;
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
        RefreshListStatus();
        //Below code commented as above code is now being used
        //to refresh status if the grid is occupied.
        /*Vector3 sizeToDetectObjects = Vector3.zero;
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
        IsOccupied = false;*/
    }

    public void ToggleObjectsInsideCollider(bool toggle, GameObject gameObject)
    {
        if (objectsInsideCollider.Contains(gameObject))
        {
            if (!toggle)
            {
                objectsInsideCollider.Remove(gameObject);
            }
        }
        else
        {
            if (toggle)
            {
                objectsInsideCollider.Add(gameObject);
            }
        }
        RefreshListStatus();
    }

    public void RefreshListStatus()
    {
        ClearDeletedObjectsFromList();
        UpdateOccupiedStatusFromList();
    }

    public void UpdateOccupiedStatusFromList()
    {
        IsOccupied = objectsInsideCollider.Count > 0;
    }

    private void ClearDeletedObjectsFromList()
    {
        for (int i = 0; i < objectsInsideCollider.Count;)
        {
            if (objectsInsideCollider[i] == null)
            {
                objectsInsideCollider.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
