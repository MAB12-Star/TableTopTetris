using UnityEngine;

public class DynamicColliderUpdater : MonoBehaviour
{
    private Vector3 lastScale;

    void Start()
    {
        lastScale = transform.localScale;
        UpdateBoxColliderSize();
    }

    void Update()
    {
        if (transform.localScale != lastScale)
        {
            UpdateBoxColliderSize();
            lastScale = transform.localScale;
        }
    }

    private void UpdateBoxColliderSize()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.size = transform.localScale;
            Debug.Log("BoxCollider size updated to match Boundary_Cube scale.");
        }
        else
        {
            Debug.LogError("BoxCollider component not found.");
        }
    }
}
