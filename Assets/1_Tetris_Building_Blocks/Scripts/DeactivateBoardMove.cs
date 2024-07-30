using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class DeactivateBoardMove : MonoBehaviour
{
    public bool isBoardActive = true;
    private GameObject grab;

    // Start is called before the first frame update
    void Start()
    {
        grab = GameObject.Find("Grab");
        if (grab == null)
        {
            Debug.LogError("Grab GameObject not found!");
        }
    }

    // Method to activate the board
    public void BoardUpdateActivate()
    {
        if (!isBoardActive && grab != null)
        {
            GrabInteractable grabbableComponent = grab.GetComponent<GrabInteractable>();

            if (grabbableComponent != null)
            {
                grabbableComponent.enabled = true;
                isBoardActive = true; // Update the flag
                Debug.Log("Grabbable component activated.");
            }
            else
            {
                Debug.LogWarning("Grabbable component is missing on the Grab GameObject.");
            }
        }
    }

    // Method to deactivate the board
    public void BoardUpdateDeactivate()
    {
        if (isBoardActive && grab != null)
        {
            GrabInteractable grabbableComponent = grab.GetComponent<GrabInteractable>();
            if (grabbableComponent != null)
            {
                grabbableComponent.enabled = false;
                isBoardActive = false; // Update the flag
                Debug.Log("Grabbable component deactivated.");
            }
            else
            {
                Debug.LogWarning("Grabbable component is missing on the Grab GameObject.");
            }
        }
    }
}
