using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject previewPrefab;
    private GameObject currentPreview;
    // Start is called before the first frame update
    void Start() => currentPreview = Instantiate(previewPrefab);


    // Update is called once per frame
    private void Update()

    {
        Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentPreview.transform.position = hit.point;
            currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                // Calculate the flat direction from the controller to the hit point (ignoring Y-axis differences)
                Vector3 flatDirectionFromController = (hit.point - ray.origin);
                flatDirectionFromController.y = 0; // Remove Y component to avoid tilt
                flatDirectionFromController.Normalize();

                // Ensure we have a valid direction
                if (flatDirectionFromController == Vector3.zero)
                {
                    flatDirectionFromController = Vector3.forward; // Default direction if directly above/below
                }

                // Create a rotation that looks away from the controller, with no tilt
                Quaternion rotation = Quaternion.LookRotation(flatDirectionFromController, Vector3.up);

                Instantiate(prefab, hit.point, rotation);
            }



        }
    }
}
