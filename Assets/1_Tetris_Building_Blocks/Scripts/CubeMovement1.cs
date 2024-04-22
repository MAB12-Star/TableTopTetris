using UnityEngine;


using System.Collections;
using Unity.VisualScripting;

public class CubeMovement1 : MonoBehaviour
{
    public float moveSpeed = 0.5f; // Adjust the move speed for grid-based movement
    public float gridSize = 1f; // Size of each grid unit
    private bool canMove = true; // Flag to control movement
    private Vector3 lastJoystickInput = Vector3.zero; // Store the last joystick input
    private Quaternion initialRotation;
    private float boundaryCubeSizeX;
    private float boundaryCubeSizeY;
    private float boundaryCubeSizeZ;
    private Grid1 grid;
    private SpawnScript1 spawnScript;
    private Rigidbody rigidbody;
    private bool hasSpawnedTrigger = false; // Flag to track if a trigger has already been spawned
    private bool isNudgeMode = false;
    private Vector3 objectSize;



    void Start()
    {
        initialRotation = transform.rotation;

        rigidbody = GetComponent<Rigidbody>();
        spawnScript = GameObject.Find("Spawner1").GetComponent<SpawnScript1>();


        if (rigidbody != null && spawnScript != null)
        {
            Debug.Log("Initial Velocity: " + rigidbody.velocity);

            rigidbody.velocity = new Vector3(0, -3, 0); // Set initial velocity for falling
        }
        else
        {
            Debug.LogError("Rigidbody or SpawnScript not found. Please check your setup.");
        }
        // Find the Boundary_Cube GameObject
        GameObject boundaryCube = GameObject.Find("Boundary_Cube1");

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



    void Update()
    {



        if (canMove)
        {
            float horizontalInput = Input.GetAxis("XRI_Left_Primary2DAxis_Horizontal");
            float verticalInput = Input.GetAxis("XRI_Left_Primary2DAxis_Vertical");
            Vector3 joystickInput = new Vector3(Mathf.Round(horizontalInput), 0, Mathf.Round(verticalInput));

            if (joystickInput != lastJoystickInput)
            {
                GameObject boundaryCube = GameObject.Find("Boundary_Cube1");
                if (boundaryCube != null)
                {
                    Grid1 grid = boundaryCube.GetComponent<Grid1>();
                    if (grid != null)
                    {
                        BoxCollider collider = boundaryCube.GetComponent<BoxCollider>();
                        if (collider != null)
                        {
                            // Ensure we factor in the boundary cube's current position.
                            Vector3 boundaryCubePosition = boundaryCube.transform.position;
                            Vector3 boundaryCubeSize = collider.size;

                            float scaleFactorX = boundaryCubeSize.x / grid.width;
                            float scaleFactorZ = boundaryCubeSize.z / grid.depth;

                            // Adjust movement based on grid scaling and boundary cube position.
                            Vector3 scaledMovement = new Vector3(joystickInput.x * scaleFactorX, 0, joystickInput.z * scaleFactorZ) * moveSpeed;
                            Vector3 newPosition = transform.position + scaledMovement;

                            // Here, we adjust the clamping to take into account the boundary cube's current position.
                            float minX = boundaryCubePosition.x - boundaryCubeSize.x / 2 + objectSize.x / 2;
                            float maxX = boundaryCubePosition.x + boundaryCubeSize.x / 2 - objectSize.x / 2;
                            float minZ = boundaryCubePosition.z - boundaryCubeSize.z / 2 + objectSize.z / 2;
                            float maxZ = boundaryCubePosition.z + boundaryCubeSize.z / 2 - objectSize.z / 2;

                            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

                            transform.position = newPosition;
                            lastJoystickInput = joystickInput;
                        }
                        else
                        {
                            Debug.LogError("BoxCollider not found on Boundary_Cube1 GameObject.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Grid1 component not found on Boundary_Cube1 GameObject.");
                    }
                }
                else
                {
                    Debug.LogError("Boundary_Cube1 GameObject not found in the scene.");
                }
            }
        }
    }
}


  