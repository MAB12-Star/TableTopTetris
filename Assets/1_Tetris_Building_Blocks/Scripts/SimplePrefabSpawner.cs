using System.Collections;
using UnityEngine;

public class SimplePrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    public GameObject previewPrefab;
    private GameObject currentPreview;

    // Start is called before the first frame update
    void Start()
    {
        currentPreview = Instantiate(previewPrefab);
       
    }

    private void Update()
    {
        Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentPreview.transform.position = hit.point;
            currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                // Set the rotation to 0 degrees on the Y-axis
                Quaternion rotation = Quaternion.Euler(0, 0, 0);

                // Instantiate the prefab
                Instantiate(prefab, hit.point, rotation);

                DeletePrefab();
                Debug.Log("DeletePrefab method called.");
            }
        }
    }

    public void ActivateSpawner()
    {
        gameObject.SetActive(true);
        Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentPreview.transform.position = hit.point;
            currentPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                // Set the rotation to 0 degrees on the Y-axis
                Quaternion rotation = Quaternion.Euler(0, 0, 0);

                // Instantiate the prefab
                Instantiate(prefab, hit.point, rotation);

                DeletePrefab();
                Debug.Log("DeletePrefab method called.");

            }
        }
    }

    public void DeletePrefab()
    {
        // Find all GameObjects with the tag "delete" and destroy them
        GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("Delete");
        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
    }
}


  
  

