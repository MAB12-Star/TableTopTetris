using UnityEngine;

public class OculusRayInteractor : MonoBehaviour
{
    public Transform rayOrigin; // Assign the controller transform here
    public GameObject objectToSpawn; // Assign the prefab you want to spawn

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) // Check if trigger is pressed
        {
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit))
            {
                // Spawn the object at the hit point
                Instantiate(objectToSpawn, hit.point, Quaternion.identity);
            }
        }
    }
}
