// CubeMovement.cs
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using System.Collections.Generic;

using UnityEngine.XR;




public class CubeMovement : MonoBehaviour
{
    public float fallSpeed = .1f;
    public float moveSpeed = 1; // Adjust the move speed for grid-based movement
    public float gridSize = 1f; // Size of each grid unit
    private bool canMove = true; // Flag to control movement
    private Vector3 lastJoystickInput = Vector3.zero; // Store the last joystick input
    private Quaternion initialRotation;
    private float boundaryCubeSizeX;
    private float boundaryCubeSizeY;
    private float boundaryCubeSizeZ;
    private Grid1 grid;
    private SpawnScript spawnScript;
    private Rigidbody rigidbody;
    private bool hasSpawnedTrigger = false; // Flag to track if a trigger has already been spawned
    private bool isNudgeMode = false;
    private AudioSource audioSource;
    private AudioManager1 audioManager;

    

    /// <summary>
    /// This list contains all colliders tagged cube_child in this trasform
    /// </summary>
    private List<BoxCollider> boxColliders = new List<BoxCollider>();

    public List<BoxCollider> BoxColliders
    {
        get
        {
            if (boxColliders == null)
            {
                boxColliders = new List<BoxCollider>();
            }
            if (boxColliders.Count < 1)
            {
                foreach (Transform child in transform)
                {
                    foreach (Transform grandChild in child)
                    {
                        BoxCollider bc = grandChild.GetComponent<BoxCollider>();
                        if (bc != null)
                        {
                            boxColliders.Add(bc);
                        }
                    }
                }
            }
            return boxColliders;
        }
    }


    void Start()
    {
        
        // Find and store the Grid1 instance
        grid = FindObjectOfType<Grid1>();
        //audioSource = GetComponent<AudioSource>();
        //AudioManager.instance.Play("BackgroundMusic");
        // Check if grid is found
        if (grid == null)
        {
            Debug.LogError("Grid1 instance not found!");
            return;
        }
      



        // Now you can access gridSizeX from grid
        int gridSizeX = grid.width; // Assuming 'width' is the number of cells along the X-axis
        int gridSizeY = grid.height; // Assuming 'height' is the number of cells along the Y-axis
        int gridSizeZ = grid.depth;


        initialRotation = transform.rotation;

        rigidbody = GetComponent<Rigidbody>();
        spawnScript = GameObject.Find("Spawner").GetComponent<SpawnScript>();

        if (rigidbody != null && spawnScript != null)
        {
            // Calculate the velocity based on the size of one grid unit
            float velocityMagnitude = fallSpeed; // Adjust as needed
            Vector3 gridUnitSize = new Vector3(1f, 1f, 1f); // Adjust as needed based on your grid size

            // Calculate the velocity vector based on the direction and magnitude
            Vector3 velocity = Vector3.down * velocityMagnitude * gridUnitSize.y;

            rigidbody.velocity = velocity; // Set the calculated velocity
        }
        else
        {
            Debug.LogError("Rigidbody or SpawnScript not found. Please check your setup.");
        }
        // Find the Boundary_Cube GameObject
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");

        if (boundaryCube != null)
{
    // Get the Grid component on Boundary_Cube
    grid = boundaryCube.GetComponent<Grid1>();

    if (grid != null)
    {
        boundaryCubeSizeX = grid.width;
        boundaryCubeSizeY = grid.height;
        boundaryCubeSizeZ = grid.depth;
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

    
    private Vector3 GetCurrentGridScale()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            // Assuming the boundary cube's scale represents the total physical size of the grid
            // and you want to maintain 10 units regardless of actual size,
            // calculate how large one "unit" of movement should be in each direction.
            Vector3 scale = boundaryCube.transform.localScale;
            return new Vector3(scale.x / 10, scale.y / 20, scale.z / 10); // Adjust Y if necessary
        }
        return Vector3.one; // Default to 1x1x1 if boundary cube not found
    }

  

    void EnsureObjectsWithinGrid()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube == null) return;

        Grid1 grid = boundaryCube.GetComponent<Grid1>();
        if (grid == null) return;

        // Assuming the boundary cube's transform represents the center point of the grid
        Vector3 gridCenter = boundaryCube.transform.position;
        Vector3 gridSize = new Vector3(grid.width, grid.height, grid.depth); // Assuming these are the grid dimensions in units
        Vector3 gridUnitSize = grid.GetGridUnitSize(); // Assuming this returns the physical size of a grid unit

        // Calculate the physical size of the grid
        Vector3 gridPhysicalSize = new Vector3(gridSize.x * gridUnitSize.x, gridSize.y * gridUnitSize.y, gridSize.z * gridUnitSize.z);
        Vector3 gridMin = gridCenter - (gridPhysicalSize / 2);
        Vector3 gridMax = gridCenter + (gridPhysicalSize / 2);

        // Adjust the parent object and its children based on combined bounds
        AdjustObjectAndChildrenToStayWithinGrid(transform, gridMin, gridMax, gridUnitSize);
    }

    void AdjustObjectAndChildrenToStayWithinGrid(Transform objectTransform, Vector3 gridMin, Vector3 gridMax, Vector3 gridUnitSize)
    {
        Bounds combinedBounds = CalculateCombinedBounds(objectTransform);
        Vector3 combinedCenter = combinedBounds.center;
        Vector3 combinedExtents = combinedBounds.extents;

        // Calculate the limits based on combined bounds to ensure the object stays within grid
        float minX = Mathf.Max(gridMin.x + combinedExtents.x, combinedCenter.x - combinedExtents.x);
        float maxX = Mathf.Min(gridMax.x - combinedExtents.x, combinedCenter.x + combinedExtents.x);
        float minY = Mathf.Max(gridMin.y + combinedExtents.y, combinedCenter.y - combinedExtents.y);
        float maxY = Mathf.Min(gridMax.y - combinedExtents.y, combinedCenter.y + combinedExtents.y);
        float minZ = Mathf.Max(gridMin.z + combinedExtents.z, combinedCenter.z - combinedExtents.z);
        float maxZ = Mathf.Min(gridMax.z - combinedExtents.z, combinedCenter.z + combinedExtents.z);

        // Clamp the combined center position within the calculated limits
        Vector3 clampedCenter = new Vector3(
            Mathf.Clamp(combinedCenter.x, minX, maxX),
            Mathf.Clamp(combinedCenter.y, minY, maxY),
            Mathf.Clamp(combinedCenter.z, minZ, maxZ));

        // Calculate the adjustment vector needed to move the object within bounds
        Vector3 adjustment = clampedCenter - combinedCenter;

        // Apply the adjustment to the parent object to move it and its children within the grid
        objectTransform.position += adjustment;
    }

    Bounds CalculateCombinedBounds(Transform transform)
    {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    


    void Update()
    {
        EnsureObjectsWithinGrid();

        if (canMove)
        {
            Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
#if UNITY_EDITOR
            thumbstickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
            float horizontalInput = thumbstickInput.x;
            float verticalInput = thumbstickInput.y;
            Vector3 joystickInput = new Vector3(Mathf.Round(horizontalInput), 0, Mathf.Round(verticalInput));

            if (joystickInput != lastJoystickInput && joystickInput != Vector3.zero)
            {
                // Access the grid unit size from Grid1 component
                GameObject boundaryCube = GameObject.Find("Boundary_Cube");
                Grid1 grid = boundaryCube.GetComponent<Grid1>();
                Vector3 gridUnitSize = grid.GetGridUnitSize();

                // Calculate movement distance based on grid unit size and joystick input
                Vector3 movement = new Vector3(joystickInput.x * gridUnitSize.x, 0, joystickInput.z * gridUnitSize.z) * moveSpeed;
                Vector3 newPosition = transform.position + movement;

                // Optionally, clamp newPosition within the boundary defined by the Boundary_Cube and grid dimensions
                // newPosition = ClampPositionWithinBoundary(newPosition, boundaryCube.transform.position, grid);

                ChangePosition(movement);
            }

            // Existing rotation and gravity code remains unchanged
            HandleRotationAndGravity();
            lastJoystickInput = joystickInput;
        }
    }

    void HandleRotationAndGravity()
    {
        Vector2 rightThumbstickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
#if UNITY_EDITOR
        rightThumbstickInput = new Vector2(Input.GetAxis("NewHorizontal"), Input.GetAxis("NewVertical"));
#endif
        float rightStickHorizontal = rightThumbstickInput.x;
        float rightStickVertical = rightThumbstickInput.y;

        RotateCubeByAxis(rightStickHorizontal, rightStickVertical);
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            rigidbody.useGravity = true;
            AudioManager1.Instance.PlaySfx("cube_drop");
           

        }
        
    }



    private bool canRotate = true;
    private Vector3 savedPosition;
    private bool rotationOccurred = false;
    public float rotationCooldown = 0.2f; // Adjust the cooldown time as needed
    private Vector3 lastRotatedSize; // New variable to store the rotated size
    private Vector3 objectSize;
    // CubeMovement.cs
    // ...

    private void RotateCubeByAxis(float horizontal, float vertical)
    {
        if (canRotate && (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)) // Ensure there's significant input
        {
            Vector3 rotationAxis = Vector3.zero;
            float angle = 90f; // Fixed rotation angle

            // Determine rotation based on joystick input
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                rotationAxis = horizontal > 0 ? Vector3.up : Vector3.down;
            }
            else if (Mathf.Abs(vertical) > 0)
            {   
                rotationAxis = vertical > 0 ? Vector3.right : Vector3.left;
            }

            if (rotationAxis != Vector3.zero)
            {
                // Perform the rotation
                Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
                //transform.rotation *= rotation;

                Rotate(rotation);
                // Align the object to the grid after rotation
                //AlignParentObjectToGrid();
                //CheckAndAdjustBoundaries();


                // Prevent immediate re-rotation
                canRotate = false;
                StartCoroutine(RotationCooldownRoutine());
            }
        }

    }

    
    public class ObjectSizeConstants : MonoBehaviour
    {
        // Define constants for object sizes
        public static Vector3 GetObjectSize(string objectName)
        {
            switch (objectName)
            {
                case "CubeT":
                case "CubeT(Clone)":
                case "CubeT Variant(Clone)":
                case "TT":
                    return new Vector3(2f, 3f, 1f);
                case "Cube Rectangle":
                case "Cube Rectangle(Clone)":
                case "TC":
                    return new Vector3(1f, 3f, 1f);
                case "CubeL":
                case "CubeL(Clone)":
                case "CL":
                    return new Vector3(2f, 3f, 1f);
                case "Cube":
                case "Cube(Clone)":
                case "Cube3 Variant1(Clone)":
                case "CC":
                    return new Vector3(1f, 1f, 1f);
                // Add more cases for other objects as needed
                default:
                    Debug.LogError("Unsupported object name for size: " + objectName);
                    return Vector3.zero;
            }

        }
    }


    IEnumerator RotationCooldownRoutine()
    {
        yield return new WaitForSeconds(rotationCooldown);
        canRotate = true;
    }


    Vector3 AlignParentObjectToGrid()
    {
        Vector3 offset = Vector3.zero;
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            Grid1 gridComponent = boundaryCube.GetComponent<Grid1>();
            if (gridComponent != null)
            {
                Vector3 gridSize = gridComponent.GetGridUnitSize();

                Bounds bounds = CalculateCombinedBounds(); // Use your existing method to calculate bounds

                // Adjust for grid alignment, using grid unit size
                Vector3 gridAlignedPosition = new Vector3(
                    Mathf.Round((bounds.min.x - boundaryCube.transform.position.x) / gridSize.x) * gridSize.x + boundaryCube.transform.position.x,
                    bounds.min.y, // Assuming y-axis alignment isn't needed; adjust if necessary
                    Mathf.Round((bounds.min.z - boundaryCube.transform.position.z) / gridSize.z) * gridSize.z + boundaryCube.transform.position.z
                );

                // Calculate and apply the offset needed to align the object within the grid
                offset = gridAlignedPosition - new Vector3(bounds.min.x, transform.position.y, bounds.min.z);
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
        return offset;
    }

    void CheckAndAdjustBoundaries()
    {
        GameObject boundaryCube = GameObject.Find("Boundary_Cube");
        if (boundaryCube != null)
        {
            Grid1 grid = boundaryCube.GetComponent<Grid1>(); // Assuming this component provides grid boundary info

            Bounds bounds = CalculateCombinedBounds(); // Using the provided bounds calculation method

            // Assuming grid dimensions reflect overall size and not just cell count
            float minX = boundaryCube.transform.position.x - (grid.width * grid.GetGridUnitSize().x) / 2;
            float maxX = boundaryCube.transform.position.x + (grid.width * grid.GetGridUnitSize().x) / 2;
            float minZ = boundaryCube.transform.position.z - (grid.depth * grid.GetGridUnitSize().z) / 2;
            float maxZ = boundaryCube.transform.position.z + (grid.depth * grid.GetGridUnitSize().z) / 2;

            // Adjust position within calculated boundaries
            Vector3 newPosition = transform.position;

            if (bounds.min.x < minX) newPosition.x += minX - bounds.min.x;
            if (bounds.max.x > maxX) newPosition.x -= bounds.max.x - maxX;
            if (bounds.min.z < minZ) newPosition.z += minZ - bounds.min.z;
            if (bounds.max.z > maxZ) newPosition.z -= bounds.max.z - maxZ;

            transform.position = newPosition;
        }
        else
        {
            Debug.LogError("Boundary_Cube not found in the scene.");
        }
    }

    Bounds CalculateCombinedBounds()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Transform child in transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                bounds.Encapsulate(childRenderer.bounds);
            }
        }
        return bounds;
    }

   
    private bool hasCollided = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided && (collision.gameObject.CompareTag("StoppingPlane") || (collision.gameObject.CompareTag("child") || (collision.gameObject.CompareTag("cube_child") || (collision.gameObject.CompareTag("Cube") && collision.gameObject.layer != LayerMask.NameToLayer("CubeLayer"))))))
        {
           // Debug.Log("Object stopped at position: " + transform.position);

            if (!hasSpawnedTrigger)
            {
                rigidbody.useGravity = true;
                float delayInSeconds = 2f;
                StartCoroutine(HandleCollisionWithDelay(delayInSeconds));
               
                AudioManager1.Instance.PlaySfx("cube_collide");
                
               
                

            }

            // Set the flag to true to indicate that collision has been handled
            hasCollided = true;
            



            // Debug.Log("After Collision - Position: " + transform.position);
            bool isAligned = CheckAlignment();

            foreach (Transform child in transform)
            {
                Vector3 childPosition = child.position;
                // Assuming grid size is 1 unit for simplicity
                float gridSize = 1.0f;

                // Check if the child's position is aligned with the grid
                bool alignedX = Mathf.Approximately(childPosition.x % gridSize, 0) || Mathf.Approximately(childPosition.x % gridSize, gridSize / 2); // Adjusted this line
                bool alignedZ = Mathf.Approximately(childPosition.z % gridSize, 0) || Mathf.Approximately(childPosition.z % gridSize, gridSize);

                // Debug.Log("Child position: " + childPosition);
                // Debug.Log("Child alignment: X-" + alignedX + ", Z-" + alignedZ);


                
                if (!alignedX || !alignedZ)
                {
                    isAligned = false;
                    break; // No need to check further if any child is misaligned

                }
            }
           


            if (!isAligned)
            {
                // Attempt automatic alignment
                AlignParentObjectToGrid();

                // Optionally, enable nudge mode for manual adjustments if automatic alignment isn't perfect
              
                // Prevent normal movement while adjusting
                Debug.Log("Misalignment detected. Attempting automatic alignment. Nudge mode enabled for manual adjustment if needed.");
            }
           
          
        }
    }

   

    private bool CheckAlignment()
    {
        float alignmentTolerance = 0.5f; // Adjust this value as needed
        Grid1 gridComponent = GameObject.Find("Boundary_Cube")?.GetComponent<Grid1>();

        foreach (Transform child in transform)
        {
            Vector3 childPosition = child.position;
            float gridSizeX = gridComponent.width; // Grid size along the X-axis
            float gridSizeZ = gridComponent.depth; ; // Grid size along the Z-axis

            // Check if the child's position is aligned with the grid within the tolerance level
            // Check if the child's position is aligned with the grid within the tolerance level
            bool alignedX = Mathf.Abs(childPosition.x % gridSizeX) < alignmentTolerance;
            bool alignedZ = Mathf.Abs(childPosition.z % gridSizeZ) < alignmentTolerance;


            // If the object is not aligned on either axis, return false
            if (!alignedX || !alignedZ)
            {
                return false; // Misalignment detected
            }
        }
        // If all children are aligned, return true
        return true;
    }



    private IEnumerator HandleCollisionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Collision Detected!");

        CubeMovement movementComponent = GetComponent<CubeMovement>();

        if (movementComponent != null)
        {
            Destroy(movementComponent);
        }

        spawnScript.SpawnNewBlock();
        rigidbody.velocity = Vector3.zero;

        canMove = false;

        if (Mathf.Approximately(rigidbody.velocity.magnitude, 0f))
        {
            rigidbody.useGravity = true;
        }
        Debug.Log("Checking grid unit occupancy...");
        grid.IsGridUnitOccupied();
        hasSpawnedTrigger = true;
        AlignParentObjectToGrid();

        // Optionally, if you have logic that determines whether the alignment was successful or if further correction is needed
        if (!CheckAlignment())
        {
            Debug.LogError("Object alignment after collision is incorrect. Additional correction may be needed.");
            // Implement further correction steps as needed
        }
        else
        {
            Debug.Log("Object aligned successfully after collision.");
        }

        // grid.ConfirmGridOccupation(new string[] { "Cube", "child", "cube_child" });

    }

    public void ChangePosition(Vector3 movement)
    {
        transform.position += movement;
        if(!CanMove())
        {
            transform.position -= movement;
        }
    }

    public void Rotate(Quaternion newRotation)
    {
        transform.rotation *= newRotation;

        Vector3 offset = AlignParentObjectToGrid();
        CheckAndAdjustBoundaries();

        if (!CanMove())
        {
            transform.position -= offset;
            transform.rotation *= Quaternion.Inverse(newRotation);
        }
        CheckAndAdjustBoundaries();
    }

    /// <summary>
    /// This method tells if the cube transform
    /// can move in a direction.
    /// Currently it does not takes any direction or rotation
    /// As before calling this method we are already applying
    /// a change(position/rotation) and reverts back this
    /// returns falls.
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        bool objectFoundInWay = false;
        
        foreach (BoxCollider boxCollider in BoxColliders)
        {
            Vector3 size = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale) / 2;
            Collider[] colliders = Physics.OverlapBox(boxCollider.transform.position, size);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag(TagConstants.CubeChild))
                {
                    if (!IsDescendantCollider(collider.transform))
                    {
                        objectFoundInWay = true;
                        break;
                    }
                }
            }
            if (objectFoundInWay)
            {
                break;
            }
        }
        return !objectFoundInWay;
    }

    public bool IsDescendantCollider(Transform trans)
    {
        foreach (BoxCollider boxCollider in BoxColliders)
        {
            if (boxCollider.transform == trans)
            {
                return true;
            }
        }
        return false;
    }
}









