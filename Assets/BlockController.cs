using UnityEngine;

public class BlockController : MonoBehaviour
{
    // Adjust the mass to make the object heavier and harder to move
    public float mass = 1f;

   
  

    private void Start()
    {
        // Add Rigidbody component to the object
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        // Set Rigidbody properties
        rb.mass = mass;
       
     
     
    }
}
