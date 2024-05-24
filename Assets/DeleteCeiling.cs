using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCeiling : MonoBehaviour
{
    private OVRSceneRoom ovrSceneRoom;
    private bool ceilingDestroyed = false;
    public string roomObjectName = "Room"; // The name of the room object (without the number)

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to find the Room object
        GameObject roomObject = GameObject.Find(roomObjectName);

        if (roomObject != null)
        {
            Debug.Log("Room object found: " + roomObject.name);

            // Find the OVRSceneRoom component within the Room object
            ovrSceneRoom = roomObject.GetComponent<OVRSceneRoom>();

            if (ovrSceneRoom != null)
            {
                Debug.Log("OVRSceneRoom component found in Room object.");
            }
            else
            {
                Debug.Log("OVRSceneRoom component not found in Room object.");
            }
        }
        else
        {
            Debug.Log("Room object not found.");
        }

        // Attempt to destroy the ceiling if it exists
        TryDestroyCeiling();
    }

    // Update is called once per frame
    void Update()
    {
        // Attempt to destroy the ceiling if it hasn't been destroyed yet
        if (!ceilingDestroyed)
        {
            TryDestroyCeiling();
        }
    }

    private void TryDestroyCeiling()
    {
        if (ovrSceneRoom != null)
        {
            if (ovrSceneRoom.Ceiling != null)
            {
                // Destroy the Ceiling game object
                Destroy(ovrSceneRoom.Ceiling.gameObject);
                ceilingDestroyed = true; // Set the flag to indicate the ceiling has been destroyed
                Debug.Log("Ceiling found and destroyed.");
            }
            else
            {
                Debug.Log("Ceiling not found.");
            }
        }
    }
}
